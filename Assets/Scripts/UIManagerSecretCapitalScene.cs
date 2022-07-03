using System.Collections;
using System.Collections.Generic;
using KyleDulce.SocketIo;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerSecretCapitalScene : MonoBehaviour
{
    private Socket _socket;

    // Start is called before the first frame update
    void Start()
    {
        var client = GlobalState.client;
        this._socket = client.socket;
        this._socket.on("receiveCapitals", (string data) =>
        {  
            Debug.Log("Received capitals receiveCapitals3: " + data);
            // Get TextMeshPro element called SecretCapitalText and set it's text to * + data + *
            GameObject.Find("SecretCapitalText")
                .GetComponent<TextMeshProUGUI>()
                .text = "* " + data + " *";
        });
        
        // Select the start button
        GameObject startGameButton = GameObject.Find("StartGameButton");
        // Get the button called StartGameButton
        if (PlayerPrefs.GetInt("fromCreateGame") == 0)
        {
            // hide the start game button
            startGameButton.SetActive(false);
        }
    }

    public void NextScene()
    {
        // this._socket.emit("getNextScene", "QuestionScene");
        SceneManager.LoadScene("QuestionScene");
    }
}
