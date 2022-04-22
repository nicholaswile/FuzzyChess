using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MaterialSetter))]
public abstract class Piece : MonoBehaviour
{
    [SerializeField] private MaterialSetter materialSetter;
    public ChessBoard board { protected get; set; }
    public Vector2Int occupiedSquare { get; set; }
    public Team team { get; set; }
    public PieceType pieceType { get; set; }
    public CorpType corpType { get; set; }
    //var to hold the name of the previous corp which a piece belonged to. used for reverting corp.
    //public CorpType prevCorpType { get { return prevCorpType; } set { prevCorpType = value; } }
    public CorpType prevCorpType { get; set; }
    public bool hasMoved { get; private set; }
    private bool delegated = false;
    public bool isDelegated { get { return delegated; } set { delegated = value; } }
    public List<Vector2Int> AvailableMoves;

    static Vector2Int[] directions = new Vector2Int[]
    {
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(-1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(1, -1),
    };

    public abstract List<Vector2Int> FindAvailableSquares();

    private void Awake()
    {
        AvailableMoves = new List<Vector2Int>();
        materialSetter = GetComponent<MaterialSetter>();
        hasMoved = false;
    }

    public int CorpMoveNumber()
    {
        GameController controller = GameObject.Find("Game Controller").GetComponent<GameController>();
        if (corpType == CorpType.Left)
            return controller.LeftCorpUsed;
        else if (corpType == CorpType.King)
            return controller.KingCorpUsed;
        else return controller.RightCorpUsed;
    }

    //reduces the number of the specified piece's corp moves by 1 to allow for another movement.
    public void ReduceCorpMoveNumber()
    {
        GameController controller = GameObject.Find("Game Controller").GetComponent<GameController>();
        if (corpType == CorpType.Left)
            controller.revertCorpMove("Left");
        else if (corpType == CorpType.King)
            controller.revertCorpMove("King");
        else controller.revertCorpMove("Right");
    }

    /**
    //set the value of the previous corpType
    public void SetPrevCorpType(CorpType setCorptype)
    {
        prevCorpType = setCorptype;
    }
    **/

    public bool CommanderMovedOne() 
    {
        if (corpType == CorpType.Left)
            return board.LeftBishopMovedOne;
        else if (corpType == CorpType.King)
            return board.KingMovedOne;
        else return board.RightBishopMovedOne;
    }

    public void SetMaterial(Material material, Material material2) 
    {
        materialSetter.SetPieceMaterials(material, material2);
    }

    //made for corp identification
    public void SetColor()
    {
        materialSetter.ChangePieceColor(this);
    }

    //made for corp identification
    public void RevertColor()
    {
        materialSetter.RevertPieceColor(this);
    }

    public bool IsFromSameTeam(Piece piece) 
    {
        return team == piece.team;
    }

    public bool CanMoveTo(Vector2Int coords) 
    {
        return AvailableMoves.Contains(coords);
    }

    public void MoveTo(Transform transform, Vector3 targetPosition)
    {
        transform.position = targetPosition;

        SFXController.PlaySoundMovement();
    }

    public virtual void MovePiece(Vector2Int coords) 
    {
        Vector3 targetPosition = board.GetPositionFromCoords(coords);
        occupiedSquare = coords;
        hasMoved = true;

        MoveTo(transform, targetPosition);
    }

    //a setter for hasMoved, since its private.
    public void setHasMoved(bool a)
    {
        hasMoved = a;
    }

    protected void TryToAddMove(Vector2Int coords) 
    {
        AvailableMoves.Add(coords);
    }

    public void SetData(Vector2Int coords, Team team, ChessBoard board, PieceType pieceType, CorpType corpType) 
    {
        this.team = team;
        this.pieceType = pieceType;
        this.corpType = corpType;
        occupiedSquare = coords;
        this.board = board;
        transform.position = board.GetPositionFromCoords(coords);
    }


    public int[,] CreatePieceRadiusMatrix(Vector2Int coords, int reachDistance)
    {
        int[,] radiusMatrix = new int[reachDistance * 2 + 1, reachDistance * 2 + 1];
        int matrixCoordsX = 0;
        int matrixCoordsY = reachDistance*2;
        Vector2Int topLeftSquareInRadius = new Vector2Int(coords.x - reachDistance, coords.y + reachDistance);


        for (int y = topLeftSquareInRadius.y; y >= coords.y - reachDistance; y--)
        {
            for (int x = topLeftSquareInRadius.x; x <= coords.x + reachDistance; x++)
            {
                Vector2Int nextCoords = new Vector2Int(x, y);
                Piece piece = board.GetPieceOnSquare(nextCoords);

                if (!board.CheckIfCoordsAreOnBoard(nextCoords)) 
                {
                    radiusMatrix[matrixCoordsX, matrixCoordsY] = 0;
                    matrixCoordsX++;
                    continue;
                }
                if (nextCoords == coords)
                {
                    radiusMatrix[matrixCoordsX, matrixCoordsY] = 1;
                }
                else if (piece == null)
                {
                    radiusMatrix[matrixCoordsX, matrixCoordsY] = 1;
                }
                else if (piece != null)
                {
                    radiusMatrix[matrixCoordsX, matrixCoordsY] = 0;
                }
                matrixCoordsX++;
            }
            matrixCoordsX = 0;
            matrixCoordsY--;
        }
        return radiusMatrix;
    }

    public List<Vector2Int> GetSquaresInRange(int[,] matrix, Vector2Int coords)
    {
        int size = matrix.GetLength(0);
        int reachDistance = (size - 1) / 2;
        List<Vector2Int> squaresInRadius = new List<Vector2Int>();
        List<Vector2Int> closedList = new List<Vector2Int>();
        List<Vector2Int> result = new List<Vector2Int>();
        Vector2Int adjustVector = new Vector2Int(coords.x - reachDistance, coords.y - reachDistance);
        Vector2Int pathStart = new Vector2Int(reachDistance, reachDistance);
        bool[,] visited = new bool[size, size];
        int iterator = 0;
        squaresInRadius.Add(pathStart);
        while (iterator < reachDistance)
        {
            foreach (var square in squaresInRadius)
            {
                foreach (var direction in directions)
                {
                    Vector2Int nextDirection = square + direction;
                    if (matrix[nextDirection.x, nextDirection.y] == 1 && !visited[nextDirection.x, nextDirection.y])
                    {
                        visited[nextDirection.x, nextDirection.y] = true;
                        closedList.Add(nextDirection);
                    }
                }
            }
            squaresInRadius.Clear();
            iterator++;
            squaresInRadius.AddRange(closedList);
        }

        closedList.Remove(pathStart);

        foreach (Vector2Int square in closedList)
        {
            Vector2Int adjustedSquare = square + adjustVector;
            result.Add(adjustedSquare);
        }
        return result;
    }

    public List<Vector2Int> GetAdjacentEnemySquares(Vector2Int coords) 
    {
        List<Vector2Int> adjacentEnemySquares = new List<Vector2Int>();

        foreach (var direction in directions) 
        {
            Vector2Int nextDirection = coords + direction ;
            Piece piece = board.GetPieceOnSquare(nextDirection);
            if (piece == null)
                continue;
            else if (!piece.IsFromSameTeam(this))
            {
                adjacentEnemySquares.Add(nextDirection);
                continue;
            }
        }
        return adjacentEnemySquares;
    }

    public List<Vector2Int> GetAdjacentSquares()
    {
        List<Vector2Int> adjacentSquares = new List<Vector2Int>();

        foreach (var direction in directions)
        {
            Vector2Int nextDirection = occupiedSquare + direction;
            Piece piece = board.GetPieceOnSquare(nextDirection);
            if (!board.CheckIfCoordsAreOnBoard(nextDirection))
                continue;
            else if (piece)
                continue;
            else if (piece == null)
            {
                adjacentSquares.Add(nextDirection);
                continue;
            }
        }
        return adjacentSquares;
    }

    public bool HasAdjacentEnemySquares(Vector2Int coords) 
    {
        List<Vector2Int>  adjacentEnemySquares = GetAdjacentEnemySquares(coords);
        if (adjacentEnemySquares.Count == 0)
            return false;
        else return true;
    }

}
