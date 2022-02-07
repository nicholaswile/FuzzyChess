using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    public override List<Vector2Int> FindAvailableSquares()
    {
        AvailableMoves.Clear();
        int reachDistance = 3;
        int[,] radiusMatrix = CreatePieceRadiusMatrix(occupiedSquare, reachDistance);
        AvailableMoves.AddRange(GetSquaresInRange(radiusMatrix, occupiedSquare));
        AvailableMoves.AddRange(GetAdjacentEnemySquares(occupiedSquare));

        return AvailableMoves;
    }
}
