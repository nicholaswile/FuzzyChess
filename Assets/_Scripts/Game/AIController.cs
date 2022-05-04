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
    private GameState state;

    private Dictionary<PieceType, Dictionary<PieceType, int>> captureTable = new Dictionary<PieceType, Dictionary<PieceType, int>>() {
        {PieceType.King, new Dictionary<PieceType, int>() {{PieceType.King, 4}, {PieceType.Queen, 4}, {PieceType.Knight, 4}, {PieceType.Bishop, 4}, {PieceType.Rook, 5}, {PieceType.Pawn, 1}}},
        {PieceType.Queen, new Dictionary<PieceType, int>() {{PieceType.King, 4}, {PieceType.Queen, 4}, {PieceType.Knight, 4}, {PieceType.Bishop, 4}, {PieceType.Rook, 5}, {PieceType.Pawn, 2}}},
        {PieceType.Knight, new Dictionary<PieceType, int>() {{PieceType.King, 5}, {PieceType.Queen, 5}, {PieceType.Knight, 5}, {PieceType.Bishop, 5}, {PieceType.Rook, 5}, {PieceType.Pawn, 2}}},
        {PieceType.Bishop, new Dictionary<PieceType, int>() {{PieceType.King, 5}, {PieceType.Queen, 5}, {PieceType.Knight, 5}, {PieceType.Bishop, 4}, {PieceType.Rook, 5}, {PieceType.Pawn, 3}}},
        {PieceType.Rook, new Dictionary<PieceType, int>() {{PieceType.King, 4}, {PieceType.Queen, 4}, {PieceType.Knight, 4}, {PieceType.Bishop, 5}, {PieceType.Rook, 5}, {PieceType.Pawn, 5}}},
        {PieceType.Pawn, new Dictionary<PieceType, int>() {{PieceType.King, 6}, {PieceType.Queen, 6}, {PieceType.Knight, 6}, {PieceType.Bishop, 5}, {PieceType.Rook, 6}, {PieceType.Pawn, 4}}}
    };
    private Dictionary<PieceType, int> moveValue = new Dictionary<PieceType, int>() {
        {PieceType.King, 1},
        {PieceType.Queen, 10},
        {PieceType.Knight, 15},
        {PieceType.Bishop, 2},
        {PieceType.Rook, 15},
        {PieceType.Pawn, 5}
    };
    private Dictionary<PieceType, int> pieceValue = new Dictionary<PieceType, int>() {
        {PieceType.King, 500},
        {PieceType.Queen, 80},
        {PieceType.Knight, 120},
        {PieceType.Bishop, 140},
        {PieceType.Rook, 100},
        {PieceType.Pawn, 10}
    };

    //Is Attack Move | AI Piece | Movement Location | Value of Move
    private List<ArrayList> aiMoves = new List<ArrayList>();

    private IEnumerator AI_TakeTurn_Coroutine() //TODO: Make AI better select which pieces to move
    {
        List<Piece> aiPieces = controller.activePlayer.ActivePieces; 

        aiMoves = updateAiMoves(aiPieces);

        while (aiMoves.Count > 0)
        {
            //Break loop at end of game.
            if (state == GameState.Win || state == GameState.Lose)
                break;

            //DEBUG: Output number of moves avaliable, what's attacking, what's being attacked, and all values related to movement.
            Debug.Log("There are " + aiMoves.Count + " AI moves that can be made this turn.");
            foreach (ArrayList a in aiMoves)
            {
                if ((bool)a[0])
                    Debug.Log("AI " + ((Piece)a[1]).pieceType + " attacking Player " + board.GetPieceOnSquare(board.GetCoordsFromPosition((Vector3)a[2])) + ". Move Value: " + a[3]);
                else
                    Debug.Log("AI " + ((Piece)a[1]).pieceType + " moving to " + board.GetCoordsFromPosition((Vector3)a[2]) + ". Move Value: " + a[3]);
            }

            Piece attackingPiece = (Piece)aiMoves.ElementAt(0)[1];
            Vector3 piecePosition = board.GetPositionFromCoords(attackingPiece.occupiedSquare);
            Vector3 movePosition = (Vector3)aiMoves.ElementAt(0)[2];

            //Select square with piece that can attack opponent
            yield return new WaitForSeconds(1.75f);
            board.OnSquareSelected(piecePosition);

            if (attackingPiece.pieceType != PieceType.Knight)
            {
                //Attack (Begins Roll)
                yield return new WaitForSeconds(1.25f);
                board.OnSquareSelected(movePosition);
                SFXController.PlaySoundMovement();
            }
            else
            {
                //Knight Determine Movement Location before Attack
                Vector2Int preAttackSquare = attackingPiece.occupiedSquare;
                foreach (Vector2Int move in attackingPiece.AvailableMoves)
                {
                    if (board.GetPieceOnSquare(move) == null)
                    {
                        foreach (Vector2Int doubleMove in attackingPiece.GetAdjacentEnemySquares(move))
                        {
                            if (doubleMove == board.GetCoordsFromPosition(movePosition))
                            {
                                preAttackSquare = move;
                                break;
                            }
                        }
                        if (preAttackSquare != attackingPiece.occupiedSquare)
                            break;
                    }
                }
                
                //Knight take Pre-Attack Jump if Jump exists.
                if(preAttackSquare != attackingPiece.occupiedSquare)
                {
                    yield return new WaitForSeconds(1.25f);
                    board.OnSquareSelected(board.GetPositionFromCoords(preAttackSquare));
                    SFXController.PlaySoundMovement();
                }

                //Knight Attack (Begins Roll with +1)
                yield return new WaitForSeconds(1.25f);
                board.OnSquareSelected(movePosition);
                SFXController.PlaySoundMovement();
            }

            aiMoves = updateAiMoves(aiPieces);
            //threateningMoves = updateThreateningMoves(aiPieces, enemyPieces);
        }

        //Skip turn when there are no more moves to make.
        GameObject.Find("UI").GetComponent<GameUI>().UI_Skip();

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
    //OUTPUT: Sorted list of moves the AI can make
    //FORMAT: aiMoves = {Is Attack Move | AI Piece | Movement Location | Value of Move}
    private List<ArrayList> updateAiMoves(List<Piece> aiPieces)
    {
        List<ArrayList> newAiMoves = new List<ArrayList>();

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
                        newAiMoves.Add(new ArrayList() { true, aiPiece, board.GetPositionFromCoords(enemyPiece.occupiedSquare), getMoveValue(true, true, move, aiPiece, enemyPiece) - 1 });
                    }
                }

                //For every move that an AI piece can make
                foreach (Vector2Int move in aiPiece.AvailableMoves)
                {
                    Piece enemyPiece = board.GetPieceOnSquare(move);
                    if (enemyPiece != null && !aiPiece.IsFromSameTeam(enemyPiece))
                        //Add an aiMove capture if there is a piece and it is from the enemy's team.
                        newAiMoves.Add(new ArrayList() { true, aiPiece, board.GetPositionFromCoords(enemyPiece.occupiedSquare), getMoveValue(true, false, move, aiPiece, enemyPiece) });
                    else if (enemyPiece == null)
                    {
                        //Add an aiMove movement if there no piece in an avaliable move spot
                        newAiMoves.Add(new ArrayList() { false, aiPiece, board.GetPositionFromCoords(move), getMoveValue(false, false, move, aiPiece, null) });
                    }
                }
            }
        }

        //Remove all AI moves where the corp has already moved
        newAiMoves.RemoveAll(corpMoved);

        //Sort all moves by Value
        newAiMoves.Sort(sortMoveList);

        return newAiMoves;
    }

    private float getMoveValue(bool isCapture, bool isKnightSpecial, Vector2Int moveLocation, Piece aiPiece, Piece enemyPiece)
    {
        if (isCapture && isKnightSpecial)
            return (float)1 / (captureTable[aiPiece.pieceType][enemyPiece.pieceType] - 1) * pieceValue[enemyPiece.pieceType];
        else if (isCapture)
            return (float)1 / (captureTable[aiPiece.pieceType][enemyPiece.pieceType]) * pieceValue[enemyPiece.pieceType];
        else
        {
            float yAim;
            if (controller.activePlayer == controller.blackPlayer)
                yAim = .1f;
            else
                yAim = -.1f;

            return (float)moveValue[aiPiece.pieceType] + (aiPiece.occupiedSquare.y - moveLocation.y) * yAim;
        }
    }

    private int sortMoveList(ArrayList x, ArrayList y)
    {
        if ((float)x[x.Count - 1] < (float)y[y.Count - 1])
        {
            return 1;
        }
        else if ((float)x[x.Count - 1] > (float)y[y.Count - 1])
        {
            return -1;
        }
        else
            return 0;
    }

    private bool corpMoved(ArrayList move)
    {
        Piece piece = (Piece)move[1];
        if (piece.corpType == CorpType.Left && controller.LeftCorpUsed < 1)
            return false;
        if (piece.corpType == CorpType.Right && controller.RightCorpUsed < 1)
            return false;
        if (piece.corpType == CorpType.King && controller.KingCorpUsed < 1)
            return false;
        return true;
    }

    private bool belowThreshold(ArrayList move)
    {
        return (float)move[3] < 5;
    }

    public void AI_TakeTurn() 
    {
        StartCoroutine(AI_TakeTurn_Coroutine());
    }

    private void Awake()
    {
        GameManager.StateChanged += GameManager_StateChanged;
    }

    private void GameManager_StateChanged(GameState state)
    {
        this.state = state;
    }
}
