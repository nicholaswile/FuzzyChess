using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorObj : MonoBehaviour
{
    [SerializeField] private CursorManager.CursorType cursorType;
    private GameController controller;
    private ChessBoard board;
    private MenuInfo menuInfo;
    private int modeChoice;

    private void Awake()
    {
        board = GameObject.Find("Chess Board").GetComponent<ChessBoard>();
        controller = GameObject.Find("Game Controller").GetComponent<GameController>();
        menuInfo = FindObjectsOfType<MenuInfo>()[FindObjectsOfType<MenuInfo>().Length - 1];
        modeChoice = menuInfo.modeNumber;
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
        if (modeChoice == 3 && !Input.GetMouseButton(1))
            CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Default);
        else if (board.isSelectable(gameObject.GetComponent<Piece>()) && (controller.activePlayer == controller.whitePlayer || modeChoice == 2) && !Input.GetMouseButton(1))
        {
            CursorManager.Instance.SetActiveCursorType(cursorType);
            Tooltip.ShowTooltip_Static("Piece: " + gameObject.GetComponent<Piece>().pieceType.ToString() + "\n" +
                "Corp: " + gameObject.GetComponent<Piece>().corpType.ToString());
        }
        else if (gameObject.name == "Highlighter(Clone)" && (controller.activePlayer == controller.whitePlayer || modeChoice == 2) && !Input.GetMouseButton(1))
        {
            List<Vector2Int> adjacentSquares = board.selectedPiece.GetAdjacentSquares();
            Vector2Int highlighterPosition = board.GetCoordsFromPosition(gameObject.transform.position);
            CursorManager.Instance.SetActiveCursorType(cursorType);
            Tooltip.ShowTooltip_Static("Spend " + board.selectedPiece.corpType.ToString() + " Corp's Command Authority");
            foreach (Vector2Int square in adjacentSquares)
            {
                if (square == highlighterPosition && !board.selectedPiece.CommanderMovedOne() && (board.selectedPiece.pieceType == PieceType.King || board.selectedPiece.pieceType == PieceType.Bishop))
                {
                    if (board.selectedPiece.pieceType == PieceType.King)
                    {
                        Tooltip.ShowTooltip_Static("Use King's Extra Move");
                        break;
                    }
                    else
                    {
                        Tooltip.ShowTooltip_Static("Use " + board.selectedPiece.corpType.ToString() + " Bishop's Extra Move");
                        break;
                    }
                }
            }
        }
        else if (IsHoveringEnemy() && (controller.activePlayer == controller.whitePlayer || modeChoice == 2) )
        {
            CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Kill);

            Piece enemyPiece = gameObject.GetComponent<Piece>();

            Tooltip.ShowTooltip_Static("Spend " + board.selectedPiece.corpType.ToString() + " Corp's Command Authority" + "\n" +
                "Roll Needed To Capture " + enemyPiece.pieceType.ToString() + ": " + board.GetRollNeeded(board.selectedPiece, enemyPiece) + "+");

        }
        else if ((!board.isSelectable(gameObject.GetComponent<Piece>()) || (controller.activePlayer != controller.whitePlayer || modeChoice == 2)) && !Input.GetMouseButton(1)) 
            CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Unavailable);
    }

    private void OnMouseExit()
    {
        CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Default);
        Tooltip.HideTooltip_Static();
    }

    private bool IsHoveringEnemy() 
    {
        if(board.selectedPiece && gameObject.name != "Highlighter(Clone)")
            foreach (Vector2Int move in board.selectedPiece.AvailableMoves) 
            {
                if (gameObject.GetComponent<Piece>().occupiedSquare == move)
                    return true;
            }
        return false;
    }
}

