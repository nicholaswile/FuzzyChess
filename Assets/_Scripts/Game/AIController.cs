using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private ChessBoard board;
    [SerializeField] private GameController controller;

    private IEnumerator AI_TakeTurn_Coroutine()
    {
        foreach (Piece corpPiece in controller.blackPlayer.LeftCorpPieces.ToList())
        {
            Vector3 piecePosition = board.GetPositionFromCoords(corpPiece.occupiedSquare);

            if (board.isSelectable(corpPiece) && corpPiece.AvailableMoves.Count > 0)
            {
                yield return new WaitForSeconds(1);
                board.OnSquareSelected(piecePosition);


                foreach (Vector2Int move in corpPiece.AvailableMoves.ToList())
                {
                    Vector3 movePosition = board.GetPositionFromCoords(move);

                    yield return new WaitForSeconds(1);
                    board.OnSquareSelected(movePosition);

                    break;
                }
                break;

            }
        }
    }
    public void AI_TakeTurn() 
    {
        StartCoroutine(AI_TakeTurn_Coroutine());
    }
}
