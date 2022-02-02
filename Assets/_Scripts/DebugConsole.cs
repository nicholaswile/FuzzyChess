using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugConsole : MonoBehaviour
{
    private bool showConsole;
    private string input;

    private const string WINS = "Wins", LOSS = "Losses";

    public static DebugConsole debugger;

    private void Awake()
    {
        if (debugger == null)
        {
            debugger = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (debugger != null)
        {
            Destroy(debugger);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape");
            showConsole = !showConsole;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Enter");
            if (showConsole)
            {
                Debug.Log("Made it");
                HandleInput();
                input = "";
            }
        }
    }

    private void OnGUI()
    {
        if (!showConsole) { return; }

        float y = 0f;

        GUI.Box(new Rect(0, y, Screen.width, 60), " ");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        GUI.skin.textField.fontSize = 30;
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 40f), input);
    }

    private void HandleInput()
    {
        Debug.Log(input);
        string[] parse = input.Split(' ');
        switch (parse[0])
        {
            case ("win"):
                GameManager.Instance.UpdateGameState(GameState.Win);
                break;
            case ("lose"):
                GameManager.Instance.UpdateGameState(GameState.Lose);
                break;
            case ("roll"):
                GameManager.Instance.UpdateGameState(GameState.Rolling);
                break;
/* These commands are commented out because they break the turn controller 
            case ("player"):
                GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
                break;
            case ("enemy"):
                GameManager.Instance.UpdateGameState(GameState.EnemyTurn);
                break;
*/
            case ("w_score"):
                PlayerPrefs.SetString(WINS, parse[1]);
                break;
            case ("l_score"):
                PlayerPrefs.SetString (LOSS, parse[1]);
                break;
            case ("reload"):
                SceneManager.LoadScene(0);
                break;
            case ("help"):
                PrintCommands();
                break;
            default:
                break;
        }

    }

    private void PrintCommands()
    {
        foreach (string s in debugCommands)
        {
            Debug.Log(s);
        }
    }

    private string[] debugCommands =
    {
        "win",
        "lose",
        "roll",
        //"player",
        //"enemy",
        "w_score",
        "l_score",
        "reload",
        "help",
    };
}

