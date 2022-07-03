using UnityEngine;
using KyleDulce.SocketIo;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class SioLobbyController : MonoBehaviour
{
     private Socket _socket;

    void Start()
    {
        Debug.Log("SioLobbyController Start");
		//The url must include "ws://" as the protocol
        // var client = new SocketIOClientDi();
        var client = GlobalState.client;
        this._socket = client.socket;
        
        // !!! Ack entry point, needed in order to make it work!!!
        // You can only initially start emiting events from here!
        // this._socket.on("ack", (string data) =>
        // {
        //     
        // });
        if (!client.initialized)
        {
            // Debug.Log("RECEIVED EVENT ack: " + data);
            if (PlayerPrefs.HasKey("fromCreateGame"))
            {
                if (PlayerPrefs.GetInt("fromCreateGame") == 1)
                {
                    this._socket.emit("createRoom", string.Empty);
                }
                    
                // BUG: Anoyingly either cross-scene sharing or the
                //  client (which will mean the singleton won't work), don't work 
                //  so the event won't get emmited from the previous scene propper
                else if (PlayerPrefs.GetInt("fromCreateGame") == 0)
                {
                    var roomCode = PlayerPrefs.GetString("roomCode");
                    if(roomCode != string.Empty)
                    {
                        this._socket.emit("joinRoom", roomCode);
                    }
                }
            }
        }

        client.initialized = true;
        
        this._socket.on("receiveGameRoomCode", (string data) =>
        {
            Debug.Log("Room code1 = " + data);
            GameObject.Find("GameCodeLabelText")
                .GetComponent<TextMeshProUGUI>().text = data;

            


        });

        this._socket.on("updatePlayerCountRoom", (string data) =>
        {
            Debug.Log("received updatePlayerCountRoom with " + data);            
            GameObject playerPanel = GameObject.Find("PlayerIconsPanel");
            playerPanel.GetComponent<PopulatePlayerPanel>().Populate(Int32.Parse(data));
        });

        
    }



    

    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.Start();
        // Log scene change
        this._socket.on("receiveGameRoomCode", (string data)=>
        {
            Debug.Log("Room code2 = " + data);
            GameObject.Find("GameCodeLabelText")
                .GetComponent<TextMeshProUGUI>().text = data;

           


        });
    }
    
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }



}