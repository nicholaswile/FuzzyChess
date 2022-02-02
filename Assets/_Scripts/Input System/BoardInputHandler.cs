using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChessBoard))]
public class BoardInputHandler : MonoBehaviour, InputHandler
{
    private ChessBoard board;

    private void Awake()
    {
        board = GetComponent<ChessBoard>();
    }
    public void ProcessInput(Vector3 inputPosition, GameObject selectedObject, Action callback)
    {
        board.OnSquareSelected(inputPosition);
    }
}
