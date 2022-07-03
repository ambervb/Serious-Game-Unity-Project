using System;
using System.Collections;
using System.Collections.Generic;
using KyleDulce.SocketIo;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DiceRollerController : MonoBehaviour
{
    private Sprite[] sides;
    private SpriteRenderer renderer;
    public Button btn;
    public Sprite DiceOne;
    public Sprite DiceTwo;
    public Sprite DiceThree;
    public Sprite DiceFour;
    public Sprite DiceFive;
    public Sprite DiceSix;
    public float DiceRollSpeed = 0.2f;
    private int diceResult = -1;
    private int questionsLeft = 0;

    private Socket _socket;

  // Start is called before the first frame update
  
  public void OnEnable()
  {
    SceneManager.sceneLoaded += OnSceneLoaded;
  }

  void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    var client = GlobalState.client;
    this._socket = client.socket;
    
    renderer = GetComponent<SpriteRenderer>();

    sides = new Sprite[6];

    //Load the sprites array
    sides[0] = DiceOne;
    sides[1] = DiceTwo;
    sides[2] = DiceThree;
    sides[3] = DiceFour;
    sides[4] = DiceFive;
    sides[5] = DiceSix;

    //Set default
    renderer.sprite = sides[5];

    this._socket.on("receiveDice", receiveDiceCallBack);
  }
  
  void OnDisable()
  {
    this._socket.off("receiveDice");
    SceneManager.sceneLoaded -= OnSceneLoaded;
  }

  public void receiveDiceCallBack(String data)   
  {
    Debug.Log("[+] receive dice got");
        /* TODO: zorg dat het nummer dat hier in data zit de kant is waarop de dice land
         zorg dat de dice eerst even draait want het event komt super snel binnen dus met een tim
         eout ofzo. als de dice dan op een kant land create een knop die naar questionScene navigeert*/

        // after this button to load next question

        //add
        //get dice roll and questionsleft from data
        JObject json = JObject.Parse(data.ToString());
        string jsonDice = json["rolledDice"].ToString();
        this.diceResult = Int32.Parse(jsonDice);
        Debug.Log("ReceiveDiceCallback parsed dice num is " + this.diceResult);

        string jsonQuestionsLeft = json["questionsLeft"].ToString();
        //this.questionsLeft = Int32.Parse(jsonQuestionsLeft);
        this.questionsLeft = 3;
        Debug.Log("ReceiveDiceCallback parsed questionsLeft num is " + this.questionsLeft);


    }
  
  public void TaskOnClick()
  {
    StartCoroutine("RollTheDice");
  }


  private IEnumerator RollTheDice()
  {
    
    int RandomSide = 6;
    while (diceResult == -1)
    {
      for (int i = 0; i < 6; i++)
      {
        RandomSide = Random.Range(0, 6);
        btn.image.sprite = sides[RandomSide];
        Debug.Log(RandomSide);
        yield return new WaitForSeconds(DiceRollSpeed);
      }
        diceResult = RandomSide + 1;
       
    }

        //add
        if (diceResult != -1)
        {
            for (int i = 0; i < 6; i++)
            {
                RandomSide = UnityEngine.Random.Range(0, 6);
                btn.image.sprite = sides[RandomSide];
                yield return new WaitForSeconds(DiceRollSpeed);
                if (i == 5)
                {
                    btn.image.sprite = sides[diceResult];
                    yield return new WaitForSeconds(3.0F);
                    if (questionsLeft == 0)
                    {
                        //this._socket.emit("answeredFinalQuestion", "");
                        SceneManager.LoadScene("FinalResultsScene");
                    }
                    else
                    {
                        
                        SceneManager.LoadScene("QuestionScene");
                    }

                }
            }
            
        }

    }
}
