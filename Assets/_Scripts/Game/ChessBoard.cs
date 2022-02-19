using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CreateHighlighters))]
public class ChessBoard : MonoBehaviour
{
    [SerializeField] private Transform bottomLeftMarker;
    [SerializeField] private float squareSize;
    public const int BOARD_SIZE = 8;
    private Piece[,] grid;
    private Piece selectedPiece;
    private GameController controller;
    private CreateHighlighters highlighter;
    private readonly List<String> pieceMoves = new List<String>();

    private bool canCapture = false, willCapture = false;
    private bool knightHasMoved = false, knightAttemptedKill = false;

    private const string DICE = "ResultDie";

    private void Awake()
    {
        highlighter = GetComponent<CreateHighlighters>();
        CreateBoardGrid();
    }

    public void SetDependencies(GameController controller)
    {
        this.controller = controller;
    }

    private void CreateBoardGrid()
    {
        grid = new Piece[BOARD_SIZE, BOARD_SIZE];
    }

    public Vector3 GetPositionFromCoords(Vector2Int coords)
    {
        return bottomLeftMarker.position + new Vector3(coords.x * squareSize, 0f, coords.y * squareSize);
    }

    private Vector2Int GetCoordsFromPosition(Vector3 inputPosition)
    {
        int x = Mathf.FloorToInt(inputPosition.x / squareSize) + BOARD_SIZE / 2;
        int y = Mathf.FloorToInt(inputPosition.z / squareSize) + BOARD_SIZE / 2;
        return new Vector2Int(x, y);
    }

    //modified by TW
    public void OnSquareSelected(Vector3 inputPosition)
    {
        Vector2Int coords = GetCoordsFromPosition(inputPosition);
        Piece piece = GetPieceOnSquare(coords);

        if (piece && controller.IsTeamTurnActive(piece.team))
            controller.TryToChangeActiveCorp(piece.corpType);

        if (selectedPiece)
        {
            if (piece != null && selectedPiece == piece && knightHasMoved == false)
                DeselectPiece();

            else if (piece != null && selectedPiece != piece && controller.IsTeamTurnActive(piece.team) && controller.IsCorpTurnActive(piece.corpType) && knightHasMoved == false)
                SelectPiece(piece);

            else if (selectedPiece.CanMoveTo(coords))
            {
                OnSelectedPieceMoved(coords, selectedPiece);
            }
        }
        else
        {
            if (piece != null && controller.IsTeamTurnActive(piece.team) && controller.IsCorpTurnActive(piece.corpType))
                SelectPiece(piece);
        }
    }

    private void SelectPiece(Piece piece)
    {
        selectedPiece = piece;
        List<Vector2Int> selection = selectedPiece.AvailableMoves;
        ShowSelectionSquares(selection);
    }

    private void ShowSelectionSquares(List<Vector2Int> selection)
    {
        Dictionary<Vector3, bool> squaresData = new Dictionary<Vector3, bool>();
        for (int i = 0; i < selection.Count; i++)
        {
            Vector3 position = GetPositionFromCoords(selection[i]);
            bool isSquareFree = GetPieceOnSquare(selection[i]) == null;

            if (position == GetPositionFromCoords(selectedPiece.occupiedSquare))
                isSquareFree = true;

            squaresData.Add(position, isSquareFree);
        }
        highlighter.ShowAvailableMoves(squaresData);
    }

    public void DeselectPiece()
    {
        selectedPiece = null;
        highlighter.ClearMoves();
    }

    private void OnSelectedPieceMoved(Vector2Int coords, Piece piece)
    {
        TryToTakeOppositePiece(coords);

        //Adds move to the move list
        if (selectedPiece.pieceType != PieceType.Knight || knightHasMoved || knightAttemptedKill || !selectedPiece.HasAdjacentEnemySquares(coords))
        {
            String test = selectedPiece.GetType().ToString() + "|" + coords.ToString();
            pieceMoves.Add(test);
        }

        if (!canCapture || willCapture)
        {
            UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null);
            selectedPiece.MovePiece(coords);
        } 

        DeselectPiece();

        //If knight can move again, call the function to show the new available moves
        if (piece.pieceType == PieceType.Knight && !knightHasMoved && !knightAttemptedKill && piece.HasAdjacentEnemySquares(coords))
        {
            ShowKnightSelectionAfterMoving(piece);
        }
        else 
        {
            EndTurn();
            knightHasMoved = false;
            knightAttemptedKill = false;
        }
    }

    private void ShowKnightSelectionAfterMoving(Piece piece)
    {
        knightHasMoved = true;
        piece.AvailableMoves.Clear();
        List<Vector2Int> newKnightMoves = piece.GetAdjacentEnemySquares(piece.occupiedSquare);
        piece.AvailableMoves.AddRange(newKnightMoves);
        piece.AvailableMoves.Add(piece.occupiedSquare);
        SelectPiece(piece);
    }

    private void TryToTakeOppositePiece(Vector2Int coords)
    {
        Piece piece = GetPieceOnSquare(coords);
        canCapture = false;
        if (piece != null && !selectedPiece.IsFromSameTeam(piece))
        {
            canCapture = true;
            //GameManager.Instance.UpdateRollState(RollState.TrueRoll);

            bool b1 = selectedPiece.GetComponent<King>();
            bool b2 = selectedPiece.GetComponent<Queen>();
            bool b3 = selectedPiece.GetComponent<Knight>();
            bool b4 = selectedPiece.GetComponent<Rook>();
            bool b5 = selectedPiece.GetComponent<Bishop>();
            bool b6 = selectedPiece.GetComponent<Pawn>();

            bool b7 = piece.GetComponent<King>();
            bool b8 = piece.GetComponent <Queen>();
            bool b9 = piece.GetComponent<Knight>();
            bool b10 = piece.GetComponent<Rook>();
            bool b11 = piece.GetComponent<Bishop>();
            bool b12 = piece.GetComponent<Pawn>();

            int result = UnityEngine.Random.Range(1, 7);
            GameUI TheGameUI = GameObject.Find("UI").GetComponent<GameUI>();
            TheGameUI.SendMessage(DICE, result);

            //If knight does surprise attack add a +1 to the die roll
            if (knightHasMoved) 
            {
                result++;
            }

            Debug.Log("Result: " + result);
            bool take = false;

            // Okay, hear me out. I know this is unreadable, but it works, and it was the quickest way I could bodge together a solution in time. Thank you for understanding. If we want to fix this in the next sprint, I can cover it - NW

            if (result >= 6)
            {
                if (b6)
                {
                    if (b7 || b8 || b9 || b10) { take = true; }
                }
            }
            if (result >= 5)
            {
                if (b10)
                {
                    if (b1 || b2 || b3 || b4 || b5) { take = true; }
                }
                if (b3)
                {
                    if (b7 || b8 || b9 || b11) { take = true; }
                }
                if (b5)
                {
                    if (b7 || b8 || b9) { take = true; }
                }
                if (b4)
                {
                    if (b11 || b12) { take = true; }
                }
                if (b6 && b11) { take = true; }
            }
            if (result >= 4)
            {
                if (b1 || b2)
                {
                    if (b7 || b8 || b9 || b11) { take = true; }
                }
                if (b5 && b11) { take = true; }
                if (b4)
                {
                    if (b7 || b8 || b9) { take = true; }
                }
                if (b6 && b12) { take = true; }
            }
            if (result >= 3)
            {
                if (b5 && b12) { take = true; }
            }
            if (result >= 2)
            {
                if (b12)
                {
                    if (b2 || b3) { take = true; }
                }
            }
            if (result >= 1)
            {
                if (b1 && b12) { take = true; }
            }

            if (selectedPiece.pieceType == PieceType.Knight) 
            {
                knightAttemptedKill = true;
            }

            if (take) { willCapture = true; TakePiece(piece); }
            //edited by TW
            else {
                GameUI TheGameU = GameObject.Find("UI").GetComponent<GameUI>();
                TheGameU.PieceTakeFailed();
                willCapture = false; }
        }
    }


    private void TakePiece(Piece piece)
    {
        if (piece)
        {
            GameUI TheGameUI = GameObject.Find("UI").GetComponent<GameUI>();
            TheGameUI.PieceWasTaken();
            grid[piece.occupiedSquare.x, piece.occupiedSquare.y] = null;
            controller.OnPieceRemoved(piece);
        }
    }

 
    private void EndTurn()
    {
        GameUI TheGameUI = GameObject.Find("UI").GetComponent<GameUI>();
        TheGameUI.updateMoveList();
        controller.EndTurn();
    }

    public void UpdateBoardOnPieceMove(Vector2Int newCoords, Vector2Int oldCoords, Piece newPiece, Piece oldPiece)
    {
        grid[oldCoords.x, oldCoords.y] = oldPiece;
        grid[newCoords.x, newCoords.y] = newPiece;
    }

    public Piece GetPieceOnSquare(Vector2Int coords)
    {
        if (CheckIfCoordsAreOnBoard(coords))
            return grid[coords.x, coords.y];
        return null;
    }

    public bool CheckIfCoordsAreOnBoard(Vector2Int coords)
    {
        if (coords.x < 0 || coords.y < 0 || coords.x >= BOARD_SIZE || coords.y >= BOARD_SIZE)
            return false;
        return true;
    }

    public int GetNumberOfPieceMoves()
    {
        return pieceMoves.Count;
    }

    public bool HasPiece(Piece piece)
    {
        for (int i = 0; i < BOARD_SIZE; i++)
        {
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                if (grid[i, j] == piece)
                    return true;
            }
        }
        return false;
    }

    public void SetPieceOnBoard(Vector2Int coords, Piece piece)
    {
        if (CheckIfCoordsAreOnBoard(coords))
            grid[coords.x, coords.y] = piece;
    }

    //TW - public method to return array which only contains new piece movements, 
    //discarding the current piecemoves array. Should save on computing power.
    public List<String> GetNewPieceMoves()
    {
        List<String> newPieceMoves = new List<String>(pieceMoves);
        pieceMoves.Clear();
        return newPieceMoves;
    }
}
