using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{ 
    AudioSource audioSource;
    AudioClip clipMusic;
    float volume = 0.02f;

    public void ChangeVolume(float changeVolume)
    {
        volume = changeVolume;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        clipMusic = Resources.Load<AudioClip> ("Audio/Chopin Fsharp Adagio Strings");
        audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.PlayOneShot(clipMusic, volume);
    }
}
