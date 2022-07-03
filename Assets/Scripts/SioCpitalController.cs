using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KyleDulce.SocketIo;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;

public class SioCpitalController : MonoBehaviour
{
    // Start is called before the first frame update
    private Socket _socket;
    private JObject capitals;
    void Start()
    {
        Debug.Log("[+] siocapitalController start called");
        var client = GlobalState.client;
        this._socket = client.socket;
        this._socket.on("initCapitals", (data) =>
        {
            Debug.Log("[+] data in siocap controller" + data);
            this.capitals = JObject.Parse(data);
            Populate();
        });

    }

    public void Populate()
    {
        // TODO: prefab capitals have to be init with capitals JObject
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
