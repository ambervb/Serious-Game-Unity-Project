using UnityEngine;

// Create a static class that holds the SocketIOClientSingleton class
// public class GlobalState
// {
//    // Create a static property that holds the SocketIOClientSingleton class
//    public static SocketIOClientSingleton socketIOClientSingleton { get; set; }
// }

public class GlobalState : MonoBehaviour
{
    public static SocketIOClientDi client { get; set; }
    void Awake() => client = new SocketIOClientDi(url:"ws://159.223.225.254:80");
    
}


