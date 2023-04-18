using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuUI : MonoBehaviour
{
    public int MaximumLevels = 3;
    public Text BotsQuantityText;
    public int BotsQuantity;
    public int SelectedLevel=1;
    public Text SelectedLevelText;
    public void IncreaseBotsQuantityText()
    {
        BotsQuantity++;
        if (BotsQuantity > 5)
            BotsQuantity = 5;
        PlayerPrefs.SetInt("BOTSQUANTITY", BotsQuantity);

        BotsQuantityText.text=BotsQuantity.ToString();
    }
    public void DecreaseBotsQuantityText()
    {
        BotsQuantity--;
        if (BotsQuantity < 0)
            BotsQuantity = 0;

        PlayerPrefs.SetInt("BOTSQUANTITY", BotsQuantity);
        BotsQuantityText.text = BotsQuantity.ToString();
    }
    
    public void IncreaseLevelText()
    {
        SelectedLevel++;
        if (SelectedLevel > MaximumLevels)
            SelectedLevel = MaximumLevels;
     
        SelectedLevelText.text = SelectedLevel.ToString();
    }
    public void DecreaseLevelText()
    {
        SelectedLevel--;
        if (SelectedLevel < 1)
            SelectedLevel = 1;

        SelectedLevelText.text = SelectedLevel.ToString();
    }
    private void Start()
    {
        BotsQuantity = PlayerPrefs.GetInt("BOTSQUANTITY",5);
        BotsQuantityText.text = BotsQuantity.ToString();
    }
    public void SetBotDifficulty(int BotDifficulty)
    {
        PlayerPrefs.SetInt("BOTDIFFICULTY", BotDifficulty);
    }
    public void SetEnemyDifficulty(int Enemydifficulty)
    {
        PlayerPrefs.SetInt("ENEMYDIFFICULTY", Enemydifficulty);
        
    }
    public void LoadScene()
    {
        PlayerPrefs.SetInt("LEVEL", SelectedLevel);
        SceneManager.LoadScene("GamePlay");

    }
}
