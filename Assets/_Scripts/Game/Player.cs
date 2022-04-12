using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Player 
{
    public List<Piece> ActivePieces { get; private set; }
    public List<Piece> LeftCorpPieces { get; set; }
    public List<Piece> KingCorpPieces { get; set; }
    public List<Piece> RightCorpPieces { get; set; }
    public CorpType currentCorp { get; set; }
    public Team team { get; set; }
    public ChessBoard board { get; set; }
    private bool leftBishopIsDead = false, rightBishopIsDead = false;
    public bool LeftBishopIsDead { get { return leftBishopIsDead; } set { leftBishopIsDead = value; } }
    public bool RightBishopIsDead { get { return rightBishopIsDead; } set { rightBishopIsDead = value; } }
    public Player(Team team, ChessBoard board, CorpType currentCorp) 
    {
        ActivePieces = new List<Piece>();
        this.team = team;
        this.board = board;
        this.currentCorp = currentCorp;

        LeftCorpPieces = new List<Piece>();
        KingCorpPieces = new List<Piece>();
        RightCorpPieces = new List<Piece>();
    }

    public void AddPiece(Piece piece) 
    {
        if (!ActivePieces.Contains(piece))
            ActivePieces.Add(piece);

        if (!LeftCorpPieces.Contains(piece) && piece.corpType == CorpType.Left)
            LeftCorpPieces.Add(piece);
        else if (!KingCorpPieces.Contains(piece) && piece.corpType == CorpType.King)
            KingCorpPieces.Add(piece);
        else if (!RightCorpPieces.Contains(piece) && piece.corpType == CorpType.Right)
            RightCorpPieces.Add(piece);
    }

    public void RemovePiece(Piece piece) 
    {
        if (ActivePieces.Contains(piece))
            ActivePieces.Remove(piece);

        if (LeftCorpPieces.Contains(piece) && piece.corpType == CorpType.Left)
            LeftCorpPieces.Remove(piece);
        else if (KingCorpPieces.Contains(piece) && piece.corpType == CorpType.King)
            KingCorpPieces.Remove(piece);
        else if (RightCorpPieces.Contains(piece) && piece.corpType == CorpType.Right)
            RightCorpPieces.Remove(piece);
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
