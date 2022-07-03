using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using KyleDulce.SocketIo;
using TMPro;


public class SioJoinGameController : MonoBehaviour
{

    private Socket _socket;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        //The url must include "ws://" as the protocol
        // var client = new SocketIOClientDi();
        // var client = GlobalState.socketIOClientSingleton;// .socketIOClientSingleton;
        // var client =  new Sock
        var client = GlobalState.client;
        this._socket = client.socket;
        
        // !!! Ack entry point, needed in order to make it work!!!
        this._socket.on("ack", (string d) => Debug.Log("Acked"));
    }

    

    // TODO: An onclick method that emits a "join room event"
    public void JoinGameOncClickListener()
    {
        
        // Debug.Log("Called JoinGameOncClickListener onclick listener");
        //
        //
        //
        // // Get code from text field
        // var gameCodeText =  GameObject.Find("GameCodeInputFieldText")
        //     .GetComponent<TextMeshProUGUI>().text
        //     .Trim().ToLower();
        //
        // Debug.Log("Received JoinGameOncClickListener onclick button: " + gameCodeText);
        //
        //
        // PlayerPrefs.SetString("roomCode", gameCodeText);
        // PlayerPrefs.Save();
        //
        // Emit join room request with text from field
        // this._socket.emit("joinRoom", gameCodeText);
    }
    
    
}
