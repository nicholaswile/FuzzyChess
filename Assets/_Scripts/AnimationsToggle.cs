using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationsToggle : MonoBehaviour
{
    public Toggle toggle;

    public void ToggleUpdated()
    {
        if(toggle.isOn)
        {
            PlayerPrefs.SetInt("AnimationsEnabled", 1);
        } else {
            PlayerPrefs.SetInt("AnimationsEnabled", 0);
        }
        SFXController.PlaySoundMenuButton();
    }
    
    void Start()
    {
        toggle.isOn = (PlayerPrefs.GetInt("AnimationsEnabled") != 0);
    }
}
