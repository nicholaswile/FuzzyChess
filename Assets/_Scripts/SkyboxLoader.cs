using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxLoader : MonoBehaviour
{
    public Material skyClouds;
    public Material skyGalaxy;
    public Material skyMars;

    [SerializeField] private Material brick, moon, mars;

    [SerializeField] private GameObject chessBoard;
    private MeshRenderer textureRenderer;

    private MenuInfo mapChoice;
    int skyNumber;

    [SerializeField] private Camera[] cameras;

    private void Awake()
    {
        textureRenderer = chessBoard.GetComponent<MeshRenderer>();
    }

    void Start()
    {
        mapChoice = FindObjectsOfType<MenuInfo>()[FindObjectsOfType<MenuInfo>().Length - 1];
        skyNumber = mapChoice.mapNumber;

        if (skyNumber == 0)
        {
            foreach(Camera cam in cameras)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
            }
            Debug.Log("Default");
        }
        else if (skyNumber == 1)
        {
            RenderSettings.skybox = skyClouds;
            SetTexture(brick);
        }
        else if (skyNumber == 2)
        {
            RenderSettings.skybox = skyGalaxy;
            SetTexture(moon);
        }
        else if (skyNumber == 3)
        {
            RenderSettings.skybox = skyMars;
            SetTexture(mars);
        }
    }

    private void SetTexture(Material material)
    {
        var materials = textureRenderer.materials;

        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].name == "Black Squares (Instance)")
            {
                materials[i] = material;
                // Tiles the board so that it is correctly 8x8 and not 32x32
                materials[i].mainTextureScale = new Vector2(.25f, .25f);
            }
            else if (materials[i].name == "White Squares (Instance)")
            {
                materials[i] = material;
            }
            else { continue; }

            materials[i] = material;
        }

        textureRenderer.materials = materials;
    }
}
