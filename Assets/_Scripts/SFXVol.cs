using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SFXVol : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider slider;
    
    public void SetLevel (float sliderValue)
    {
	    mixer.SetFloat("SFXVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
    }

    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
    }
}
