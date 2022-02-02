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
        float reachDistance = 1;
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
                else if (!piece.IsFromSameTeam(this))
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
