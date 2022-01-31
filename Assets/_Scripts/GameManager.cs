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

    // Used to notify scripts who subscribed that state changed
    public static event Action<GameState> StateChanged;

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
            case GameState.Rolling:
                HandleRolling();
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

    private void HandleRolling()
    {
        Debug.Log("Rolling");
    }

    private void HandleLose()
    {
        Debug.Log("Lose");
    }

    private void HandleWin()
    {
        Debug.Log("Win");
    }

    private void HandleEnemyTurn()
    {
        Debug.Log("Enemy Turn");
        StartCoroutine(IEnemyTurn());
    }

    private IEnumerator IEnemyTurn()
    {
        // Put Enemy turn info here
        yield return new WaitForSeconds(3);
        Debug.Log("Enemy turn over");
        Instance.UpdateGameState(GameState.PlayerTurn);
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
    Rolling,
    Win,
    Lose
}
