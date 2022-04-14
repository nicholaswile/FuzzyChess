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
        {PieceType.King, 4000},
        {PieceType.Queen, 75},
        {PieceType.Knight, 55},
        {PieceType.Bishop, 70},
        {PieceType.Rook, 10},
        {PieceType.Pawn, 5}
    };
    private float threshold = 10.0f; //A move must be greater than or equal to this threshold to be chosen

    //IsAIAttacking | AI Piece | Opponent Piece | Roll Requirement | Value of the Piece Under Attack | Cost of Move
    private List<ArrayList> moveList = new List<ArrayList>(); 

    private IEnumerator AI_TakeTurn_Coroutine()
    {
        List<Piece> aiPieces = controller.blackPlayer.ActivePieces; 
        List<Piece> enemyPieces = controller.whitePlayer.ActivePieces;

        List<Piece> activeCorpPieces = controller.blackPlayer.KingCorpPieces;


        while (controller.activePlayer == controller.blackPlayer)
        {
            updateMoveList(aiPieces, enemyPieces);
            moveList.Sort(sortMoveList);

            Debug.Log("There are " + moveList.Count + " capturing moves that can be made.");
            foreach (ArrayList a in moveList)
            {
                if ((bool)a[0])
                    Debug.Log("AI " + ((Piece)a[1]).pieceType + " attacking Player " + ((Piece)a[2]).pieceType + ". A roll of " + a[3] + " will capture. The value of this piece is " + a[4] + ". Cost: " + a[5]);
                else
                    Debug.Log("AI " + ((Piece)a[1]).pieceType + " is being attacked by Player " + ((Piece)a[2]).pieceType + ". A roll of " + a[3] + " will capture. The value of this piece is " + a[4] + ". Cost: " + a[5]);
            }

            if (moveList.Count > 0 && (bool)moveList.ElementAt(0)[0] == true && ((Piece)moveList.ElementAt(0)[2]).pieceType == PieceType.King)
            {
                Piece attackingPiece = ((Piece)moveList.ElementAt(0)[1]);
                Vector3 piecePosition = board.GetPositionFromCoords(attackingPiece.occupiedSquare);
                Vector3 movePosition = board.GetPositionFromCoords(((Piece)moveList.ElementAt(0)[2]).occupiedSquare);

                //Select square with piece that can attack opponent King
                yield return new WaitForSeconds(1);
                board.OnSquareSelected(piecePosition);

                if(attackingPiece.pieceType == PieceType.Knight)
                {
                    Vector2Int preAttackSquare = new Vector2Int(-1, -1);
                    foreach (Vector2Int move in attackingPiece.AvailableMoves)
                    {
                        if (board.GetPieceOnSquare(move) == null)
                        {
                            foreach (Vector2Int doubleMove in attackingPiece.GetAdjacentEnemySquares(move))
                            {
                                if (doubleMove == ((Piece)moveList.ElementAt(0)[2]).occupiedSquare)
                                {
                                    preAttackSquare = move;
                                    break;
                                }
                            }
                            if (preAttackSquare != new Vector2Int(-1,-1))
                                break;
                        }
                    }

                    //Select square next to opponent King (Begins knight pre-attack)
                    yield return new WaitForSeconds(1);
                    board.OnSquareSelected(board.GetPositionFromCoords(preAttackSquare));
                    SFXController.PlaySoundMovement();

                    //Select square with opponent King (Begins attack roll)
                    yield return new WaitForSeconds(1);
                    board.OnSquareSelected(movePosition);
                    SFXController.PlaySoundMovement();

                }
                else
                {
                    //Select square with opponent King (Begins attack roll)
                    yield return new WaitForSeconds(1);
                    board.OnSquareSelected(movePosition);
                    SFXController.PlaySoundMovement();
                }
            }
            else
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

    //INPUT: A list of pieces from the AI team and a list from the enemy team
    //OUTPUT: Update moveList list of potential moves
    //FORMAT: IsAIAttacking | AI Piece | Opponent Piece | Roll Requirement | Value of the Piece Under Attack | Cost of Move
    private void updateMoveList(List<Piece> aiPieces, List<Piece> enemyPieces)
    {
        moveList.Clear();

        //For every AI piece
        foreach (Piece aiPiece in aiPieces)
        {
            if (aiPiece.AvailableMoves.Count > 0)
            {
                //Handle finding the true area a knight can capture. Knight's AvailableMoves doesn't give an honest representation.
                if (aiPiece.pieceType == PieceType.Knight)
                {
                    foreach (Vector2Int move in getKnightMoves(aiPiece))
                    {
                        //Add all additional knight moves.
                        Piece enemyPiece = board.GetPieceOnSquare(move);
                        moveList.Add(new ArrayList() { true, aiPiece, enemyPiece, getMinRoll(aiPiece, enemyPiece) - 1, captureWorth[enemyPiece.pieceType], getMoveCost(getMinRoll(aiPiece, enemyPiece), captureWorth[enemyPiece.pieceType]) });
                    }
                }

                //For every move that an AI piece can make
                foreach (Vector2Int move in aiPiece.AvailableMoves)
                {
                    Piece enemyPiece = board.GetPieceOnSquare(move);
                    //Add a potentialAttack if there is a piece and it is from the enemy's team.
                    if (enemyPiece != null && !aiPiece.IsFromSameTeam(enemyPiece))
                        moveList.Add(new ArrayList() { true, aiPiece, enemyPiece, getMinRoll(aiPiece, enemyPiece), captureWorth[enemyPiece.pieceType], getMoveCost(getMinRoll(aiPiece, enemyPiece), captureWorth[enemyPiece.pieceType]) });
                }
            }
        }

        //For every non-AI piece
        foreach (Piece enemyPiece in enemyPieces)
        {
            if (enemyPiece.AvailableMoves.Count > 0)
            {
                //Handle finding the true area a knight can capture. Knight's AvailableMoves doesn't give an honest representation.
                if (enemyPiece.pieceType == PieceType.Knight)
                {
                    foreach (Vector2Int move in getKnightMoves(enemyPiece))
                    {
                        //Add all additional knight moves.
                        Piece aiPiece = board.GetPieceOnSquare(move);
                        moveList.Add(new ArrayList() { false, aiPiece, enemyPiece, getMinRoll(enemyPiece, aiPiece) - 1, defenseWorth[aiPiece.pieceType], getMoveCost(getMinRoll(enemyPiece, aiPiece), defenseWorth[aiPiece.pieceType]) });
                    }
                }

                //For every move that a non-AI piece can make
                foreach (Vector2Int move in enemyPiece.AvailableMoves)
                {
                    Piece aiPiece = board.GetPieceOnSquare(move);
                    //Add a potentialDanger if there is a piece and it is from the AI's team.
                    if (aiPiece != null && !enemyPiece.IsFromSameTeam(aiPiece))
                        moveList.Add(new ArrayList() { false, aiPiece, enemyPiece, getMinRoll(enemyPiece, aiPiece), defenseWorth[aiPiece.pieceType], getMoveCost(getMinRoll(enemyPiece, aiPiece), defenseWorth[aiPiece.pieceType]) });
                }
            }
        }

        moveList.RemoveAll(outsideThreshold);
        moveList.RemoveAll(corpMoved);
    }

    private int getMinRoll(Piece attacking, Piece defending) {
        return captureTable[attacking.pieceType][defending.pieceType];
    }

    private float getMoveCost(int minRoll, int pieceValue) {
        return ((float)1/minRoll * 1.5f) * pieceValue;
    }

    private bool outsideThreshold(ArrayList move)
    {
        return (float)move[5] < threshold;
    }

    private int sortMoveList(ArrayList x, ArrayList y)
    {
        if ((float)x[5] < (float)y[5])
        {
            return 1;
        }
        else if ((float)x[5] > (float)y[5])
        {
            return -1;
        }
        else
            return 0;
    }

    private bool corpMoved(ArrayList move)
    {
        Piece piece = (Piece)move[1];
        if (piece.pieceType != PieceType.King && piece.pieceType != PieceType.Bishop)
        {
            if (piece.CorpMoveNumber() >= 2)
                return true;
            else if (piece.CorpMoveNumber() >= 1 && piece.CommanderMovedOne() == false)
                return true;
            else
                return false;
        }
        else
        {
            if (piece.CorpMoveNumber() >= 1)
                return true;
            return false;
        }
    }

    public void AI_TakeTurn() 
    {
        StartCoroutine(AI_TakeTurn_Coroutine());
    }
}
