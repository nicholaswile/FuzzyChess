using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxLoader : MonoBehaviour
{
    public Material skyClouds;
    public Material skyGalaxy;
    public Material skyMars;

    private MenuInfo mapChoice;
    int skyNumber;

    void Start()
    {
        mapChoice = FindObjectsOfType<MenuInfo>()[FindObjectsOfType<MenuInfo>().Length - 1];
        skyNumber = mapChoice.mapNumber;


        if (skyNumber == 1)
            RenderSettings.skybox = skyClouds;
        else if (skyNumber == 2)
            RenderSettings.skybox = skyGalaxy;
        else if (skyNumber == 3)
            RenderSettings.skybox = skyMars;
    }
}
