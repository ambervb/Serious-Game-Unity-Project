using System.Collections;
using System.Collections.Generic;
using KyleDulce.SocketIo;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    public Animator createGameButton;
    public Animator joinGameButton;
    public Animator enterGameCodePanel;
    //public Animator StartCreatedGameButton;
    
    public void Start()
    {
        

    }

    public void CreateGame()
    {
        Debug.Log("JoinCreatedGame");
        PlayerPrefs.SetInt("fromCreateGame", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("LobbyScene");
    }

    public void EnterGameCode()
    {
        createGameButton.SetBool("isHidden", true);
        joinGameButton.SetBool("isHidden", true);
        enterGameCodePanel.SetBool("isHidden", false);
    }

    public void CloseGameCodePanel()
    {
        createGameButton.SetBool("isHidden", false);
        joinGameButton.SetBool("isHidden", false);
        enterGameCodePanel.SetBool("isHidden", true);
    }

    public void JoinCreatedGame()
    {

        Debug.Log("Called JoinGameOncClickListener onclick listener");

        // Get code from text field
        var gameCodeText =  GameObject.Find("GameCodeInputFieldText")
            .GetComponent<TextMeshProUGUI>().text
            .Trim().ToLower();
        
        Debug.Log("Received JoinGameOncClickListener onclick button: " + gameCodeText);
        
        PlayerPrefs.SetString("roomCode", gameCodeText);
        PlayerPrefs.Save();

        Debug.Log("JoinCreatedGame");
        PlayerPrefs.SetInt("fromCreateGame", 0);
        PlayerPrefs.Save();
        
        //TODO: check if game has minimum of 3 players, start game if true
        SceneManager.LoadScene("Lobby");
    }

}
