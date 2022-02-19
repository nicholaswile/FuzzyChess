using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CreatePieces))]
public class GameController : MonoBehaviour
{
    [SerializeField] private Layout layoutAtStart;
    [SerializeField] private ChessBoard board;
    private CreatePieces pieceCreator;
    private Player whitePlayer;
    private Player blackPlayer;
    private Player activePlayer;
    private bool leftCorpUsed = false, kingCorpUsed = false, rightCorpUsed = false;
    private int killCount = 0;

    private void Awake()
    {
        SetDependencies();
        CreatePlayers();

        GameManager.StateChanged += GameManager_StateChanged;
    }

    private void OnDestroy()
    {
        GameManager.StateChanged -= GameManager_StateChanged;
    }

    private void GameManager_StateChanged(GameState state)
    {
        if (state == GameState.PlayerTurn || state == GameState.EnemyTurn)
        {
            ChangeActiveTeam();
        }
    }

    private void SetDependencies()
    {
        pieceCreator = GetComponent<CreatePieces>();
    }

    private void CreatePlayers()
    {
        whitePlayer = new Player(Team.White, board, CorpType.King);
        blackPlayer = new Player(Team.Black, board, CorpType.King);
    }

    private void Start()
    {
        StartNewGame();
    }

    private void StartNewGame()
    {
        board.SetDependencies(this);
        CreatePiecesFromLayout(layoutAtStart);
        activePlayer = whitePlayer;
        GenerateAllPlayerMoves(blackPlayer);
        GenerateAllPlayerMoves(whitePlayer);
    }

    private void CreatePiecesFromLayout(Layout layout)
    {
        for (int i = 0; i < layout.GetNumberOfPieces(); i++)
        {
            Vector2Int squareCoords = layout.GetCoordsAtIndex(i);
            Team team = layout.GetTeamColorAtIndex(i);
            string typeName = layout.GetPieceTypeStringAtIndex(i);
            PieceType pieceType = layout.GetPieceTypeAtIndex(i);
            CorpType corpType = layout.GetCorpTypeAtIndex(i);
            Type type = Type.GetType(typeName);

            CreatePieceAndInitialize(squareCoords, team, type, pieceType, corpType);
        }
    }

    private void CreatePieceAndInitialize(Vector2Int squareCoords, Team team, Type type, PieceType pieceType, CorpType corpType)
    {
        Piece newPiece = pieceCreator.CreatePiece(type).GetComponent<Piece>();
        newPiece.SetData(squareCoords, team, board, pieceType, corpType);

        Material teamMaterial = pieceCreator.GetTeamMaterial(team);
        Material bottomMaterial = pieceCreator.GetBottomMaterial();
        newPiece.SetMaterial(teamMaterial, bottomMaterial);

        board.SetPieceOnBoard(squareCoords, newPiece);

        Player currentPlayer = team == Team.White ? whitePlayer : blackPlayer;

        if (currentPlayer == blackPlayer && pieceType == PieceType.Knight) 
        {
            newPiece.transform.Rotate(0.0f, 0.0f, 180.0f, Space.Self);
        }
        
        currentPlayer.AddPiece(newPiece);
    }

    private void GenerateAllPlayerMoves(Player player)
    {
        player.GenerateAllMoves();
    }

    public bool IsTeamTurnActive(Team team)
    {
        return activePlayer.team == team;
    }

    private void ChangeActiveTeam()
    {
        activePlayer = activePlayer == whitePlayer ? blackPlayer : whitePlayer;
        board.DeselectPiece();
        killCount = 0;
    }
    public bool IsCorpTurnActive(CorpType currentCorp)
    {
        return activePlayer.currentCorp == currentCorp;
    }
    private void ChangeActiveCorp()
    {
        //Marks the corp that is used
        if (activePlayer.currentCorp == CorpType.Left)
            leftCorpUsed = true;
        else if (activePlayer.currentCorp == CorpType.King)
            kingCorpUsed = true;
        else if (activePlayer.currentCorp == CorpType.Right)
            rightCorpUsed = true;

        //Assigns the next new open corp
        if (!leftCorpUsed)
            activePlayer.currentCorp = CorpType.Left;
        else if (!kingCorpUsed)
            activePlayer.currentCorp = CorpType.King;
        else if (!rightCorpUsed)
            activePlayer.currentCorp = CorpType.Right;
    }
    public void TryToChangeActiveCorp(CorpType corpType)
    {
        //attempts to change active corp when a new corp is clicked
        if (corpType == CorpType.Left && !leftCorpUsed)
            activePlayer.currentCorp = CorpType.Left;
        else if (corpType == CorpType.King && !kingCorpUsed)
            activePlayer.currentCorp = CorpType.King;
        else if (corpType == CorpType.Right && !rightCorpUsed)
            activePlayer.currentCorp = CorpType.Right;
    }

    public void EndTurn()
    {
        GenerateAllPlayerMoves(activePlayer);
        GenerateAllPlayerMoves(GetOppositePlayer(activePlayer));
        GameUI TheGameUI = GameObject.Find("UI").GetComponent<GameUI>();
        int iteratorNum = TheGameUI.GetIteratorCount();
        ChangeActiveCorp();
        Debug.Log("LeftCorpUsed: " + leftCorpUsed);
        Debug.Log("KingCorpUsed: " + kingCorpUsed);
        Debug.Log("RightCorpUsed: " + rightCorpUsed);

        if (GameManager.Instance.State == GameState.PlayerTurn && iteratorNum % 3 == 0)
        {
            GameManager.Instance.UpdateGameState(GameState.EnemyTurn);
            OpenCorpSelection();
        }
        else if (GameManager.Instance.State == GameState.EnemyTurn && iteratorNum % 3 == 0)
        {
            GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
            OpenCorpSelection();
        }
    }

    public void OpenCorpSelection()
    {
        leftCorpUsed = false;
        kingCorpUsed = false;
        rightCorpUsed = false;
    }

    private Player GetOppositePlayer(Player player)
    {
        return player == whitePlayer ? blackPlayer : whitePlayer;
    }

    public void OnPieceRemoved(Piece piece)
    {
        Player pieceOwner = (piece.team == Team.White) ? whitePlayer : blackPlayer;
        if (piece.GetComponent<King>()!=null)
        {
            Debug.Log("<color=red>King captured</color>");
            if (piece.team == Team.White)
            {
                GameManager.Instance.UpdateGameState(GameState.Lose);
            }
            else if (piece.team == Team.Black) {
                GameManager.Instance.UpdateGameState(GameState.Win);
            }
        }

        pieceOwner.RemovePiece(piece);
        killCount++;

        int capturedPieces = 15 - pieceOwner.ActivePieces.Count;
        if (piece.team == Team.White)
            piece.MovePiece(new Vector2Int(-2 - (capturedPieces % 3), 7 - capturedPieces / 3));
        else
            piece.MovePiece(new Vector2Int(9 + (capturedPieces % 3), capturedPieces / 3));
    }
}
