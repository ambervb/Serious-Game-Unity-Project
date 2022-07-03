using System.Collections;
using System.Collections.Generic;
using KyleDulce.SocketIo;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManagerCapitalsScene : MonoBehaviour
{
        private Socket _socket;
    
        void Start()
        {
                Debug.Log("UiManagerCapitalsScene StartX4");
                var client = GlobalState.client;
                this._socket = client.socket;
                this._socket.on("lobbyStartGame", (string data) =>
                {  
                    Debug.Log("Received capitals receiveCapitals1: " + data);
                });
                this._socket.on("initializeGame", (string data) =>
                {  
                    Debug.Log("Received capitals receiveCapitalsinitializeGame7: " + data);
                });
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
