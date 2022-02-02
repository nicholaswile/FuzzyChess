using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateHighlighters : MonoBehaviour
{
    [SerializeField] private Material freeHighlightMaterial;
    [SerializeField] private Material opponentHighlightMaterial;
    [SerializeField] private GameObject highlighterPrefab;
    private List<GameObject> instantiatedHighlighters = new List<GameObject>();

    public void ShowAvailableMoves(Dictionary<Vector3, bool> squareData) 
    {
        ClearMoves();
        foreach (var data in squareData) 
        {
            GameObject selector = Instantiate(highlighterPrefab, data.Key, Quaternion.identity);
            instantiatedHighlighters.Add(selector);
            foreach (var setter in selector.GetComponentsInChildren<MaterialSetter>()) 
            {
                setter.SetAnyMaterial(data.Value ? freeHighlightMaterial : opponentHighlightMaterial);
            }
        }
    }

    public void ClearMoves()
    {
        foreach (var selector in instantiatedHighlighters) 
        {
            Destroy(selector.gameObject);
        }
    }
}
