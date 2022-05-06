using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicVol : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider slider;
    
    public void SetLevel (float sliderValue)
    {
	    mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }

    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
    }
}
