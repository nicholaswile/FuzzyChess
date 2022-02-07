using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    Vector2Int[] directions = new Vector2Int[]
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
    public override List<Vector2Int> FindAvailableSquares()
    {
        AvailableMoves.Clear();
        int reachDistance = 2;

        // The code that is commented below allows the ROOK to move 2 squares WITHOUT having to move in a straight line
        // **If you want to use this code you must comment out the foreach loop below it**

        //int[,] radiusMatrix = CreatePieceRadiusMatrix(occupiedSquare, reachDistance);
        //AvailableMoves.AddRange(GetSquaresInRange(radiusMatrix, occupiedSquare));

        // The code below allows the ROOK to move 2 squares WITH having to move in a straight line
        foreach (var direction in directions)
        {
            for (int i = 1; i <= reachDistance; i++)
            {
                Vector2Int nextCoords = occupiedSquare + direction * i;
                Piece piece = board.GetPieceOnSquare(nextCoords);
                if (!board.CheckIfCoordsAreOnBoard(nextCoords))
                    break;
                if (piece == null)
                    TryToAddMove(nextCoords);
                else
                    break;
            }
        }

        //Capturing, can shoot up to two squares in any direction over other units. Two squares does not count Rook or enemy.
        for (int x = -3; x <= 3; x++)
        {
            for (int y = -3; y <= 3; y++)
            {
                Vector2Int nextCoords = occupiedSquare + new Vector2Int(x, y);
                Piece piece = board.GetPieceOnSquare(nextCoords);
                if (!board.CheckIfCoordsAreOnBoard(nextCoords) || piece == null || nextCoords == occupiedSquare)
                    continue; //If the selected square is off the board or not on a piece or is selecting the rook
                if (!piece.IsFromSameTeam(this))
                    TryToAddMove(nextCoords);
            }
        }

        return AvailableMoves;
    }
}
