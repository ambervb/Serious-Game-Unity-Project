using System;
using System.Collections;
using System.Collections.Generic;
using KyleDulce.SocketIo;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManagerLobbyScene : MonoBehaviour
{
    private Socket _socket;
    public Sprite newSprite;
    

    // Start is called before the first frame update
    void Start()
    {
                var client = GlobalState.client;
                this._socket = client.socket;
                
                GameObject startGameButton = GameObject.Find("StartGameButton");

                if (PlayerPrefs.GetInt("fromCreateGame") == 0)
                {
                    // hide the start game button
                    startGameButton.SetActive(false);
                }
                this._socket.on("gameCanGoToSecretCapitals", (string data) =>
                {
                    Debug.Log("gameCanGoToSecretCapitals started called");
                    // get the button StartGameButton
                    startGameButton.GetComponent<Button>().image.sprite = newSprite; // Resources.Load<Sprite>("Sprites/Btn_Rectangle00_Yellow");
                    GameObject.Find("StartButtonText")
                        .GetComponent<TextMeshProUGUI>().text = "Start Game"; 
                });

                
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartCreatedGame()
    {
        //TODO: check if game has minimum of 3 players, enable startting game if true
        SceneManager.LoadScene("SecretCapitalScene");

    }

    public void goToMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
