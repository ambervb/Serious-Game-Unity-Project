using UnityEngine;
using KyleDulce.SocketIo;
using TMPro;
using UnityEngine.SceneManagement;


public class SioQuestionController : MonoBehaviour
{
     private Socket _socket;

    void Start()
    {
        Debug.Log("SioQuestionController Start");
		//The url must include "ws://" as the protocol
        // var client = new SocketIOClientDi();
        var client = GlobalState.client;
        this._socket = client.socket;
        
        
        // !!! Ack entry point, needed in order to make it work!!!
        // You can only initially start emiting events from here!
        // this._socket.on("ack", (string data) =>
        // {
        //     
        // }}
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
}