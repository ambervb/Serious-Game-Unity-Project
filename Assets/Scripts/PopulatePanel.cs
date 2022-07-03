using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KyleDulce.SocketIo;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PopulatePanel : MonoBehaviour
{
    public GameObject answerPrefab;
    public int numberOfAnswers = 6;
    private Socket _socket;

    private List<String> answersArr;
    private int isHost = 1;

    // Start is called before the first frame update
    void Start()
    {
        var client = GlobalState.client;
        this._socket = client.socket;
        this._socket.on("initAnswers", ( data) =>
        {
            this.answersArr = data.Split(',').ToList();
            
            Populate();
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Populate()
    {
        GameObject newObj;

        for(int i = 0; i < answersArr.Count; i++)
        {
            newObj = (GameObject)Instantiate(answerPrefab, transform);

            newObj.GetComponent<Image>().name = "Answer" + (i+1);
            newObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = answersArr[i];


            //unclickable for non-host
            Button answerButton = newObj.GetComponent<Button>();
            answerButton.GetComponent<Button>().onClick.AddListener(AnswerQuestionOnClick);
            if (isHost == 0)
            {
                answerButton.interactable = false;
            }
            newObj.GetComponent<Image>().name = "Answer" + (i + 1);
        }
    }

    public void AnswerQuestionOnClick()
    {
        // TODO: get given answer number from button and add this to emit 
        //get button that was clicked
        var clickedButton = EventSystem.current.currentSelectedGameObject;
        Debug.Log("Clicked button was " + clickedButton.name);
        var buttonName = clickedButton.name;

        //get index from button that was clicked
        string answerIndex = buttonName.Substring(buttonName.Length - 1);
        Debug.Log("Clicked answer index was " + answerIndex);


        //respond to click
        int testanswerIndex = Int32.Parse(answerIndex);
        testanswerIndex = testanswerIndex - 1;
        SceneManager.LoadScene("ResultScene");
        this._socket.emit("answerQuestion", testanswerIndex.ToString());
        Debug.Log("[+++] answerQuestion has been emitted to server with " + testanswerIndex.ToString());


    }
}
