using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject softScreen,mainMenuScreen, playScreen, statsScreen, rulesScreen, creditsScreen;

    private void Awake()
    {
        softScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
        playScreen.SetActive(false);
        statsScreen.SetActive(false);
        rulesScreen.SetActive(false);
        creditsScreen.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    { 
        if (Input.anyKey && softScreen.activeSelf)
        {
            softScreen.SetActive(false);
            mainMenuScreen.SetActive(true);
        }
    }
    public void UI_PlayMode()
    {
        playScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
    }

    public void UI_HumanMode()
    {
        Debug.Log("Human vs AI Play Mode");
        // Start game in Human Mode
        GameManager.Instance.UpdateGameState(GameState.PlayerTurn);

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
    }


    public void UI_StatsMode()
    {
        statsScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
    }

    public void UI_ResetScore()
    {
        Debug.Log("Reset Player Score");
        // Reset Player Score
    }

    public void UI_RulesMode()
    {
        rulesScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
    }
    
    public void UI_CreditsMode()
    {
        creditsScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
    }

    public void UI_Exit()
    {
        Debug.Log("Quitting Application");
        Application.Quit();
    }
}
