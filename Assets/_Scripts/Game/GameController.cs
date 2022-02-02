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

    private void GameManager_StateChanged(GameState obj)
    {
        ChangeActiveTeam();
    }

    private void SetDependencies()
    {
        pieceCreator = GetComponent<CreatePieces>();
    }

    private void CreatePlayers()
    {
        whitePlayer = new Player(Team.White, board);
        blackPlayer = new Player(Team.Black, board);
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
        GenerateAllPlayerMoves(activePlayer);
    }

    private void CreatePiecesFromLayout(Layout layout)
    {
        for (int i = 0; i < layout.GetNumberOfPieces(); i++)
        {
            Vector2Int squareCoords = layout.GetCoordsAtIndex(i);
            Team team = layout.GetTeamColorAtIndex(i);
            string typeName = layout.GetPieceTypeAtIndex(i);
            Type type = Type.GetType(typeName);

            CreatePieceAndInitialize(squareCoords, team, type);
        }
    }

    private void CreatePieceAndInitialize(Vector2Int squareCoords, Team team, Type type)
    {
        Piece newPiece = pieceCreator.CreatePiece(type).GetComponent<Piece>();
        newPiece.SetData(squareCoords, team, board);

        Material teamMaterial = pieceCreator.GetTeamMaterial(team);
        Material bottomMaterial = pieceCreator.GetBottomMaterial();
        newPiece.SetMaterial(teamMaterial, bottomMaterial);

        board.SetPieceOnBoard(squareCoords, newPiece);

        Player currentPlayer = team == Team.White ? whitePlayer : blackPlayer;
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
    }

    public void EndTurn()
    {
        GenerateAllPlayerMoves(activePlayer);
        GenerateAllPlayerMoves(GetOppositePlayer(activePlayer));
        if (GameManager.Instance.State == GameState.PlayerTurn)
        {
            GameManager.Instance.UpdateGameState(GameState.EnemyTurn);
        }
        else if (GameManager.Instance.State == GameState.EnemyTurn)
        {
            GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
        }
        //ChangeActiveTeam();
    }

    private Player GetOppositePlayer(Player player)
    {
        return player == whitePlayer ? blackPlayer : whitePlayer;

    }


    public void OnPieceRemoved(Piece piece)
    {
        Player pieceOwner = (piece.team == Team.White) ? whitePlayer : blackPlayer;
        pieceOwner.RemovePiece(piece);
        Destroy(piece.gameObject);
    }
}
