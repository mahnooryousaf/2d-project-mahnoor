using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum Items
{
    Key,Switch,System,Car
}

public class GameManager : MonoBehaviour
{
    public int DistanceForEnemyToShoot;

    public static GameManager instance;
    public GameObject ObjectInView;
    public List<Items> CollectedItems = new List<Items>();
    public bool CanPickUpItem = false;
    public GameObject ObjectivePanel;
    public GameObject[] ObjectiveTickMark;
    public Text[] Objectives;
    public int ObjectiveCleared = 0;
    public float HealthOfPlayer;

    public Image PlayerHealthImg;
    
    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ObjectivePanel.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            ObjectivePanel.SetActive(false);
        }

    }
    public void DecrementHealth()
    {
        HealthOfPlayer -= 0.5f;
        PlayerHealthImg.fillAmount = HealthOfPlayer / 100;

        if(HealthOfPlayer<=0 && Manager.instance.CurrentGameState==Manager.GameState.Running) 
        {
            Manager.instance.CurrentGameState = Manager.GameState.PlayerHasDied;
            PlayerController.instance.PlayerDied();
            UIManager.instance.LevelFailed();
        }
    }
    public bool IsItemContained(Items requiredItem)
    {
        return CollectedItems.Contains(requiredItem);
           
    }
    public void OpenDoor()
    {
        bool status=ObjectInView.GetComponent<Door>().CheckLockStatus();
        if (status == false)
        {
            UIManager.instance.ShowToast("FIND KEY FIRST");
        }
        else
        {
            ObjectInView.GetComponent<Door>().OpenDoor();
        }
    }
    private int switches = 0;
    public void ItemPicked()
    {
        if(ObjectInView.GetComponent<Item>().ItemType==Items.Switch)
        {
            CanPickUpItem = false;
            UIManager.instance.TooglePrompt(false);
            CollectedItems.Add(ObjectInView.GetComponent<Item>().ItemType);

            ObjectiveTickMark[(int)ObjectInView.GetComponent<Item>().ItemType].gameObject.SetActive(true);

            SoundManager.instance.ObjectiveCompleteSound.Play();

            switches++;
            if(switches==3)
            {
                ObjectiveCleared++;
                UIManager.instance.ShowToast(" Objective Completed: Generator Turned Off!");
                Objectives[(int)ObjectInView.GetComponent<Item>().ItemType].color = Color.green;
            }
        else
                UIManager.instance.ShowToast("Generator Turned Off: "+switches+"/3");

            ObjectInView.GetComponent<Item>().TurnAnims(false);
            ObjectInView.GetComponent<Item>().TurnOffAudios();

      
            ObjectInView.transform.GetChild(0).gameObject.SetActive(true);
            ObjectInView.transform.GetChild(1).gameObject.SetActive(false);
            ObjectInView.transform.GetComponent<BoxCollider>().enabled = false;

        }
        if (ObjectInView.GetComponent<Item>().ItemType == Items.Key)
        {
            ObjectiveCleared++;
               CanPickUpItem = false;
            UIManager.instance.TooglePrompt(false);
            CollectedItems.Add(ObjectInView.GetComponent<Item>().ItemType);

            ObjectiveTickMark[(int)ObjectInView.GetComponent<Item>().ItemType].gameObject.SetActive(true);
            Objectives[(int)ObjectInView.GetComponent<Item>().ItemType].color = Color.green;

            UIManager.instance.ShowToast(ObjectInView.GetComponent<Item>().ItemType.ToString() + " Picked Up!");
            Destroy(ObjectInView);
            Debug.Log("ITEM PICKED");
            SoundManager.instance.ObjectiveCompleteSound.Play();

        }
        if (ObjectInView.GetComponent<Item>().ItemType == Items.System)
        {
            ObjectiveCleared++;
               CanPickUpItem = false;
            UIManager.instance.TooglePrompt(false);
            CollectedItems.Add(ObjectInView.GetComponent<Item>().ItemType);

            ObjectiveTickMark[(int)ObjectInView.GetComponent<Item>().ItemType].gameObject.SetActive(true);

          
            Objectives[(int)ObjectInView.GetComponent<Item>().ItemType].color = Color.green;

            ObjectInView.GetComponent<Item>().TurnAnims(false);
            ObjectInView.GetComponent<Item>().TurnOffAudios();
            SoundManager.instance.ObjectiveCompleteSound.Play();

            UIManager.instance.ShowToast("Objective Completed:System Turned Off!");

            ObjectInView.transform.tag = "Untagged";
          
        }
        if (ObjectInView.GetComponent<Item>().ItemType == Items.Car)
        {
            if(ObjectiveCleared!=3)
            {
                UIManager.instance.ShowToast("Complete All Objectives To Escape!");
            }
            else
            {
                ObjectInView.transform.tag = "Untagged";
                UIManager.instance.LevelCompleted();
            }
          //  CanPickUpItem = false;
            UIManager.instance.TooglePrompt(false);
            CollectedItems.Add(ObjectInView.GetComponent<Item>().ItemType);
            

      



        }
    }
    public void SetObjectInViewStatus(GameObject g,bool Status,string DialogueboxText="")
    {
        ObjectInView = g;
        UIManager.instance.TooglePrompt(Status, DialogueboxText);
        CanPickUpItem = Status;
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Home()
    {
        SceneManager.LoadScene("Main");
    }
}
