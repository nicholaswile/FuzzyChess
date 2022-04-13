using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private ChessBoard board;
    [SerializeField] private GameController controller;
    [SerializeField] private GameUI gameUI;

    private Dictionary<PieceType, Dictionary<PieceType, int>> captureTable = new Dictionary<PieceType, Dictionary<PieceType, int>>() {
        {PieceType.King, new Dictionary<PieceType, int>() {{PieceType.King, 4}, {PieceType.Queen, 4}, {PieceType.Knight, 4}, {PieceType.Bishop, 4}, {PieceType.Rook, 5}, {PieceType.Pawn, 1}}},
        {PieceType.Queen, new Dictionary<PieceType, int>() {{PieceType.King, 4}, {PieceType.Queen, 4}, {PieceType.Knight, 4}, {PieceType.Bishop, 4}, {PieceType.Rook, 5}, {PieceType.Pawn, 2}}},
        {PieceType.Knight, new Dictionary<PieceType, int>() {{PieceType.King, 5}, {PieceType.Queen, 5}, {PieceType.Knight, 5}, {PieceType.Bishop, 5}, {PieceType.Rook, 5}, {PieceType.Pawn, 2}}},
        {PieceType.Bishop, new Dictionary<PieceType, int>() {{PieceType.King, 5}, {PieceType.Queen, 5}, {PieceType.Knight, 5}, {PieceType.Bishop, 4}, {PieceType.Rook, 5}, {PieceType.Pawn, 3}}},
        {PieceType.Rook, new Dictionary<PieceType, int>() {{PieceType.King, 4}, {PieceType.Queen, 4}, {PieceType.Knight, 4}, {PieceType.Bishop, 5}, {PieceType.Rook, 5}, {PieceType.Pawn, 5}}},
        {PieceType.Pawn, new Dictionary<PieceType, int>() {{PieceType.King, 6}, {PieceType.Queen, 6}, {PieceType.Knight, 6}, {PieceType.Bishop, 5}, {PieceType.Rook, 6}, {PieceType.Pawn, 4}}}
    };
    private Dictionary<PieceType, int> captureWorth = new Dictionary<PieceType, int>() {
        {PieceType.King, 5000},
        {PieceType.Queen, 70},
        {PieceType.Knight, 65},
        {PieceType.Bishop, 80},
        {PieceType.Rook, 25},
        {PieceType.Pawn, 10}
    };
    private Dictionary<PieceType, int> defenseWorth = new Dictionary<PieceType, int>() {
        {PieceType.King, 5000},
        {PieceType.Queen, 75},
        {PieceType.Knight, 55},
        {PieceType.Bishop, 70},
        {PieceType.Rook, 10},
        {PieceType.Pawn, 5}
    };
    private float threshold = 10.0f; //A move must be greater than or equal to this threshold to be chosen

    private List<ArrayList> potentialDangers = new List<ArrayList>(); //Team AI Defending Piece, Opponent Attacking Piece, Roll Requirement, Value of the Piece Under Attack
    private List<ArrayList> potentialAttacks = new List<ArrayList>(); //Team AI Attacking Piece, Opponent Defending Piece, Roll Requirement, Value of the Piece the AI is Attacking

    private IEnumerator AI_TakeTurn_Coroutine()
    {
        List<Piece> aiPieces = controller.blackPlayer.ActivePieces; 
        List<Piece> enemyPieces = controller.whitePlayer.ActivePieces;

        List<Piece> activeCorpPieces = controller.blackPlayer.KingCorpPieces;

        //Update potential danger from enemy captures and potential attacks that could be made.
        updatePotentialDanger(enemyPieces);
        updatePotentialAttacks(aiPieces);

        //DEBUG: Output to determine who is attacking what.
        Debug.Log("There are " + potentialDangers.Count + " attacking moves the non-AI team could make.");
        foreach (ArrayList a in potentialDangers)
            Debug.Log("AI " + ((Piece)a[0]).pieceType + " is being attacked by Player " + ((Piece)a[1]).pieceType + ". A roll of " + a[2] + " will capture. The value of this piece is " + a[3] + ". Cost: " + a[4]);
        Debug.Log("There are " + potentialAttacks.Count + " attacking moves the AI can make.");
        foreach (ArrayList a in potentialAttacks)
            Debug.Log("AI " + ((Piece)a[0]).pieceType + " attacking Player " + ((Piece)a[1]).pieceType+ ". A roll of " + a[2] + " will capture. The value of this piece is " + a[3] + ". Cost: " + a[4]);

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


    //INPUT: A knight piece
    //OUTPUT: A list of all the locations where the input knight may roll a +1 on capture.
    private List<Vector2Int> getKnightMoves(Piece knight)
    {
        List<Vector2Int> dangerSpots = new List<Vector2Int>();
        foreach (Vector2Int move in knight.AvailableMoves)
        {
            Piece piece = board.GetPieceOnSquare(move);

            //Check to make sure we're not looking at already-occupied squares. Knight can't move after killing.
            if (piece != null && !knight.IsFromSameTeam(piece))
                continue;
            else
            {
                //Run through each secondary move and add it if it is new. 
                foreach (Vector2Int doubleMove in knight.GetAdjacentEnemySquares(move))
                    if (!(dangerSpots.Contains(doubleMove)))
                        dangerSpots.Add(doubleMove);
            }

        }
        return dangerSpots;
    }

    //INPUT: A list of pieces from the attacking team (example: controller.whitePlayer.ActivePieces)
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
                    foreach (Vector2Int move in getKnightMoves(enemyPiece)) {
                        //Add all additional knight moves.
                        Piece aiPiece = board.GetPieceOnSquare(move);
                        potentialDangers.Add(new ArrayList() {aiPiece, enemyPiece, getMinRoll(enemyPiece, aiPiece) - 1, defenseWorth[aiPiece.pieceType], getMoveCost(getMinRoll(enemyPiece, aiPiece), defenseWorth[aiPiece.pieceType])});
                    }
                }

                //For every move that a non-AI piece can make
                foreach (Vector2Int move in enemyPiece.AvailableMoves)
                {
                    Piece aiPiece = board.GetPieceOnSquare(move);
                    //Add a potentialDanger if there is a piece and it is from the AI's team.
                    if (aiPiece != null && !enemyPiece.IsFromSameTeam(aiPiece))
                        potentialDangers.Add(new ArrayList() {aiPiece, enemyPiece, getMinRoll(enemyPiece, aiPiece), defenseWorth[aiPiece.pieceType], getMoveCost(getMinRoll(enemyPiece, aiPiece), defenseWorth[aiPiece.pieceType])});
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
                    foreach (Vector2Int move in getKnightMoves(aiPiece)) {
                        //Add all additional knight moves.
                        Piece enemyPiece = board.GetPieceOnSquare(move);
                        potentialAttacks.Add(new ArrayList() {aiPiece, enemyPiece, getMinRoll(aiPiece, enemyPiece) - 1, captureWorth[enemyPiece.pieceType], getMoveCost(getMinRoll(aiPiece, enemyPiece), captureWorth[enemyPiece.pieceType])});
                    }
                }

                //For every move that an AI piece can make
                foreach (Vector2Int move in aiPiece.AvailableMoves)
                {
                    Piece enemyPiece = board.GetPieceOnSquare(move);
                    //Add a potentialAttack if there is a piece and it is from the enemy's team.
                    if (enemyPiece != null && !aiPiece.IsFromSameTeam(enemyPiece))
                        potentialAttacks.Add(new ArrayList() {aiPiece, enemyPiece, getMinRoll(aiPiece, enemyPiece), captureWorth[enemyPiece.pieceType], getMoveCost(getMinRoll(aiPiece, enemyPiece), captureWorth[enemyPiece.pieceType])});
                }
            }
        }
    }

    private int getMinRoll(Piece attacking, Piece defending) {
        return captureTable[attacking.pieceType][defending.pieceType];
    }

    private float getMoveCost(int minRoll, int pieceValue) {
        return ((float)1/minRoll * 1.5f) * pieceValue;
    }

    public void AI_TakeTurn() 
    {
        StartCoroutine(AI_TakeTurn_Coroutine());
    }
}
