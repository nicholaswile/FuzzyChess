using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInfo : MonoBehaviour
{
    public int mapNumber { get; set; }
    public int modeNumber { get; set; }

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

}
