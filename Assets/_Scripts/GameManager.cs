using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to manage state of game

public class GameManager : MonoBehaviour
{
    // Singleton pattern used to refernce game manager from anywhere in our game
    public static GameManager Instance;

    public GameState State;

    public RollState RollState;

    private const string WINS = "Wins", LOSS = "Losses";

    // Used to notify scripts who subscribed that state changed
    public static event Action<GameState> StateChanged;
    public static event Action<RollState> RollStateChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        UpdateGameState(GameState.MainMenu);
        UpdateRollState(RollState.FalseRoll);
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                HandleMainMenu();
                break;
            case GameState.PlayerTurn:
                HandlePlayerTurn();
                break;
            case GameState.EnemyTurn:
                HandleEnemyTurn();
                break;
            case GameState.Win:
                HandleWin();
                break;
            case GameState.Lose:
                HandleLose();
                break;
            default:
                throw new System.Exception("Argument out of range");
        }

        // If there are subscribers, event trigger is called to notify them of state change
        StateChanged?.Invoke(newState);
    }

    public void UpdateRollState(RollState rollState)
    {
        RollState = rollState;

        switch(rollState)
        {
            case RollState.TrueRoll:
                HandleTrueRoll();
                break;

            case RollState.FalseRoll:
                HandleFalseRoll();
                break;
            default:
                throw new System.Exception("Argument out of range");
        }
        RollStateChanged?.Invoke(rollState);
    }

    void LateUpdate()
    {
        if(GameObject.Find("Chess Set") != null)
        {
            GameUI TheGameUI = GameObject.Find("UI").GetComponent<GameUI>();
            TheGameUI.ScrollToBottom();
        }
    }

    private void HandleFalseRoll()
    {
        Debug.Log("Stop Rolling");
    }

    private void HandleTrueRoll()
    {
        Debug.Log("Rolling");
    }

    private void HandleLose()
    {
        Debug.Log("Lose");
        if (PlayerPrefs.HasKey(LOSS))
        {
            string s = PlayerPrefs.GetString(LOSS);
            int i = Convert.ToInt32(s);
            i++;
            PlayerPrefs.SetString(LOSS, Convert.ToString(i));
        }
        else
        {
            PlayerPrefs.SetString(LOSS, "1");
        }
    }

    private void HandleWin()
    {
        Debug.Log("Win");
        if (PlayerPrefs.HasKey(WINS))
        {
            string s = PlayerPrefs.GetString(WINS);
            int i = Convert.ToInt32(s);
            i++;
            PlayerPrefs.SetString(WINS, Convert.ToString(i));
        }
        else
        {
            PlayerPrefs.SetString(WINS, "1");
        }
    }

    private void HandleEnemyTurn()
    {
        Debug.Log("Enemy Turn");
    }

    private void HandlePlayerTurn()
    {
        Debug.Log("Player Turn");
    }

    private void HandleMainMenu()
    {
        Debug.Log("Menu");
    }
}

public enum GameState
{
    MainMenu,
    PlayerTurn,
    EnemyTurn,
    Win,
    Lose
}

public enum RollState
{
    TrueRoll,
    FalseRoll
}

