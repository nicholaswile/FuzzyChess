using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MaterialSetter))]
public abstract class Piece : MonoBehaviour
{
    [SerializeField] private MaterialSetter materialSetter;
    public ChessBoard board { protected get; set; }
    public Vector2Int occupiedSquare { get; set; }
    public Team team { get; set; }
    public bool hasMoved { get; private set; }
    public List<Vector2Int> AvailableMoves;


    public abstract List<Vector2Int> FindAvailableSquares();

    private void Awake()
    {
        AvailableMoves = new List<Vector2Int>();
        materialSetter = GetComponent<MaterialSetter>();
        hasMoved = false;
    }

    public void SetMaterial(Material material, Material material2) 
    {
        materialSetter.SetPieceMaterials(material, material2);
    }

    public bool IsFromSameTeam(Piece piece) 
    {
        return team == piece.team;
    }

    public bool CanMoveTo(Vector2Int coords) 
    {
        return AvailableMoves.Contains(coords);
    }

    public void MoveTo(Transform transform, Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    public virtual void MovePiece(Vector2Int coords) 
    {
        Vector3 targetPosition = board.GetPositionFromCoords(coords);
        occupiedSquare = coords;
        hasMoved = true;

        MoveTo(transform, targetPosition);
    }

    protected void TryToAddMove(Vector2Int coords) 
    {
        AvailableMoves.Add(coords);
    }

    public void SetData(Vector2Int coords, Team team, ChessBoard board) 
    {
        this.team = team;
        occupiedSquare = coords;
        this.board = board;
        transform.position = board.GetPositionFromCoords(coords);
    }
}
