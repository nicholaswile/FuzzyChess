using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePieces : MonoBehaviour
{
    [SerializeField] private GameObject[] piecesPrefabs;
    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteMaterial;
    [SerializeField] private Material bottomMaterial;

    private Dictionary<string, GameObject> nameToPieceDict = new Dictionary<string, GameObject>();

    private void Awake()
    {
        foreach (var piece in piecesPrefabs) 
        {
            nameToPieceDict.Add(piece.GetComponent<Piece>().GetType().ToString(), piece);
        }
    }

    public GameObject CreatePiece(Type type) 
    {
        GameObject prefab = nameToPieceDict[type.ToString()];
        if (prefab) 
        {
            GameObject newPiece = Instantiate(prefab);
            return newPiece;
        }
        return null;
    }

    public Material GetTeamMaterial(Team team) 
    {
        return team == Team.White ? whiteMaterial : blackMaterial;
    }

    public Material GetBottomMaterial() 
    {
        return bottomMaterial;
    }
}
