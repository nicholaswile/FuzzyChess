using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Player 
{
    public List<Piece> ActivePieces { get; private set; }
    public Team team { get; set; }
    public ChessBoard board { get; set; }

    public Player(Team team, ChessBoard board) 
    {
        ActivePieces = new List<Piece>();
        this.team = team;
        this.board = board;
    }

    public void AddPiece(Piece piece) 
    {
        if (!ActivePieces.Contains(piece))
            ActivePieces.Add(piece);
    }

    public void RemovePiece(Piece piece) 
    {
        if (ActivePieces.Contains(piece))
            ActivePieces.Remove(piece);
    }

    public void GenerateAllMoves() 
    {
        foreach (var piece in ActivePieces) 
        {
            if (board.HasPiece(piece))
                piece.FindAvailableSquares();
        }
    }
}
