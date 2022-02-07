using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public override List<Vector2Int> FindAvailableSquares()
    {
        AvailableMoves.Clear();
        int reachDistance = 4;
        int[,] radiusMatrix = CreatePieceRadiusMatrix(occupiedSquare, reachDistance);
        AvailableMoves.AddRange(GetSquaresInRange(radiusMatrix, occupiedSquare));
        AvailableMoves.AddRange(GetAdjacentEnemySquares(occupiedSquare));

        //Adam - Needs the ability to combine movement with a capture in the same action
        //Knight receives a +1 to the die roll if moving and then attempting a capture.

        return AvailableMoves;
    }
}
