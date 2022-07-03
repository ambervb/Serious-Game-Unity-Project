const express = require("express");
const app = express();
const port = 80;
const Router = express.Router;
const router = new Router();

const multer = require('multer');
const csv = require('fast-csv');
const fs = require('fs');
const upload = multer({ dest: 'tmp/csv' });

const capitals = ["human", "maufactured", "intellectual", "social", "natural", "financial"];

questions = []
const games = {};
const server = require("http").createServer(app);

server.listen(port, () => {
	console.log(`[*] Server listening at port ${port}`);
});


const { instrument } = require("@socket.io/admin-ui");


const io = require("socket.io")(server, {
	cors: {
		origin: ['*', 'https://admin.socket.io']
	}
});



//// Middleware to handle all sio requests




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




io.on("connection", (socket) => {
	console.log("[*] New connection established.");
	socket.emit("ack", "");
	

	socket.on("createRoom", (data) => {
			data = data.replace(/[^\x00-\x7F]/g, "");
			console.log("[+] Received createRoomRequest Event " + data);
			let randomCode = Math.floor(9999 + Math.random() * (9999 - 1000 + 1));
			let gameCode = randomCode.toString()
			socket.emit("receiveGameRoomCode", gameCode);
			socket.emit("playerIsRoundsChairman", "")
		
			socket.emit("gameCanGoToSecretCapitals", "") // TODO: REMOVE!!
		
		
		

			let game = {
						"players": [{ "socketId": socket.id, "host": true }],
						"questions" : createQuestionDeck(),
						"capitals": getCapitals(),
						"round": 1
					};
			games[gameCode] = game;
		
			socket.emit("playerJoinedGame", 1);
			socket.join(gameCode)
			console.log(games)
		});

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
				socket.emit("playerJoinedGame", game.players.length);

				// Join the sio room by gamecode
				socket.join(data)
				console.log(games)

				// If => 3 players send an event to start the game.
				// Enables the start game button client side and send
				// startGame request.
				
				if (game.players.length >= 3) {
					console.log("[+] Enough players to start game");
					
					// TODO: actually emit this to the room!
					// let room = io.sockets.sockets[socket.id].rooms[0];
					socket.emit("gameCanGoToSecretCapitals", "")
				}
			}
		} else {
			console.log("[!] Error, game room not found");
			socket.emit("Error", "Room code does not exist!")
		}
	});


	socket.on("lobbyStartGame", (data) => {
		socket.emit("Error", "XXX***33")
		socket.emit("receiveCapitals", "XXXXXXXXXfeaX")

	})

	// =>3 players have joined the game.
	// Check if player that makes reqeust is host.
	socket.on("initializeGame", (data) => {
		data = data.replace(/[^\x00-\x7F]/g, "")
		

		// Because the middleware doesn't seem to fire on a per-event basis this is needed :(
		// let playerAndGame = getPlayerAndGameFromSocket(socket.id)
		// console.log("[+] Game is: " + playerAndGame.game)
		// console.log("[+] Player is: " + playerAndGame.player)

		socket.emit("Error", "XXX***8")


		socket.emit("receiveCapitals", "XXXXXXXXX5X")

		// Check if the player is a chairman
		// Loop through the capitals using the generator
		// for (let [index, capital] of secretCapitalsGenerator(playerAndGame.game.players.length)) {
		// 	// Get the player by index
		// 	let gameParticipant = playerAndGame.game.players[index];
		// 	console.log("[+] Emitting secret capital '" + capital + "' to player: " + gameParticipant.socketId);
		// 	// Send the capital to the socket id of the player
		// 	// io.to(gameParticipant.socketId)
		//	
		// 	// socket.emit("receiveCapitals", capital)
		// 	// socket.emit("Error", "x1"+capital)
		// 	// socket.emit("gameCanGoToSecretCapitals", "")
		//
		//
		// 	socket.emit("receiveCapitals", "XXXXXXXXXX")
		//	
		// }
	})

	// Send first question action
	socket.on("startGame", (data) => {
		data = data.replace(/[^\x00-\x7F]/g, "");
		
		
		// Emit an event to the room with the first question

		// Send question 

	})

	// Respond to event emmited when client has answerd a question
	socket.on("answerQuestion", (data) => {
		data = data.replace(/[^\x00-\x7F]/g, "");
		game = games[data];
		game.round = game.round + 1;
		let room = io.sockets.sockets[socket.id].rooms[0];

		// C̶h̶e̶c̶k̶ ̶i̶f̶ ̶p̶l̶a̶y̶e̶r̶ ̶a̶n̶s̶w̶e̶r̶i̶n̶g̶ ̶i̶s̶ ̶t̶h̶e̶ ̶c̶h̶a̶i̶r̶m̶a̶n̶

		// let answer = 
		
		// Check if game has ended
		if(game.round >= 7){
			// Emit end game event to the room including the effects
			// room.emit("endGame", effects)
		}
		

		// Check if answer contains dice roll 
		
		// and emit roll dice event

		// Compute effect (effects being the effects caused by the answer)

		

		// Send back effects to the room
		// room.emit(effects)
		
		// Send back next question to the room
		
	})

	
	
	
	
	
	
	// utils

	function sendQuestion() {

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
			yield [i,capitals[i]];
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

	function rollDice() {
		return Math.floor(Math.random() * 6);
	}

	

	
});


router.get("/", (req, res) => {
	res.send(`
	<!-- TODO: Check if the multipart should be enabled or disabled-->
	<form enctype="multipart/form-data" accept=".csv"  action="/" method="POST">
		Select a CSV file with questions: <br/><br/>
		<input type="file" /><br/><br/>
		<input type="submit" value="Upload questions" />
	</form>
	`);
})

router.post('/', upload.single('file'), function (req, res) {
	const fileRows = [];
	csv.fromPath(req.file.path).on("data", function (data) {
		fileRows.push(data);
	}).on("end", function () {

		questions = fileRows;
		console.log(questions);
	})
	res.send(fileRows);

});
router.get('/questions', function (req, res) {
	res.send(questions);
});
app.use('/', router);

instrument(io, {auth: false})

