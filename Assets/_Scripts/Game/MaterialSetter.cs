using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]

public class MaterialSetter : MonoBehaviour
{
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
        meshRenderer.materials[1].color = Color.blue;
    }

    //made for corp identification
    public void RevertPieceColor()
    {
        meshRenderer.materials[1].color = Color.white;
    }

    public void SetAnyMaterial(Material material)
    {
        meshRenderer.material = material;
    }

}
