using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerAnswersScene : MonoBehaviour
{
    // Start is called before the first frame update


    // public void NextScene()
    // {
    //     SceneManager.LoadScene("LobbyScene");
    // }



    public void Start()
    {
        //TODO: check if game has minimum of 3 players, start game if true
        //SceneManager.LoadScene("SecretMissionScene");
        Debug.Log("called Start");

    }

    // public void showQuestion()
    // {
    //     
    //     // SceneManager.LoadScene("QuestionScene");
    // }
    //
    // public void showAnswers()
    // {
    //     SceneManager.LoadScene("AnswerScene");
    // }
}
