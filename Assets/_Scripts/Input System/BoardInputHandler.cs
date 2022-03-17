using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChessBoard))]
public class BoardInputHandler : MonoBehaviour, InputHandler
{
    private ChessBoard board;
    private GameController controller;

    private void Awake()
    {
        board = GetComponent<ChessBoard>();
        controller = GameObject.Find("Game Controller").GetComponent<GameController>();
    }
    public void ProcessInput(Vector3 inputPosition, GameObject selectedObject, Action callback)
    {
        if (controller.activePlayer == controller.whitePlayer)
            board.OnSquareSelected(inputPosition);
    }
}
