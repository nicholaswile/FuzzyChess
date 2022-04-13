using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private ChessBoard board;
    [SerializeField] private GameController controller;
    [SerializeField] private GameUI gameUI;

    private int baseCostPerMove = 50; //Base "cost" that a move takes. The AI will attempt to search for the lowest cost. Prevents unnecessary movements.

    private List<ArrayList> potentialDangers = new List<ArrayList>(); //First item is the defending piece, second item is the attacking piece.
    private List<ArrayList> potentialAttacks = new List<ArrayList>(); //First item is the attacking piece, second item is the defending piece.

    private IEnumerator AI_TakeTurn_Coroutine()
    {
        List<Piece> aiPieces = controller.blackPlayer.ActivePieces; 
        List<Piece> enemyPieces = controller.whitePlayer.ActivePieces;

        List<Piece> activeCorpPieces = controller.blackPlayer.KingCorpPieces;

        //Update potential danger from enemy captures and potential attacks that could be made.
        updatePotentialDanger(enemyPieces);
        updatePotentialAttacks(aiPieces);

        Debug.Log("There are " + potentialDangers.Count + " attacking moves the non-AI team could make.");
        foreach (ArrayList a in potentialDangers)
            Debug.Log("AI " + ((Piece)a[0]).pieceType + " is being attacked by Player" + ((Piece)a[1]).pieceType);

        Debug.Log("There are " + potentialAttacks.Count + " attacking moves the AI can make.");
        foreach (ArrayList a in potentialAttacks)
            Debug.Log("AI " + ((Piece)a[0]).pieceType + " attacking Player" + ((Piece)a[1]).pieceType);

        yield return new WaitForSeconds(30);

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
                        SFXController.PlaySoundMovement();

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


    //Creates a list of type Vector2Int which stores all the locations of pieces which a knight may roll a +1 on capture.
    //This was created because knight movement in AvailableMoves doesn't list places a knight may move to, then capture around.
    //Pieces stored in this list are at a heightened risk of capture, as knights get +1 on their roll for these.
    private List<Vector2Int> KnightHighDangerZone(Piece piece)
    {
        List<Vector2Int> dangerSpots = new List<Vector2Int>();
        foreach (Vector2Int move in piece.AvailableMoves.ToList())
        {
            Debug.Log("Knight first move location: " + move);
            //gather a list of new knight moves at each location that the knight could potentially land.
            Piece contestedPiece = board.GetPieceOnSquare(move);

            //check to make sure we're not looking at already-occupied squares. knight can't move after killing.
            if (contestedPiece != null && !piece.IsFromSameTeam(contestedPiece))
            {
                continue;
            }
            else
            {
                List<Vector2Int> newKnightMoves = piece.GetAdjacentEnemySquares(move);
                foreach (Vector2Int futureSpot in newKnightMoves)
                {
                    //unneeded, since GetAdjacentEnemySquares already checks if there's a piece and if it's an enemy on the square.
                    //Piece possiblePiece = board.GetPieceOnSquare(futureSpot);

                    //go through the usual checks to see if something may be captured, but also check to avoid duplicates. Dupes are unneeded.
                    if (!(dangerSpots.Contains(futureSpot)))
                    {
                        Debug.Log("Added danger spot from knight at " + futureSpot);
                        dangerSpots.Add(futureSpot);
                    }
                }
            }

        }
        return dangerSpots;
    }

    //INPUT: a list of pieces from the attacking team (example: controller.whitePlayer.ActivePieces)
    //OUTPUT: Update potentialDangers list of potential attacking moves by the non-AI team.
    private void updatePotentialDanger(List<Piece> enemyPieces)
    {
        potentialDangers.Clear();
        foreach (Piece enemyPiece in enemyPieces) //For every non-AI piece
        {
            if (enemyPiece.AvailableMoves.Count > 0)
            {
                //Handle finding the true area a knight can capture. Knight's AvailableMoves doesn't give an honest representation.
                if (enemyPiece.pieceType == PieceType.Knight)
                {
                    foreach (Vector2Int dz in KnightHighDangerZone(enemyPiece))
                        //Add all additional knight moves.
                        potentialDangers.Add(new ArrayList() {board.GetPieceOnSquare(dz), enemyPiece});
                }

                //For every move that a non-AI piece can make
                foreach (Vector2Int move in enemyPiece.AvailableMoves)
                {
                    Piece aiPiece = board.GetPieceOnSquare(move);
                    //Add a potentialDanger if there is a piece and it is from the AI's team.
                    if (aiPiece != null && !enemyPiece.IsFromSameTeam(aiPiece))
                        potentialDangers.Add(new ArrayList() {aiPiece, enemyPiece});
                }
            }
        }
    }

    //INPUT: a list of pieces from the attacking team (example: controller.whitePlayer.ActivePieces)
    //OUTPUT: Update potentialAttacks list of potential attacking moves by the AI team.
    private void updatePotentialAttacks(List<Piece> aiPieces)
    {
        potentialAttacks.Clear();
        foreach (Piece aiPiece in aiPieces) //For every AI piece
        {
            if (aiPiece.AvailableMoves.Count > 0)
            {
                //Handle finding the true area a knight can capture. Knight's AvailableMoves doesn't give an honest representation.
                if (aiPiece.pieceType == PieceType.Knight)
                {
                    foreach (Vector2Int dz in KnightHighDangerZone(aiPiece))
                        //Add all additional knight moves.
                        potentialAttacks.Add(new ArrayList() {aiPiece, board.GetPieceOnSquare(dz)});
                }

                //For every move that an AI piece can make
                foreach (Vector2Int move in aiPiece.AvailableMoves)
                {
                    Piece enemyPiece = board.GetPieceOnSquare(move);
                    //Add a potentialAttack if there is a piece and it is from the enemy's team.
                    if (enemyPiece != null && !aiPiece.IsFromSameTeam(enemyPiece))
                        potentialAttacks.Add(new ArrayList() {aiPiece, enemyPiece});
                }
            }
        }
    }

    public void AI_TakeTurn() 
    {
        StartCoroutine(AI_TakeTurn_Coroutine());
    }
}
