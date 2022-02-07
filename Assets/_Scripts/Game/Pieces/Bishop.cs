using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
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

        // The code that is commented below allows the bishop to move 3 squares WITHOUT having to move in a straight line
        // **If you want to use this code you must comment out the foreach loop below it**

        //int[,] radiusMatrix = CreatePieceRadiusMatrix(occupiedSquare, reachDistance);
        //AvailableMoves.AddRange(GetSquaresInRange(radiusMatrix, occupiedSquare));
        //AvailableMoves.AddRange(GetAdjacentEnemySquares(occupiedSquare));


        // The code below allows the bishop to move 3 squares WITH having to move in a straight line
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
                else if (!piece.IsFromSameTeam(this) && i == 1) //Capturing, must be adjacent
                {
                    TryToAddMove(nextCoords);
                    break;
                }
                else if (piece.IsFromSameTeam(this))
                    break;
            }
        }

        return AvailableMoves;
    }
}
