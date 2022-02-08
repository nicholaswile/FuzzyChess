using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Corps : MonoBehaviour
{
    //Keep track of the units in each corp currently, these lists are used to track the number of units in each corps (6 commanded + 1 commander maximum)
    public List<Piece> corp_BlackLeft { get; private set; }
    public List<Piece> corp_BlackKing { get; private set; }
    public List<Piece> corp_BlackRight { get; private set; }

    public List<Piece> corp_WhiteLeft { get; private set; }
    public List<Piece> corp_WhiteKing { get; private set; }
    public List<Piece> corp_WhiteRight { get; private set; }

    //Each corp can move one of it's pieces once per turn
    //The king can delegate one of his units into another unit or recall all of the delegations he's made instead once per turn
    //These booleans will be used by the game state manager to determine what actions can be performed and when a turn is over
    public bool corpMoved_BlackLeft = false;
    public bool corpMoved_BlackKing = false;
    public bool corpMoved_BlackRight = false;

    public bool corpCommand_BlackLeft = false;
    public bool corpCommand_BlackKing = false;
    public bool corpCommand_BlackRight = false;
    public bool corpDelegated_Black = false;


    public bool corpMoved_WhiteLeft = false;
    public bool corpMoved_WhiteKing = false;
    public bool corpMoved_WhiteRight = false;

    public bool corpCommand_WhiteLeft = false;
    public bool corpCommand_WhiteKing = false;
    public bool corpCommand_WhiteRight = false;
    public bool corpDelegated_White = false;


    // !!!!!! Function to assign corps when pieces are instantiated in piece file -- CreatePieces.CreatePiece()!!!!!!!
    //        Need help to figure out what's happening in this class and how to call this function upon the creation of each piece

    //LEFT: 3 left Pawns and Knight
    //KING: Queen, 2 Rooks, and 2 center Pawns
    //RIGHT: 3 right Pawns Knight
    public void AssignCorpAtGameStart(Piece piece, CorpType corpAssignment)
    {
        if (corpAssignment == CorpType.Left)
        {
            piece.corpOriginal = CorpType.Left;
            piece.corpCurrent = CorpType.Left;
        }
        else if (corpAssignment == CorpType.King)
        {
            piece.corpOriginal = CorpType.King;
            piece.corpCurrent = CorpType.King;
        }
        else if (corpAssignment == CorpType.Right)
        {
            piece.corpOriginal = CorpType.Right;
            piece.corpCurrent = CorpType.Right;
        }
    }

    //King can assign pieces to new corps once per turn, the new corp can't have six units already
    //Checks if it's a valid operation, checks if the left or right corp is at capacity, if all flags are green then the swap between corps is performed
    public void CorpDelegate(Piece piece, CorpType changeCorpTo)
    {
        if (piece.corpCurrent == changeCorpTo)
        {
            print("Invalid change, the unit is in that corp already!");
            return;
        }
        if (piece.team == Team.Black)
        {
            if (changeCorpTo == CorpType.Left)
            {
                if (corp_BlackLeft.Count < 6)
                {
                    if (piece.corpCurrent == CorpType.Right)
                    {
                        corp_BlackRight.Remove(piece);
                    }
                    else if (piece.corpCurrent == CorpType.King)
                    {
                        corp_BlackKing.Remove(piece);
                    }
                    piece.corpCurrent = CorpType.Left;
                    corp_BlackLeft.Add(piece);
                    corpDelegated_Black = true;
                }
                else
                {
                    print("Invalid change, corp size is already at capacity! [" + corp_BlackLeft.Count + "]");
                }
            }
            else if (changeCorpTo == CorpType.Right)
            {
                if (corp_BlackRight.Count < 6)
                {
                    if (piece.corpCurrent == CorpType.Left)
                    {
                        corp_BlackLeft.Remove(piece);
                    }
                    else if (piece.corpCurrent == CorpType.King)
                    {
                        corp_BlackKing.Remove(piece);
                    }
                    piece.corpCurrent = CorpType.Left;
                    corp_BlackRight.Add(piece);
                    corpDelegated_Black = true;
                }
                else
                {
                    print("Invalid change, corp size is already at capacity! [" + corp_BlackRight.Count + "]");
                }
            }
        }
        else if (piece.team == Team.White)
        {
            if (changeCorpTo == CorpType.Left)
            {
                if (corp_WhiteLeft.Count < 6)
                {
                    if (piece.corpCurrent == CorpType.Right)
                    {
                        corp_WhiteRight.Remove(piece);
                    }
                    else if (piece.corpCurrent == CorpType.King)
                    {
                        corp_WhiteKing.Remove(piece);
                    }
                    piece.corpCurrent = CorpType.Left;
                    corp_WhiteLeft.Add(piece);
                    corpDelegated_White = true;
                }
                else
                {
                    print("Invalid change, corp size is already at capacity! [" + corp_WhiteLeft.Count + "]");
                }
            }
            else if (changeCorpTo == CorpType.Right)
            {
                if (corp_WhiteRight.Count < 6)
                {
                    if (piece.corpCurrent == CorpType.Left)
                    {
                        corp_WhiteLeft.Remove(piece);
                    }
                    else if (piece.corpCurrent == CorpType.King)
                    {
                        corp_WhiteKing.Remove(piece);
                    }
                    piece.corpCurrent = CorpType.Left;
                    corp_WhiteRight.Add(piece);
                    corpDelegated_White = true;
                }
                else
                {
                    print("Invalid change, corp size is already at capacity! [" + corp_WhiteRight.Count + "]");
                }
            }
        }
    }

    //I'm bad with C# iterators HELP

    //All of the pieces that started in the King's corp return to his corp
    //pass in a piece from a team to identify which color king is recalling its pieces
    /*public void CorpRecall(Piece piece)
    {
        if(piece.team == Team.Black)
        {
            foreach (Piece piece in corp_BlackLeft)
            {
                if (piece.corpOriginal != piece.corpCurrent)
                {
                    corp_BlackLeft.Remove(piece);
                    piece.corpCurrent = CorpType.King;
                    corp_BlackKing.Add(piece);
                    print(piece + " recalled from Black Left corp to King corp!");
                }
            }
            foreach (Piece piece in corp_BlackRight)
            {
                if (piece.corpOriginal != piece.corpCurrent)
                {
                    corp_BlackRight.Remove(piece);
                    piece.corpCurrent = CorpType.King;
                    corp_BlackKing.Add(piece);
                    print(piece + " recalled from Black Right corp to King corp!");
                }
            }
            corpDelegated_Black = true;
        } else if (piece.team == Team.White)
        {
            foreach (Piece piece in corp_WhiteLeft)
            {
                if (piece.corpOriginal != piece.corpCurrent)
                {
                    corp_WhiteLeft.Remove(piece);
                    piece.corpCurrent = CorpType.King;
                    corp_WhiteKing.Add(piece);
                    print(piece + " recalled from White Left corp to King corp!");
                }
            }
            foreach (Piece piece in corp_WhiteRight)
            {
                if (piece.corpOriginal != piece.corpCurrent)
                {
                    corp_WhiteRight.Remove(piece);
                    piece.corpCurrent = CorpType.King;
                    corp_WhiteKing.Add(piece);
                    print(piece + " recalled from White Right corp to King corp!");
                }
            }
            corpDelegated_White = true;
        }
    }*/

    //When a bishop is captured, its pieces fall under the control of the king
    public void BishopDeath(Piece piece)
    {

    }

    //The king can move 3 spaces if it does not capture a piece that turn (in addition to commanding one of it's unit to attack)
    //The bishop can move 2 spaces if it does not capture a piece that turn (in addition to commanding one of it's unit to attack)
    public void CommanderMovement(Piece piece)
    {

    }

    /*//Use game state manager instead of calling updates every frame
    void Update()
    {
        
    }*/
}
