/*
* 
* Imports
* 
*/
const { instrument } = require("@socket.io/admin-ui");
const express = require("express");
const fileUpload = require('express-fileupload');
const multer = require("multer");
const http = require("http");
const csv=require('csvtojson');
const sio = require("socket.io");
const fs = require("fs");
const { Console } = require("console");






/*
* 
* Variables 
* 
*/

// Functionality
const capitals = ["human", "manufactured", "intellectual", "social", "natural", "financial"];
const questions = parseExcelFileToQuestions('./default_questions.xlsx');
const games = {}

// Express
const app = express();
const port = 80;
const Router = express.Router;
const router = new Router();

app.use(fileUpload());


/*
* 
* Setup server listeners
* 
*/

const server = http.createServer(app);
server.listen(port, () => {
    console.log(`[*] Server listening at port ${port}`);
});

// Setup socket.io
const io = sio(server, {
    cors: { origin: "*"}
});


/*
* 
* Socket.io middleware
* 
*/
io.use((packet, next) => {
    try {
        console.log("[*] Socket.io middleware called");

        // Strip non utf-8 characters
        packet.data = packet.data.toString().replace(/[^\x00-\x7F]/g, "");

        // Get the socket id
        let socketId = Array.from(packet.nsp.sockets.keys())[0]

        // Check if socket is equal to the socketId of any player in any game
        // assign the game and the player to the socket
        let playerAndGame = getPlayerAndGameFromSocket(socketId)
        packet.game = playerAndGame.game;
        packet.player = playerAndGame.player;


    } catch (err) { }
    next();
});


/*
* 
* Socket.io setup
* 
*/

io.on("connection", (socket) => {
    // On connection
    console.log("[*] New connection established.");
    socket.emit("ack", "");


    /*
    * Routes
    */

    // Create room
    socket.on("createRoom", (data) => {
        data = data.replace(/[^\x00-\x7F]/g, "");
        console.log("[+] Received createRoomRequest Event " + data);
        let randomCode = Math.floor(9999 + Math.random() * (9999 - 1000 + 1));
        let gameCode = randomCode.toString()
        socket.emit("receiveGameRoomCode", gameCode);
        // socket.emit("playerIsRoundsChairman", "")
        
        socket.emit("gameCanGoToSecretCapitals", "") // TODO: REMOVE!!

        let game = {
            "gameCode" : gameCode,
            "players": [{ "socketId": socket.id, "host": true }],
            "questions": createQuestionDeck(),
            "capitals": getCapitals(),
            "round": 1
        };
        games[gameCode] = game;
        // socket.emit("playerJoinedGame", 1); // TODO: THIS LINE CAUSED THE BUG!!! DONT INCLUDE EMISSIONS UNLESS YOU LISTEN TO THEM!!!!!!
        socket.join(gameCode)

        //update player count shown to user
        var playerCount = io.sockets.adapter.rooms.get(gameCode).size;
        io.in(gameCode).emit("updatePlayerCountRoom", playerCount.toString());
        console.log(games)
    });


    // Join room
    socket.on("joinRoom", (data) => {
        data = data.replace(/[^\x00-\x7F]/g, ""); // Removes all non-ASCII characters
        console.log("[+] Received joinRoom Event " + data)
        let game = games[data];
        if (game != null) {

            console.log("[+] Game room found.");
            if (game.players.length >= 5) {
                console.log("[!] Error, game room is full");
                socket.emit("Error", "too many players!")
            }
            else {
                console.log("[+] Game room found and is not full");

                // Send game code to client
                socket.emit("receiveGameRoomCode", data);

                // Add to games and join sioroom
                game.players.push({ "socketId": socket.id, "host": false, })
                // socket.emit("playerJoinedGame", game.players.length);

                // Join the sio room by gamecode
                socket.join(data)
                console.log(games)

                //Update player count shown to user
                var playerCount = io.sockets.adapter.rooms.get(gameCode).size;
                io.in(data).emit("updatePlayerCountRoom", playerCount.toString());

                // If => 3 players send an event to start the game.
                // Enables the start game button client side and send
                // startGame request.

                if (game.players.length >= 3) {
                    console.log("[+] Enough players to start game");

                    
                    // let room = io.sockets.sockets[socket.id].rooms[0];
                    // DONE: actually emit this to the room
                    // TODO: must be handled at the client
                    // io.in(data).emit("EnoughPlayersToStart","")
                    socket.emit("gameCanGoToSecretCapitals", "")
                }
            }
        } else {
            console.log("[!] Error, game room not found");
            socket.emit("Error", "Room code does not exist!")
        }
    });

    // Initialize game
    // =>3 players have joined the game.
    // Check if player that makes reqeust is host.
    socket.on("initializeGame", (data) => {
        data = data.replace(/[^\x00-\x7F]/g, "")

        // Because the middleware doesn't seem to fire on a per-event basis this is needed :(
        let playerAndGame = getPlayerAndGameFromSocket(socket.id)
        
        // Check if the player is a chairman
        // Loop through the capitals using the generator
        for (let [index, capital] of secretCapitalsGenerator(playerAndGame.game.players.length)) {
            // Get the player by index
            let gameParticipant = playerAndGame.game.players[index];
            gameParticipant["secretCapital"] = capital;
            console.log("[+] Emitting secret capital " + capital + " to player: " + gameParticipant.socketId);
            // Send the capital to the socket id of the player
            io.to(gameParticipant.socketId)
                .emit("receiveCapitals", capital)
        }
        

        // else {
        // 	socket.emit("Error", "Can't initializeGame user is not host!");
        // }
    })
    socket.on("getNextScene",(data)=>{
        data = data.replace(/[^\x00-\x7F]/g, "");
        let playerAndGame = getPlayerAndGameFromSocket(socket.id)
        console.log("[+] scnene called"+ data)
        
        io.in(playerAndGame.game.gameCode.toString()).emit("LoadScene", data);
        
    })

    // Send first question action
    socket.on("startGame", (data) => {
        data = data.replace(/[^\x00-\x7F]/g, "");
        let playerAndGame = getPlayerAndGameFromSocket(socket.id)
      
        
        // Send question 
       
        io.in(playerAndGame.game.gameCode.toString()).emit("initQuestion",playerAndGame.game.questions[0].text);
        // Send answers
        io.in(playerAndGame.game.gameCode.toString()).emit("initAnswers", getAnswerStrArr(playerAndGame.game.questions[playerAndGame.game.round - 1 ]).toString())
        // Send capitals
        io.in(playerAndGame.game.gameCode.toString()).emit("initCapitals", JSON.stringify(playerAndGame.game.capitals));
    })
    socket.on("nextQuestion",(data)=>{
        data = data.replace(/[^\x00-\x7F]/g, "");
        let playerAndGame = getPlayerAndGameFromSocket(socket.id)
        let curGame = playerAndGame.game;
        
        if(curGame.round >= 3){
            io.in(playerAndGame.game.gameCode.toString()).emit("showFinalResult","");
        }else{
            // Send question 
            // io.in(playerAndGame.game.gameCode.toString()).emit("initQuestion",playerAndGame.game.questions[curGame.round -1].text);
            //temp shortcut
            io.in(playerAndGame.game.gameCode.toString()).emit("initQuestion",playerAndGame.game.questions[2].text);
            // Send answers
            io.in(playerAndGame.game.gameCode.toString()).emit("initAnswers", getAnswerStrArr(playerAndGame.game.questions[playerAndGame.game.round - 1 ]).toString())
            // Send capitals
            io.in(playerAndGame.game.gameCode.toString()).emit("initCapitals", JSON.stringify(playerAndGame.game.capitals));

        }
    })

    // Respond to event emmited when client has answerd a question
    socket.on("answerQuestion", (data) => {
        data = data.replace(/[^\x00-\x7F]/g, "");
        let playerAndGame = getPlayerAndGameFromSocket(socket.id)
        let curGame = playerAndGame.game;
        
        let curAnswer = curGame.questions[curGame.round - 1].answers[parseInt(data)]
        
        console.log("[+] answerQuestion has been emitted to server")
        
        // C̶h̶e̶c̶k̶ ̶i̶f̶ ̶p̶l̶a̶y̶e̶r̶ ̶a̶n̶s̶w̶e̶r̶i̶n̶g̶ ̶i̶s̶ ̶t̶h̶e̶ ̶c̶h̶a̶i̶r̶m̶a̶n̶

        
        // Check if answer contains dice roll and add to response
        
        let curAnsweRes= {};
        curAnsweRes["anserResult"] = curAnswer.result
        curAnsweRes["affects"] = curAnswer.affects;
        curAnsweRes["dice"] = curAnswer.dice;
        
        console.log("[+] amswerreponse" + JSON.stringify(curAnsweRes));
        // io.in(playerAndGame.player.socketId).emit("showAnswerResult", JSON.stringify(curAnsweRes));


        curGame["curAnsweRes"] = curAnsweRes;
        
        
        

        // and emit roll dice event

        // Compute effect (effects being the effects caused by the answer)



        // Send back effects to the room
        // room.emit(effects)

        // increase round
        curGame.round = curGame.round + 1;

        console.log("[+] showAsnwerResult has been emitted to client")
        // Send back next question to the room
        io.in(curGame.gameCode.toString()).emit("showAnswerResult", JSON.stringify(curGame.curAnsweRes));
        
    });


    //show capital scores
    socket.on("getCurScores",(data)=>{
        console.log("receives getCurScores")
        data = data.replace(/[^\x00-\x7F]/g, "");
        let playerAndGame = getPlayerAndGameFromSocket(socket.id)
        let curGame = playerAndGame.game;

        let curScores = getCapitals();
        console.log("capitals scores: " + JSON.stringify(curGame.capitals))
        // io.in(curGame.gameCode.toString()).emit("receiveScores", JSON.stringify(JSON.stringify(curScores)));
        io.in(curGame.gameCode.toString()).emit("receiveScores", JSON.stringify(curGame.capitals));
    });



    
    socket.on("rollTheDice",(data)=>{
        data = data.replace(/[^\x00-\x7F]/g, "");
        let playerAndGame = getPlayerAndGameFromSocket(socket.id)
        let curGame = playerAndGame.game;

        let curAnswer = curGame.curAnsweRes;
        
        curAnswer["rolledDice"] = rollDice();
        console.log("rolled dice with  " + JSON.stringify(curAnswer.rollDice));
        //add
        //temporarily hardocded to test, look into how to get leftover questions num in game
        curAnswer["questionsLeft"] = "0";

        console.log("QuestionsLeft is " + JSON.stringify(curAnswer.questionsLeft));
        io.in(curGame.gameCode.toString()).emit("receiveDice",  JSON.stringify(curAnswer));
    });
    
    socket.on("getCapitals",(data)=>{
        data = data.replace(/[^\x00-\x7F]/g, "");
        let playerAndGame = getPlayerAndGameFromSocket(socket.id)
        let curGame = playerAndGame.game;
        
        io.in(curGame.gameCode.toString()).emit("recieveCapitalStatus", JSON.stringify(curGame.capitals));
    });
    
    socket.on("calculateAffects", (data)=>{
        data = data.replace(/[^\x00-\x7F]/g, "");
        let playerAndGame = getPlayerAndGameFromSocket(socket.id)
        let curGame = playerAndGame.game;
        
        
        calcAffects(curGame);
        
    //    if answer has roll the dice 
    //      get effects on number of dice
    //      save to curgame
    
    //    else get effects on number of 1
    //      save to curgame
        
    //    calculate the new capitals and save to the curgame
        
    //    increase round mun 
        
    //    THIS CAN BE ADDED LATER THO, switch chairman 
    //     ;
    //     console.log("winner of this round "+ calcWinner(curGame))
        
    });
    
    socket.on("calculateWinnner", (data)=>{
        console.log("[+] received calculateWinner")
        data = data.replace(/[^\x00-\x7F]/g, "");
        let playerAndGame = getPlayerAndGameFromSocket(socket.id);
        let curGame = playerAndGame.game;
        
       
        let winnerGame = calcWinner(curGame);
        console.log("[+] calculateWinner is: " + winnerGame);

    //    calculate wich player and secret capital has 
        io.in(curGame.gameCode.toString()).emit("receiveWinner", winnerGame);
    //    THIS CAN BE ADDED LATER THO, if capitals are of balance every body looses
        
    });
  
});



/*
* 
* HTTP Routes
*/

router.get("/", (req, res) => {
    res.send(`
	<form enctype="multipart/form-data" action="/" method="POST">
		<h1><b>Select a Excel file with questions: </b><h1></h1><br/><br/>
		<input type="file" name="file" accept=".xlsx"/><br/><br/>
		<input type="submit" value="Upload questions" />
	</form>
	`);
});
router.post("/", (req, res) =>{
    if (!req.files || Object.keys(req.files).length === 0) {
        return res.status(400).send("<h1>No files were uploaded.</h1>");
    }
    let filename = "./uploads/" + "uploaded_questions_" + Math.random().toString(36).slice(2, 16) + ".xlsx";
    req.files.file.mv(filename, function(err) {
        if (err) return res.status(500).send(`<h1>Error uploading file.: {err}</h1>`);
        // questions = parseExcelFileToQuestions(filename);
        res.send('<h1>Questions updated</h1>');
    });
});
router.get("/questions", function (req, res) {
    res.send(questions);
});
app.use("/", router);




/*
* 
* Utilities
* 
*/

function calcWinner(curGame){
    let defaultCaps = getCapitals();
    
    // let players = curGame.players;
    
    for(let curCap of Object.values(curGame.capitals)){
        let val1 = Object.keys(curCap)[0];
        for(let capitalDefault of defaultCaps){
            if(Object.keys(capitalDefault)[0] == val1){
                
                curCap[val1] = curCap[val1] - Object.values(capitalDefault)[0];
            }
        }
    }
    // player met begin capital opslaan
    // dit dan aftrekken met eindkapitalen 
    // hierhoogste getal uithalen en terug sturen 
    console.log("break")
    let max = 0;
    let capWinner;
    for (let cap of curGame.capitals){
        let key = Object.keys(cap)[0];
        let val = Object.values(cap)[0]
        if(val > max){
            max = val;
            capWinner = key;
        }
    }
    return capWinner;    
}

function calcAffects(curGame){
    if(curGame.curAnsweRes.affects.length ==1){
        for (let effect of Object.values(curGame.curAnsweRes.affects[0])[0])
        {
            let val1 = Object.keys(effect)[0];
            for (let capital of curGame.capitals){
                if(Object.keys(capital)[0] == val1){
                    capital[val1] = Object.values(capital)[0] - Object.values(effect)[0];
                }
            }
        }
    }else{
        for (let effect of Object.values(curGame.curAnsweRes.affects[curGame.curAnsweRes.rolledDice])[0])
        {
            let val1 = Object.keys(effect)[0];
            for (let capital of curGame.capitals){
                if(Object.keys(capital)[0] == val1){
                    capital[val1] = Object.values(capital)[0] - Object.values(effect)[0];
                }
            }
        }
    }

}

function getCapitals(){
    return [
        { "human": 60.00 },
        { "manufactured": 35.00 },
        { "intellectual": 90.00 },
        { "social": 50.00 },
        { "natural": 25.00 },
        { "financial": 70.00 }
    ]
}

// Generate the question deck
const createQuestionDeck =()=> shuffle(questions).slice(0, 7);

// Generator for secret capitals
function *secretCapitalsGenerator(maxLen) {
    let shuffledCapitals = shuffle(capitals)
    // For loop because we also need the count
    for (let i = 0; i < maxLen; i++) {
        yield [i,shuffledCapitals[i]];
    }
}


function shuffle(array){
    const newArr = array.slice()
    for (let i = newArr.length - 1; i > 0; i--) {
        const rand = Math.floor(Math.random() * (i + 1));
        [newArr[i], newArr[rand]] = [newArr[rand], newArr[i]];
    }
    return newArr
}

function getPlayerAndGameFromSocket(socketId) {
    for (const [gameCode, game] of Object.entries(games)) {
        for (const [playerId, player] of Object.entries(game.players)) {
            if (player.socketId === socketId) {
                console.log("[*] Socket.io middleware found player");
                return { "game" : game, "player" : player }
            }
        }
    }

}
function getAnswerStrArr(curQuestion){
   var answerList = [];
   for( var i =0; i< curQuestion.answers.length; i++){
       answerList.push(curQuestion.answers[i].text)
   }
   return answerList;
}

function rollDice() {
    return Math.floor(Math.random() * 6);
}

function parseExcelFileToQuestions(filePath) {
    console.log("[*] Parsing excel file to questions " + filePath);
    let questions = []
    let XLSX = require("xlsx")
    let workbook = XLSX.readFile(filePath)
    let sheetName = workbook.SheetNames[0]
    let sheet = workbook.Sheets[sheetName]
    let excelQuestions = XLSX.utils.sheet_to_json(sheet, {
        header: 1,
        blankrows: true,
        defval: null,
    })
    let header = excelQuestions.shift()
    function addToAnswer(answer_index, answers, key, item) {
        if (!answers[answer_index]) answers[answer_index] = {}
        let answer = answers[answer_index]
        answer[key] = item
    }

    function addToAffect(affect_index, affects, key, item) {
        if (!affects[affect_index]) affects[affect_index] = []
        let affect = affects[affect_index]
        affect[key] = item
    }

    function getFromAnswer(answer_index, answers, key) {
        let answer = answers[answer_index]
        return answer[key] ? answer[key] : {}
    }

    function addAffectToAnswer(answers, answer_index, affects_dice, effect) {
        let answer = answers[answer_index]
        if (!answer.affects || answer.affects == null) answer.affects = []
        for (affect of answer.affects) {
            let key = Object.keys(affect)[0]
            if (key === affects_dice) {
                affect[key].push(effect)
                return
            }
        }
        obj1 = {}
        obj1[affects_dice] = []
        obj1[affects_dice].push(effect)
        answer.affects.push(obj1)

    }

    // loop through all questions in the excel sheet
    for (let excelQuestion of excelQuestions) {

        // initialize a new question object
        let question = {answers: []}

        // loop through all excel cells/question attributes
        for (let [index, question_item] of Object.entries(excelQuestion)) {
            let question_item_key = header[index]
            let dashed_question_item_key = question_item_key.split("-")

            if (question_item_key === "question") {
                question.text = question_item
            }

            if (question_item_key.startsWith("answer")) {
                let answer_index = parseInt(question_item_key.replace("answer", ""))
                addToAnswer(answer_index, question.answers, "number", index.toString())
                addToAnswer(answer_index, question.answers, "text", question_item)
            }

            // Get the results for each question
            // This should actually be done with a regex, but whatever
            if (dashed_question_item_key[0] === "answer" &&
                dashed_question_item_key.slice(-1)[0] === "result") {
                let answer_index = dashed_question_item_key[1]
                addToAnswer(answer_index, question.answers, "result", question_item)
            }
            if (dashed_question_item_key[0] === "answer" &&
                dashed_question_item_key.slice(-1)[0] === "dice") {
                let answer_index = dashed_question_item_key[1]
                addToAnswer(answer_index, question.answers, "dice", question_item)
            }
            // dice-answer-dice_number
            if (dashed_question_item_key[0] === "dice") {
                let answer_index = dashed_question_item_key[1]

                if (question_item) {
                    // console.log("[+] this " + question_item);
                    let effect_str = replaceAll(question_item,'"', "")
                    //  let effect_str = question_item.replaceAll('"', "")
                    let effect_strs = effect_str.split(",")
                    for (let effect_str of effect_strs) {
                        // split the effect string into its parts by :
                        let effect_parts = effect_str.split(":")
                        let effect_key = effect_parts[0]
                        let effect_value = effect_parts[1]
                        let effect = {}
                        effect[effect_key] = parseInt(effect_value)
                        addAffectToAnswer(question.answers, answer_index, dashed_question_item_key[2], effect)
                    }
                }
            }
        }
        question.answers = question.answers.filter(answer=>!!answer&&!!answer.text)
        questions.push(question)
    }
    return questions
}

function escapeRegExp(string) {
    return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'); // $& means the whole matched string
}

function replaceAll(str, find, replace) {
    return str.replace(new RegExp(escapeRegExp(find), 'g'), replace);
}


// End
instrument(io, {auth: false})
