using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulatePlayerPanel : MonoBehaviour
{
    public GameObject playerPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Populate(int playerCount)
    {
        Debug.Log("Called player populate with " + playerCount + " players");
        
        //Clear players to fill with updated room count
        GameObject playerPanel = GameObject.Find("PlayerIconsPanel");
        foreach (Transform child in playerPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        

        //add new player icon prefab
        GameObject newObj;
        for (int i = 0; i < playerCount; i++)
        {
            newObj = (GameObject)Instantiate(playerPrefab, transform);
            Debug.Log(newObj);

            //TODO: change to different character
            
            
        }
    }
}
