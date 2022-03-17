using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private ChessBoard board;
    [SerializeField] private GameController controller;
    [SerializeField] private GameUI gameUI;

    private IEnumerator AI_TakeTurn_Coroutine()
    {
        List<Piece> activeCorpPieces = controller.blackPlayer.KingCorpPieces;
        while (controller.activePlayer == controller.blackPlayer)
        {
            foreach (Piece corpPiece in activeCorpPieces.ToList())
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
            if (activeCorpPieces == controller.blackPlayer.KingCorpPieces) 
                activeCorpPieces = controller.blackPlayer.RightCorpPieces;
            else if (activeCorpPieces == controller.blackPlayer.RightCorpPieces) 
                activeCorpPieces = controller.blackPlayer.LeftCorpPieces;
            else if (activeCorpPieces == controller.blackPlayer.LeftCorpPieces) 
                activeCorpPieces = controller.blackPlayer.KingCorpPieces;
        }
    }
    public void AI_TakeTurn() 
    {
        StartCoroutine(AI_TakeTurn_Coroutine());
    }
}
