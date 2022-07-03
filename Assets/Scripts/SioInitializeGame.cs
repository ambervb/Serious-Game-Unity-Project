using System;
using System.Collections;
using System.Collections.Generic;
using KyleDulce.SocketIo;
using UnityEngine;

public class SioInitializeGame : MonoBehaviour
{
    private Socket _socket;

    void Start()
    {
        var client = GlobalState.client;
        this._socket = client.socket;
        
        
        this._socket.emit("lobbyStartGame",String.Empty);

        this._socket.emit("initializeGame",String.Empty);

        this._socket.on("receiveCapitals", (string data) =>
        {  
            Debug.Log("Received capitals receiveCapitals2: " + data);

            //set capital in playerprefs 
            string currentCapital = data;
            PlayerPrefs.SetString("currentCapital", currentCapital);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
