using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;
    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
    }
    public Text AiPlayerText;
    public Text EnemyStateText;
    public StateMachine Player;
    public StateMachine Enemy;

    public Text CurrentPlayerTask;


    public GameObject LevelCompletePanel;
    public GameObject LevelFailedPanel;
    public GameObject PlayerDiedPanel;
    public GameObject TimerTextObj;
    public Text TimerText;
    public int Timer = 0;

    public void StartTimer()
    {
        Timer = 3;
        StartCoroutine(TimerCountDown());
    }
    IEnumerator TimerCountDown()
    {
        while(Timer>-1)
        {
            TimerText.text = Timer.ToString();
             yield return new WaitForSeconds(1);
            Timer--;
        }
        TimerText.gameObject.SetActive(false);

        TimerTextObj.SetActive(false);
    }
    public void SetPlayerText(string task)
    {
        CurrentPlayerTask.text =task;
    }
    private void Update()
    {
        //AiPlayerText.text = "AI:" + Player.Attributes.State.ToString()+",Delay:"+ (int)Player.Attributes.TaskDelay;
        EnemyStateText.text= "Enemy:"+ Enemy.Attributes.State.ToString();
    }

    public void Next()
    {
        if(PlayerPrefs.GetInt("LEVEL", 1)!=3)
        {
            PlayerPrefs.SetInt("LEVEL", PlayerPrefs.GetInt("LEVEL", 1) + 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }    
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Home()
    {
        SceneManager.LoadScene("MainMenu");

    }
}
