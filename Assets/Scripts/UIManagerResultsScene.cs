using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using KyleDulce.SocketIo;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerResultsScene : MonoBehaviour
{
    public GameObject SecretCapitalButtonVisible;
    public GameObject SecretCapitalButtonHidden;
    private bool lastQuestion;
    private int questionsLeft = 0;
    private int dice;
    
    
    private Socket _socket;
    
    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // this.Start();
        // Log scene change
        var client = GlobalState.client;
        this._socket = client.socket;
        
        Debug.Log("[+++] resultScene onSceneLoaded has been called");

        this._socket.on("showAnswerResult", showAnswerResultCall);
        
        Debug.Log("[+++] showAnswerResult has been registered ");

        //get capitals scores
        this._socket.emit("getCurScores", "");
        this._socket.on("receiveScores", receiveCurScoresCall);

 
    }
    
    void OnDisable()
    {
        this._socket.off("showAnswerResult");

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
   

    // Update is called once per frame
    void Update()
    {

    }

    public void showAnswerResultCall(String data)
    {
        Debug.Log("[+++] showAnswerResult has been triggered ");
        // Deserialize data variable to a JSON object
        JObject json = JObject.Parse(data.ToString());

        Debug.Log("[+] answerResult form server " + data);
            
        // Get text input called QuestionResultText
        var questionResultText = GameObject.Find("QuestionResultText").GetComponent<TextMeshProUGUI>();
        questionResultText.text = json["anserResult"].ToString();
            
       
        if (json["dice"].ToString() == "1")
        {
            this._socket.emit("rollTheDice", "");
            this._socket.emit("calculateAffects", "");
            // SceneManager.LoadScene("DiceScene");
            this.dice = 1;

            //show dice or next question button
            GameObject.Find("DiceOrQuestionText").GetComponent<TextMeshProUGUI>().text = "Roll Dice";
        }
        else
        {
            
            //
            this._socket.emit("calculateAffects", "");
            // SceneManager.LoadScene("QuestionScene");
            this.dice = 0;
            // // Debug.Log("[+++] calculate effects has been emitted to server");

        }

        
        

    }

    //show current scores
    public void receiveCurScoresCall(string data)
    {
        Debug.Log("[+++] receiveCurScores called");
        Debug.Log("[+++] receiveCurScores called with data: " + data);
        //populate scorepanel
        GameObject scoresPanel = GameObject.Find("ScoresPanel");
        PopulateCapitalScores scoresPanelScript = scoresPanel.GetComponent<PopulateCapitalScores>();

        scoresPanelScript.PopulateCurrentScores(data);

    }

    public void nextQuestionOnClick()
    {
        if (this.dice == 1)
        {
            // this._socket.emit("getNextScene", "DiceScene");

            //add
            this._socket.emit("rollTheDice", "");
            SceneManager.LoadScene("DiceScene");
        }
        else if(this.dice == 0)
        {
            //this._socket.emit("getNextScene", "QuestionScene");
            SceneManager.LoadScene("QuestionScene");

            //temporary to test final scene
            //SceneManager.LoadScene("FinalResultsScene");
        }
    }
    public void ShowSecretCapitalOnClick()
    {
        SecretCapitalButtonVisible.SetActive(SecretCapitalButtonHidden.active);
        SecretCapitalButtonHidden.SetActive(!SecretCapitalButtonVisible.active);
    }
}
