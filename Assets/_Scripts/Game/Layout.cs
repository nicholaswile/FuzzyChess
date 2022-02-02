using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layout : ScriptableObject
{
    [System.Serializable]
    private class BoardSetup
    {
        public Vector2Int position;
        public PieceType pieceType;
        public Team teamColor;
    }

    [SerializeField] private BoardSetup[] boardSquares;

    public int GetNumberOfPieces() 
    {
        return boardSquares.Length;
    }
    
    public Vector2Int GetCoordsAtIndex(int index)
    {
        if (boardSquares.Length <= index) 
        {
            Debug.LogError("PIECE NOT IN RANGE");
            return new Vector2Int(-1, -1);
        }
        return new Vector2Int(boardSquares[index].position.x - 1, boardSquares[index].position.y - 1);
    }

    public string GetPieceTypeAtIndex(int index)
    {
        if (boardSquares.Length <= index)
        {
            Debug.LogError("PIECE NOT IN RANGE");
            return "";
        }
        return boardSquares[index].pieceType.ToString();
    }

    public Team GetTeamColorAtIndex(int index)
    {
        if (boardSquares.Length <= index)
        {
            Debug.LogError("PIECE NOT IN RANGE");
            return Team.Black;
        }
        return boardSquares[index].teamColor;
    }

}
