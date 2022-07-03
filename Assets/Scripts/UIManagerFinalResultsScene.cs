using System.Collections;
using System.Collections.Generic;
using KyleDulce.SocketIo;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerFinalResultsScene : MonoBehaviour
{
    public GameObject secretCapitalButtonVisible;
    public GameObject secretCapitalButtonHidden;

    private Socket _socket;
    private string winningPlayer;
    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[+++] OnSceneLoaded FinalResultScene");
        var client = GlobalState.client;
        this._socket = client.socket;

        //emit calculatewinner and set listener
        this._socket.emit("calculateWinnner", "");
        this._socket.on("receiveWinner", showFinalWinnerCall);

        //get capitals scores
        this._socket.emit("getCurScores", "");
        this._socket.on("receiveScores", receiveCurScoresCall);
    }

    void OnDisable()
    {
        this._socket.off("showFinalWinnerCall");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showFinalWinnerCall(string data)
    {
        Debug.Log("[+] showFinalCall called");

        this.winningPlayer = data;
        Debug.Log("[+] winning player data: " + data);
        ShowWinningPlayer();

    }

    public void NewGameOnClick()
    {

        SceneManager.LoadScene("MenuScene");
    }
    public void ShowSecretCapitalOnClick()
    {
        secretCapitalButtonVisible.SetActive(secretCapitalButtonHidden.active);
        secretCapitalButtonHidden.SetActive(!secretCapitalButtonVisible.active);
    }

    void ShowWinningPlayer()
    {
        //TODO: show player icon of winning player through adding capital 
        GameObject.Find("WinningPlayerText").GetComponent<TextMeshProUGUI>().text = this.winningPlayer;
    }

    //show current scores
    public void receiveCurScoresCall(string data)
    {
        Debug.Log("[+++] receiveCurScores called");
        Debug.Log("[+++] receiveCurScores called with data: " + data);
        //populate scorepanel
        GameObject scoresPanel = GameObject.Find("ScoresPanel");
        PopulateCapitalScores scoresPanelScript = scoresPanel.GetComponent<PopulateCapitalScores>();

        scoresPanelScript.PopulateCurrentScores(data);

    }
}
