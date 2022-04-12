using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject softScreen,mainMenuScreen, playScreen, statsScreen, rulesScreen, creditsScreen;
    [SerializeField] private TextMeshProUGUI winText, loseText;
    [SerializeField] private Button aiVsAiButton;

    private const string WINS = "Wins", LOSS = "Losses";

    private void Awake()
    {
        softScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
        playScreen.SetActive(false);
        statsScreen.SetActive(false);
        rulesScreen.SetActive(false);
        creditsScreen.SetActive(false);

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
        playScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
        UI_ButtonSound();
    }

    public void UI_HumanMode()
    {
        Debug.Log("Human vs AI Play Mode");
        // Start game in Human Mode
        GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
        SceneManager.LoadScene(1);
    }

    public void UI_AIMode()
    {
        Debug.Log("AI vs AI Play Mode");
        // Start game in AI Mode
    }

    public void UI_Back(GameObject currentScreen)
    {
        mainMenuScreen.SetActive(true);
        currentScreen.SetActive(false);
        UI_ButtonSound();
    }

    public void UI_StatsMode()
    {
        statsScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
        UI_ButtonSound();
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
    }

    public void UI_RulesMode()
    {
        rulesScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
        UI_ButtonSound();
    }
    
    public void UI_CreditsMode()
    {
        creditsScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
        UI_ButtonSound();
    }

    public void UI_Exit()
    {
        Debug.Log("Quitting Application");
        Application.Quit();
    }

    private void UI_ButtonSound()
    {
        GameObject soundObject = new GameObject("SFX Object");

        AudioClip clipMenuButton = Resources.Load<AudioClip> ("Audio/MenuButton");

        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(clipMenuButton);
    }
}
