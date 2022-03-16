using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]

public class MaterialSetter : MonoBehaviour
{
    private Color BlackColor = new Color(0.4056604f, 0.08824264f, 0.009567461f, 1f);
    private Color WhiteColor = Color.white;
    private Color CorpColor = new Color(.6f, .6f, .6f, 1f);
    private MeshRenderer _meshRenderer;
    private MeshRenderer meshRenderer
    {
        get 
        {
            if (_meshRenderer == null)
                _meshRenderer = GetComponent<MeshRenderer>();
            return _meshRenderer;    
        }
    }

    public void SetPieceMaterials(Material material, Material material2) 
    {
        Material[] myMaterials = new Material[] { material2, material };
        meshRenderer.materials = myMaterials;
    }

    //made for corp identification
    public void ChangePieceColor()
    {
        meshRenderer.materials[1].color = CorpColor;
    }

    //made for corp identification
    public void RevertPieceColor(Piece piece)
    {
        if (piece.team == Team.White)
            meshRenderer.materials[1].color = WhiteColor;
        else
            meshRenderer.materials[1].color = BlackColor;
    }

    public void SetAnyMaterial(Material material)
    {
        meshRenderer.material = material;
    }

}
