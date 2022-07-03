using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using KyleDulce.SocketIo;
using UnityEngine.SceneManagement;


public interface ISocketIoClient
{
}
public class SocketIOClientDi : ISocketIoClient
{ 
    // private static SocketIOClientSingleton _instance;

    public bool initialized = false;
    public Socket socket;
    private string url;
    
    public SocketIOClientDi(string url="ws://localhost:3000", bool autoConnect=true){
        Debug.Log("Initiated");
        this.url=url;
        socket = SocketIo.establishSocketConnection(url);
        if (autoConnect) socket.connect();
        socket.on("Error", (data) => {
            Debug.LogError(data);
        });
        // socket.on("LoadScene", (data) =>
        // {
        //     Debug.Log("====== Loadscene " + data);
        //     SceneManager.LoadScene(data);
        // });

    }
    

    // public static SocketIOClientSingleton Instance 
    // { 
    //     get { return _instance; } 
    // } 
    //
    // private void Awake() 
    // { 
    //     if (_instance != null && _instance != this) 
    //     { 
    //         Destroy(this.gameObject);
    //         return;
    //     }
    //
    //     _instance = this;
    //     DontDestroyOnLoad(this.gameObject);
    // }
}