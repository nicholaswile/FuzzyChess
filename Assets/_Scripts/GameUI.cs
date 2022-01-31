using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject mainGameUI, captureTable, cam2d, cam3d, winScreen, loseScreen, rollScreen, movesList;
    [SerializeField] private Button exitButton, skipButton, moveButton, camButton, rollButton;

    private Vector3[,] camSwitch = new Vector3[2, 2];

    private void Awake()
    {
        mainGameUI.SetActive(true);
        captureTable.SetActive(false);
        movesList.SetActive(false); 

        GameManager.StateChanged += GameManager_StateChanged;
    }

    private void OnDestroy()
    {
        GameManager.StateChanged -= GameManager_StateChanged;
    }

    private void GameManager_StateChanged(GameState state)
    {
        // Can only skip on player turn
        exitButton.interactable = (state == GameState.PlayerTurn);
        skipButton.interactable = (state == GameState.PlayerTurn);
        moveButton.interactable = (state == GameState.PlayerTurn);
        camButton.interactable = (state == GameState.PlayerTurn);
        rollButton.interactable = (state == GameState.PlayerTurn);

        winScreen.SetActive(state == GameState.Win);
        loseScreen.SetActive(state == GameState.Lose);
        rollScreen.SetActive(state == GameState.Rolling);
    }

    public void UI_CaptureTable()
    {
        bool open = captureTable.activeSelf;
        captureTable.SetActive(!open);
        exitButton.interactable = open;
        skipButton.interactable = open;
        moveButton.interactable = open;
        camButton.interactable = open;
    }

    public void UI_Exit()
    {
        Debug.Log("Exit");

        // Code to handle what happens before the player can exit

        GameManager.Instance.UpdateGameState(GameState.MainMenu);
        SceneManager.LoadScene(0);
        // Quits to soft start / main menu
    }

    public void UI_Skip()
    {
        Debug.Log("Skip");

        // Code to handle what happens before the player can skip

        GameManager.Instance.UpdateGameState(GameState.EnemyTurn);
    }

    public void UI_Move()
    {
        Debug.Log("Moves");
        movesList.SetActive(!movesList.activeSelf);

        // Shows moves transcript
    }

    // Changes camera rotation angle and position
    public void UI_Cam()
    {
        // Switch from 2d / top down view to 3d / POV 
        if (cam2d.activeSelf)
        {
            cam3d.SetActive(true);
            cam2d.SetActive(false);
            Debug.Log("Switched to 3D Cam");
        }
        // Vice versa
        else
        {
            cam2d.SetActive(true);
            cam3d.SetActive(false);
            Debug.Log("Switched to 2D Cam");
        }
    }
}
