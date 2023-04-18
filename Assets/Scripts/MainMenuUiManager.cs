using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuUiManager : MonoBehaviour
{
    public GameObject Loading;
    void Start()
    {
        Cursor.visible = true;
        
    }
    public void LoadGame()
    {
        Loading.SetActive(true);
        SceneManager.LoadScene("Game");
    }
    public void QuitTheGame()
    {
        Application.Quit();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
