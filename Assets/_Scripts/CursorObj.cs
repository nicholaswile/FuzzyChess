using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorObj : MonoBehaviour
{
    [SerializeField] private CursorManager.CursorType cursorType;
    private GameController controller;
    private ChessBoard board;

    private void Awake()
    {
        board = GameObject.Find("Chess Board").GetComponent<ChessBoard>();
        controller = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    private void Update()
    {
        if (board.selectedPieceMoved == true) 
        {
            CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Default);
            board.selectedPieceMoved = false;
        }
    }

    private void OnMouseOver()
    {
        if (board.isSelectable(gameObject.GetComponent<Piece>()) && controller.activePlayer == controller.whitePlayer && !Input.GetMouseButton(1))
            CursorManager.Instance.SetActiveCursorType(cursorType);
        else if (gameObject.name == "Highlighter(Clone)" && controller.activePlayer == controller.whitePlayer && !Input.GetMouseButton(1))
            CursorManager.Instance.SetActiveCursorType(cursorType);
        else if (!board.isSelectable(gameObject.GetComponent<Piece>()) && controller.activePlayer == controller.whitePlayer && !Input.GetMouseButton(1))
            CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Unavailable);
    }

    private void OnMouseExit()
    {
        CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Default);
    }
}

