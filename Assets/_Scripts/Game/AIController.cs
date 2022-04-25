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
    private Dictionary<PieceType, int> captureValue = new Dictionary<PieceType, int>() {
        {PieceType.King, 101},
        {PieceType.Queen, 40},
        {PieceType.Knight, 60},
        {PieceType.Bishop, 70},
        {PieceType.Rook, 50},
        {PieceType.Pawn, 5}
    };
    private Dictionary<PieceType, int> threatenedValue = new Dictionary<PieceType, int>() {
        {PieceType.King, 100},
        {PieceType.Queen, 40},
        {PieceType.Knight, 60},
        {PieceType.Bishop, 70},
        {PieceType.Rook, 50},
        {PieceType.Pawn, 5}
    };

    //IsAIAttacking | AI Piece | Opponent Piece | Roll Requirement | Value of the Threatened Piece | Cost of Move
    private List<ArrayList> moveList = new List<ArrayList>();

    //Is Attack Move | AI Piece | Movement Location | Value of Move
    private List<ArrayList> aiMoves = new List<ArrayList>();

    //AI Piece | Opponent Piece | Value of Move
    private List<ArrayList> threateningMoves = new List<ArrayList>();

    private IEnumerator AI_TakeTurn_Coroutine()
    {
        List<Piece> aiPieces = controller.activePlayer.ActivePieces; 
        List<Piece> enemyPieces = controller.GetOppositePlayer(controller.activePlayer).ActivePieces;

        List<Piece> activeCorpPieces = controller.blackPlayer.KingCorpPieces;


        while (controller.activePlayer == controller.blackPlayer)
        {
            //Break loop at end of game.
            if (state == GameState.Win || state == GameState.Lose)
                break;

            updateMoveList(aiPieces, enemyPieces);
            aiMoves.Sort(sortMoveList);
            threateningMoves.Sort(sortMoveList);
            //moveList.Sort(sortMoveList);

            //DEBUG: Output number of moves avaliable, what's attacking, what's being attacked, and all values related to movement.
            Debug.Log("There are " + aiMoves.Count + " AI moves that can be made this turn, and " + threateningMoves.Count + " opponent moves that are threatining the AI.");
            //Debug.Log("There are " + moveList.Count + " possible capturing moves (this turn and next) that can be made.");
            foreach (ArrayList a in aiMoves)
            {
                if ((bool)a[0])
                    Debug.Log("AI " + ((Piece)a[1]).pieceType + " attacking Player " + board.GetPieceOnSquare(board.GetCoordsFromPosition((Vector3)a[2])) + ". Move Value: " + a[3]);
                else
                    Debug.Log("AI " + ((Piece)a[1]).pieceType + " moving to " + ((Vector3)a[2]) + ". Move Value: " + a[3]);
            }
            foreach (ArrayList a in threateningMoves)
                Debug.Log("AI " + ((Piece)a[0]).pieceType + " is threatened by opponent " + ((Piece)a[1]) + ". Move Value: " + a[2]);
            /*foreach (ArrayList a in moveList)
            {
                if ((bool)a[0])
                    Debug.Log("AI " + ((Piece)a[1]).pieceType + " attacking Player " + ((Piece)a[2]).pieceType + ". A roll of " + a[3] + " will capture. The value of this piece is " + a[4] + ". Cost: " + a[5]);
                else
                    Debug.Log("AI " + ((Piece)a[1]).pieceType + " is being attacked by Player " + ((Piece)a[2]).pieceType + ". A roll of " + a[3] + " will capture. The value of this piece is " + a[4] + ". Cost: " + a[5]);
            }*/

            int i = 0;
            while (i < moveList.Count && (bool)moveList.ElementAt(i)[0] == false)
                i++;

            if (false && moveList.Count > 0 && i < moveList.Count)
            {
                Piece attackingPiece = ((Piece)moveList.ElementAt(i)[1]);
                Vector3 piecePosition = board.GetPositionFromCoords(attackingPiece.occupiedSquare);
                Vector3 movePosition = board.GetPositionFromCoords(((Piece)moveList.ElementAt(i)[2]).occupiedSquare);

                //Select square with piece that can attack opponent
                yield return new WaitForSeconds(2.5f);
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
                                if (doubleMove == ((Piece)moveList.ElementAt(i)[2]).occupiedSquare)
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
            }
            else
            {
                foreach (Piece corpPiece in activeCorpPieces.ToList())
                {
                    Vector3 piecePosition = board.GetPositionFromCoords(corpPiece.occupiedSquare);
                    if (board.isSelectable(corpPiece) && corpPiece.AvailableMoves.Count > 0)
                    {
                        yield return new WaitForSeconds(2.5f);
                        board.OnSquareSelected(piecePosition);

                        foreach (Vector2Int move in corpPiece.AvailableMoves.ToList())
                        {
                            Vector3 movePosition = board.GetPositionFromCoords(move);

                            yield return new WaitForSeconds(1.25f);
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
    //FORMAT: aiMoves = {Is Attack Move | AI Piece | Movement Location | Value of Move}
    //        threateningMoves = {AI Piece | Opponent Piece | Value of Move}
    private void updateMoveList(List<Piece> aiPieces, List<Piece> enemyPieces)
    {
        aiMoves.Clear();
        threateningMoves.Clear();
        //moveList.Clear();

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
                        aiMoves.Add(new ArrayList() { true, aiPiece, board.GetPositionFromCoords(enemyPiece.occupiedSquare), getMoveValue(getMinRoll(aiPiece, enemyPiece) - 1, captureValue[enemyPiece.pieceType]) });
                        //moveList.Add(new ArrayList() { true, aiPiece, enemyPiece, getMinRoll(aiPiece, enemyPiece) - 1, captureValue[enemyPiece.pieceType], getMoveValue(getMinRoll(aiPiece, enemyPiece) - 1, captureValue[enemyPiece.pieceType]) });
                    }
                }

                //For every move that an AI piece can make
                foreach (Vector2Int move in aiPiece.AvailableMoves)
                {
                    Piece enemyPiece = board.GetPieceOnSquare(move);
                    //Add a potentialAttack if there is a piece and it is from the enemy's team.
                    if (enemyPiece != null && !aiPiece.IsFromSameTeam(enemyPiece))
                        aiMoves.Add(new ArrayList() { true, aiPiece, board.GetPositionFromCoords(enemyPiece.occupiedSquare), getMoveValue(getMinRoll(aiPiece, enemyPiece), captureValue[enemyPiece.pieceType]) });
                        //moveList.Add(new ArrayList() { true, aiPiece, enemyPiece, getMinRoll(aiPiece, enemyPiece), captureValue[enemyPiece.pieceType], getMoveValue(getMinRoll(aiPiece, enemyPiece), captureValue[enemyPiece.pieceType]) });
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
                        threateningMoves.Add(new ArrayList() { aiPiece, enemyPiece, getMoveValue(getMinRoll(enemyPiece, aiPiece) - 1, threatenedValue[aiPiece.pieceType]) });
                        //moveList.Add(new ArrayList() { false, aiPiece, enemyPiece, getMinRoll(enemyPiece, aiPiece) - 1, threatenedValue[aiPiece.pieceType], getMoveValue(getMinRoll(enemyPiece, aiPiece) - 1, threatenedValue[aiPiece.pieceType]) });
                    }
                }

                //For every move that a non-AI piece can make
                foreach (Vector2Int move in enemyPiece.AvailableMoves)
                {
                    Piece aiPiece = board.GetPieceOnSquare(move);
                    //Add a potentialDanger if there is a piece and it is from the AI's team.
                    if (aiPiece != null && !enemyPiece.IsFromSameTeam(aiPiece))
                        threateningMoves.Add(new ArrayList() { aiPiece, enemyPiece, getMoveValue(getMinRoll(enemyPiece, aiPiece), threatenedValue[aiPiece.pieceType]) });
                        //moveList.Add(new ArrayList() { false, aiPiece, enemyPiece, getMinRoll(enemyPiece, aiPiece), threatenedValue[aiPiece.pieceType], getMoveValue(getMinRoll(enemyPiece, aiPiece), threatenedValue[aiPiece.pieceType]) });
                }
            }
        }

        //moveList.RemoveAll(corpMoved);
        aiMoves.RemoveAll(corpMoved);
    }

    private int getMinRoll(Piece attacking, Piece defending)
    {
        return captureTable[attacking.pieceType][defending.pieceType];
    }

    private float getMoveValue(int minRoll, int pieceValue)
    {
        return ((float)1/minRoll) * pieceValue;
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

    private void Awake()
    {
        GameManager.StateChanged += GameManager_StateChanged;
    }

    private void GameManager_StateChanged(GameState state)
    {
        this.state = state;
    }
}
