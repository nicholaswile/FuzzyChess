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
        List<Piece> enemyPieces = controller.whitePlayer.ActivePieces;
        List<Vector2Int> potentialCaptures = new List<Vector2Int>();

        //scan each enemy piece and determine what pieces that said piece could capture immediately.
        //status: naive to pieces that can move more than once per turn, and knights. functional otherwise.
        //(example: a bishop can move more than once during a turn. It can move, place itself, then capture from its new position.)
        //This means that for each piece that is capable of moving more than once in a turn, it must not only be assessed
        //for its potential to capture in its first position, but also its potential to capture in every other position it can reach.
        foreach (Piece corpPiece in enemyPieces.ToList())
        {
            //Debug.Log("Reached inside of enemypieces");
            //Debug.Log("Number of enemy pieces: " + enemyPieces.Count);
            //Debug.Log("Available moves: " + corpPiece.AvailableMoves.Count);
            Vector3 piecePosition = board.GetPositionFromCoords(corpPiece.occupiedSquare);

            if (corpPiece.AvailableMoves.Count > 0)
            {
                //Debug.Log("Reached inside of isselectable");
                //Debug.Log(corpPiece.AvailableMoves.Count);
                //yield return new WaitForSeconds(1);
                //board.OnSquareSelected(piecePosition);

                foreach (Vector2Int move in corpPiece.AvailableMoves.ToList())
                {
                    //see if we can detect that a "take" is available.
                    //Debug.Log("Reached inside of availablemoves");
                    Debug.Log(move);
                    Piece piece = board.GetPieceOnSquare(move);
                    if (piece != null && !corpPiece.IsFromSameTeam(piece))
                    {
                        Debug.Log("Found cap");
                        potentialCaptures.Add(move);
                    }

                }
            }
        }

        foreach(Vector2Int potcap in potentialCaptures.ToList())
        {
            Debug.Log(potcap);
        }
        Debug.Log("There are " + potentialCaptures.Count + " captures that could occur.");
        potentialCaptures.Clear();

        while (controller.activePlayer == controller.blackPlayer)
        {
                foreach (Piece corpPiece in activeCorpPieces.ToList())
            {
                Vector3 piecePosition = board.GetPositionFromCoords(corpPiece.occupiedSquare);

                if (board.isSelectable(corpPiece) && corpPiece.AvailableMoves.Count > 0)
                {
                    //Debug.Log(corpPiece.AvailableMoves.Count);
                    yield return new WaitForSeconds(1);
                    board.OnSquareSelected(piecePosition);

                    foreach (Vector2Int move in corpPiece.AvailableMoves.ToList())
                    {
                        //see if we can detect that a "take" is available.
                        //Debug.Log(move);
                        //Piece piece = board.GetPieceOnSquare(move);
                        //if (piece != null && !corpPiece.IsFromSameTeam(piece)) Debug.Log("Found cap");
                    }

                        foreach (Vector2Int move in corpPiece.AvailableMoves.ToList())
                    {
                        //Debug.Log(move);
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
