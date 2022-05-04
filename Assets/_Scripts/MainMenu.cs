using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject softScreen,mainMenuScreen, playScreen, statsScreen, settingsScreen, rulesScreen, creditsScreen, mapScreen;
    [SerializeField] private TextMeshProUGUI winText, loseText;
    [SerializeField] private Button aiVsAiButton;
    public MenuInfo mapChoice;

    private const string WINS = "Wins", LOSS = "Losses";
    private GameObject greyscale;
    private GameObject tanscale;

    private void Awake()
    {
        softScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
        playScreen.SetActive(false);
        statsScreen.SetActive(false);
        rulesScreen.SetActive(false);
        creditsScreen.SetActive(false);
        settingsScreen.SetActive(false);

        // Will comment the following out once ai vs ai mode is complete
        aiVsAiButton.interactable = false;

        // Text = saved win score
        SetStatText();

        // And wherever wins / losses are handled (probably Game Manager), update the value
    }

    private void SetStatText()
    {
        if (PlayerPrefs.HasKey(WINS))
        {
            winText.text = PlayerPrefs.GetString(WINS);
        }
        else
        {
            winText.text = "000";
        }

        if (PlayerPrefs.HasKey(LOSS))
        {
            loseText.text = PlayerPrefs.GetString(LOSS);
        }
        else
        {
            loseText.text = "000";
        }
    }

    // Update is called once per frame
    void Update()
    { 
        if (Input.anyKey && softScreen.activeSelf)
        {
            softScreen.SetActive(false);
            mainMenuScreen.SetActive(true);
        }
        if (statsScreen.activeSelf)
        {
            SetStatText();
        }
    }
    public void UI_PlayMode()
    {
        mapScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
        SFXController.PlaySoundMenuButton();
    }

    public void UI_HumanMode()
    {
        Debug.Log("Player vs AI Play Mode");
        mapChoice.modeNumber = 1;
        // Start game in Human Mode
        GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
        SceneManager.LoadScene(1);
        SFXController.PlaySoundMenuButton();
    }

    public void UI_PvPMode()
    {
        Debug.Log("Player vs Player Play Mode");
        mapChoice.modeNumber = 2;
        // Start game in PvP Mode
        GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
        SceneManager.LoadScene(1);
        SFXController.PlaySoundMenuButton();
    }

    public void UI_AIMode()
    {
        Debug.Log("AI vs AI Play Mode");
        mapChoice.modeNumber = 3;
        // Start game in AI Mode
        GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
        SceneManager.LoadScene(1);
        SFXController.PlaySoundMenuButton();
    }

    public void UI_Back(GameObject currentScreen)
    {
        mainMenuScreen.SetActive(true);
        currentScreen.SetActive(false);
        SFXController.PlaySoundMenuButton();
    }

    public void UI_Back_PlayScreen()
    {
        mapScreen.SetActive(true);
        playScreen.SetActive(false);
        SFXController.PlaySoundMenuButton();
    }

    public void UI_Clouds()
    {
        mapChoice.mapNumber = 1;
        playScreen.SetActive(true);
        mapScreen.SetActive(false);
        SFXController.PlaySoundMenuButton();
    }
    public void UI_Galaxy()
    {
        mapChoice.mapNumber = 2;
        playScreen.SetActive(true);
        mapScreen.SetActive(false);
        SFXController.PlaySoundMenuButton();
    }
    public void UI_Mars()
    {
        mapChoice.mapNumber = 3;
        playScreen.SetActive(true);
        mapScreen.SetActive(false);
        SFXController.PlaySoundMenuButton();
    }

    public void UI_StatsMode()
    {
        statsScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
        SFXController.PlaySoundMenuButton();
    }

    public void UI_ResetScore()
    {
        Debug.Log("Reset Player Score");
        // Reset Player Score

        // Reset wins
        // Reset losses
        winText.text = "000";
        loseText.text = "000";

        PlayerPrefs.SetString(WINS, "000");
        PlayerPrefs.SetString(LOSS, "000");
        SFXController.PlaySoundMenuButton();
    }

    public void UI_SettingsMode()
    {
        settingsScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
        SFXController.PlaySoundMenuButton();

        //display AI style setting
        switch (PlayerPrefs.GetInt("AIStyle"))
        {
            case 1:
                tanscale = GameObject.Find("Aggressive Button");
                tanscale.GetComponent<Image>().color = new Color32(244, 231, 192, 255);
                break;
            case 2:
                tanscale = GameObject.Find("Defensive Button");
                tanscale.GetComponent<Image>().color = new Color32(244, 231, 192, 255);
                break;
            default:
                tanscale = GameObject.Find("Balanced Button");
                tanscale.GetComponent<Image>().color = new Color32(244, 231, 192, 255);
                break;
        }
    }

    public void UI_AgressiveAI()
    {
        PlayerPrefs.SetInt("AIStyle", 1);
        AI_Style_Recolor();
        tanscale = GameObject.Find("Aggressive Button");
        tanscale.GetComponent<Image>().color = new Color32(244, 231, 192, 255);
        Debug.Log("AI style set to attack more often.");
        SFXController.PlaySoundMenuButton();
    }

    public void UI_DefensiveAI()
    {
        PlayerPrefs.SetInt("AIStyle", 2);
        AI_Style_Recolor();
        tanscale = GameObject.Find("Defensive Button");
        tanscale.GetComponent<Image>().color = new Color32(244, 231, 192, 255);
        Debug.Log("AI style set to protect itself.");
        SFXController.PlaySoundMenuButton();
    }

    public void UI_BalancedAI()
    {
        PlayerPrefs.SetInt("AIStyle", 0);
        AI_Style_Recolor();
        tanscale = GameObject.Find("Balanced Button");
        tanscale.GetComponent<Image>().color = new Color32(244, 231, 192, 255);
        Debug.Log("AI style set to balanced.");
        SFXController.PlaySoundMenuButton();
    }

    public void UI_Animations()
    {
        SFXController.PlaySoundMenuButton();
    }

    public void UI_CameraSpin()
    {
        SFXController.PlaySoundMenuButton();
    }

    public void UI_RulesMode()
    {
        rulesScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
        SFXController.PlaySoundMenuButton();
    }
    
    public void UI_CreditsMode()
    {
        creditsScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
        SFXController.PlaySoundMenuButton();
    }

    public void UI_Exit()
    {
        Debug.Log("Quitting Application");
        Application.Quit();
    }

    public void AI_Style_Recolor()
    {
        greyscale = GameObject.Find("Balanced Button");
        greyscale.GetComponent<Image>().color = new Color32(244, 231, 192, 100);
        greyscale = GameObject.Find("Aggressive Button");
        greyscale.GetComponent<Image>().color = new Color32(244, 231, 192, 100);
        greyscale = GameObject.Find("Defensive Button");
        greyscale.GetComponent<Image>().color = new Color32(244, 231, 192, 100);
    }
}
