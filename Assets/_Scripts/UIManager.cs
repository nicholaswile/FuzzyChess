using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * This script isn't really doing anything right now; might come back and do stuff with it later
 */

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainScreen;

    private void Awake()
    {
        GameManager.StateChanged += GameManager_StateChanged;
    }

    private void OnDestroy()
    {
        GameManager.StateChanged -= GameManager_StateChanged;
    }

    private void GameManager_StateChanged(GameState state)
    {
        mainScreen.SetActive(state == GameState.MainMenu);
    }
}
