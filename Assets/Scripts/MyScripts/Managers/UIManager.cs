using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI ToastText;
    public GameObject LevelCOmpletePanel;
    public GameObject LevelFailedPanel;
    public GameObject GamePaused;

    public bool ispaused = false;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public GameObject PromptBox;
    public Text Prompt;

    public void LevelCompleted()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Manager.instance.CurrentGameState = Manager.GameState.PlayerHasWon_Running;
        LevelCOmpletePanel.SetActive(true);
    }
    public void LevelFailed()

    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible =true;
        Manager.instance.CurrentGameState = Manager.GameState.PlayerHasDied;
        Invoke("DelayLevelFailed", 5f);
    }
    public void DelayLevelFailed()
    {
        LevelFailedPanel.SetActive(true);
    }
    public void TooglePrompt(bool status,string prompttext="")
    {
        PromptBox.SetActive(status);
        Prompt.text = prompttext;
    }
    void Start()
    {
        
    }

    public IEnumerator ToastCoRoutine(string Toast)
    {
        ToastText.text = Toast;
        ToastText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        ToastText.gameObject.SetActive(false);
    }
   public void ShowToast(string Message)
    {
        StartCoroutine(ToastCoRoutine(Message));
    }
    public void ResumeGame()
    {
        ispaused = false;
        GamePaused.SetActive(false);
        Cursor.visible = false;
        Time.timeScale = 1;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(ispaused)
            {
                ispaused = false;
                GamePaused.SetActive(false);
                Cursor.visible = false;
                Time.timeScale = 1;
            }
            else
            {
                ispaused = true;
                GamePaused.SetActive(true);
                Cursor.visible = true;
                Time.timeScale = 0;
            }
        }
    }
}
