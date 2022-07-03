using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using KyleDulce.SocketIo;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManagerQuestionScene : MonoBehaviour
{
    
    public Animator QuestionPanel;
    public Animator FullAnswersPanel;
    public Animator FullAnswersButton;
    public Animator QuestionButton;
    public GameObject SecretCapitalButtonVisible;
    public GameObject SecretCapitalButtonHidden;
    public GameObject CapitalText;

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
        Debug.Log("[+++] uimanager question scene onsceneloaded has been called");

        this._socket.on("showFinalResult", showFinalCall);
        
        Debug.Log("[+++] showFinalResult has been registerd");

        this._socket.on("initQuestion", initQuestionCall);

        //get capitals scores
        this._socket.emit("getCurScores", "");
        this._socket.on("receiveScores", receiveCurScoresCall);

        Debug.Log("[+++] initquestion has been registerd ");
        
        // this._socket.on("showFinalResult", (data) =>
        // {
        //     
        //     Debug.Log("[+++] show final result listener has been triggered ");
        // });
        // Debug.Log("[+++] showFinalResult has been registerd");
        //
        // this._socket.on("initQuestion", ( data) =>
        // {
        //     Debug.Log("[+++] initQuestion has been triggerd");
        //     Debug.Log("[+] data in q controller"+data);
        //
        //     GameObject.Find("QuestionPanelText")
        //         .GetComponent<TextMeshProUGUI>()
        //         .text = "* " + data + " *";
        //
        // });
        //
        // Debug.Log("[+++] initquestion has been registerd ");
        this._socket.emit("nextQuestion", "");
        Debug.Log("[+++] nextQuestion has been emitted to server");

        
    }
    
    void OnDisable()
    {
        this._socket.off("showFinalResult");
        this._socket.off("initQuestion");
        this._socket.off("receiveCurScores");

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    // Start is called before the first frame update
    // void Start()
    // {
    //     var client = GlobalState.client;
    //     this._socket = client.socket;
    //
    //     this._socket.on("showFinalResult", (data) =>
    //     {
    //         Debug.Log("[+] show final result scene");
    //     });
    //
    //     this._socket.on("initQuestion", ( data) =>
    //     {
    //         Debug.Log("[+] data in q controller"+data);
    //
    //         GameObject.Find("QuestionPanelText")
    //             .GetComponent<TextMeshProUGUI>()
    //             .text = "* " + data + " *";
    //
    //     });
    //     this._socket.emit("nextQuestion", "");
    // }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showFinalCall(String data)
    {
        SceneManager.LoadScene("FinalResultsScene");
        // this._socket.emit("getNextScene", "FinalResultsScene");
    }

    public void initQuestionCall(String data)
    {
        Debug.Log("[+++] initQuestion has been triggerd");
        Debug.Log("[+] data in q controller"+ data);
        
        GameObject.Find("QuestionPanelText")
            .GetComponent<TextMeshProUGUI>()
            .text = "* " + data + " *";
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

        //set capital
        string curCap = PlayerPrefs.GetString("currentCapital");
        CapitalText.GetComponent<TextMeshProUGUI>().text = curCap;
        Debug.Log("current capital is: " + curCap);

    }




    public void showQuestion()
    {
        
        FullAnswersPanel.SetBool("isHidden", true);
        QuestionPanel.SetBool("isHidden", false);
        FullAnswersButton.SetBool("isHidden", false );
        QuestionButton.SetBool("isHidden", true);
    }

    public void showFullAnswers()
    {
       
        QuestionPanel.SetBool("isHidden", true);
        FullAnswersButton.SetBool("isHidden", true);
        QuestionButton.SetBool("isHidden", false);
        FullAnswersPanel.SetBool("isHidden", false);

    }

    public void showShortAnswers()
    {      
        QuestionPanel.SetBool("isHidden", true);
        FullAnswersPanel.SetBool("isHidden", true);
        
        FullAnswersButton.SetBool("isHidden", false);

    }

    public void ShowSecretCapitalOnClick()
    {

      

        SecretCapitalButtonVisible.SetActive(SecretCapitalButtonHidden.active);
        SecretCapitalButtonHidden.SetActive(!SecretCapitalButtonVisible.active);

    }

    //public void AnswerQuestionOnClick2()
    //{
    //    // TODO: get given answer number from button and add this to emit 
    //    // this._socket.emit("getNextScene", "ResultScene");
    //    SceneManager.LoadScene("ResultScene");
    //    this._socket.emit("answerQuestion", "0");
    //    Debug.Log("[+++] answerQuestion has been emitted to server");



    //    //!sends answerquestion emit from PopulatePanel script.
    //}
}
