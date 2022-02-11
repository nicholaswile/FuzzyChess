using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    public override List<Vector2Int> FindAvailableSquares()
    {
        AvailableMoves.Clear();
        int reachDistance = 2;

        int[,] radiusMatrix = CreatePieceRadiusMatrix(occupiedSquare, reachDistance);
        AvailableMoves.AddRange(GetSquaresInRange(radiusMatrix, occupiedSquare));

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
