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
        GenerateAllPlayerMoves(blackPlayer);
        GenerateAllPlayerMoves(whitePlayer);
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

        if (currentPlayer == blackPlayer && type.ToString() == "Knight") 
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
    }

    public void EndTurn()
    {
        GenerateAllPlayerMoves(activePlayer);
        GenerateAllPlayerMoves(GetOppositePlayer(activePlayer));
        GameUI TheGameUI = GameObject.Find("UI").GetComponent<GameUI>();
        int iteratorNum = TheGameUI.GetIteratorCount();

        if (GameManager.Instance.State == GameState.PlayerTurn && iteratorNum % 3 == 0)
        {
            GameManager.Instance.UpdateGameState(GameState.EnemyTurn);
        }
        else if (GameManager.Instance.State == GameState.EnemyTurn && iteratorNum % 3 == 0)
        {
            GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
        }
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
        Destroy(piece.gameObject);
    }
}
