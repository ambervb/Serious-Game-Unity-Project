using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KyleDulce.SocketIo;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopulateCapitalScores : MonoBehaviour
{
   
    // Start is called before the first frame update
    private Socket _socket;
    private JArray capitals;
    void Start()
    {
        Debug.Log("[+] populate scores start called");


    }

    public void UpdateScoreResults(string data)
    {
        //updates results in resultsScene

        Debug.Log("[+] Called UpdateScoreResults " + data.ToString());
        PopulateCurrentScores(data);

        //TODO: show effects of result in scorepanel (finace +5 etc)


    }
    public void PopulateCurrentScores(string data)
    {
        //Expects a json array as data consisting of {"human":60},{"manufactured":35}, etc
        Debug.Log("[+] Called populate cur score capitals");
        JArray capitals = JArray.Parse(data);
        Debug.Log("[+] Parsed json array " + capitals.ToString());


        //Goes through children of scorepanel, fills each current score text element with received capital value
        for (int i = 0; i < capitals.Count; i++)
        {
            Debug.Log("[+] TEST ");
            foreach (JObject value in capitals)
            {
                Transform capitalScoreText = GameObject.Find("ScoresPanel").transform.GetChild(i).transform.GetChild(1);
                JObject scoreValue = (JObject)capitals[i];
                foreach (var property in scoreValue.Properties())
                {

                    capitalScoreText.GetComponent<TextMeshProUGUI>().text = property.Value.ToString();
                    Debug.Log("[+] Populate capital " + property.Name.ToString() + " with score: " + property.Value);
                }
                break;


            }
        }
    }

}

