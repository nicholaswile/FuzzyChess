using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour
{ 
    AudioSource audioSource;
    AudioClip clipMusic;

    // Start is called before the first frame update
    void Start()
    {
        clipMusic = Resources.Load<AudioClip> ("Audio/Chopin Fsharp Adagio Strings");
        audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.PlayOneShot(clipMusic, 0.02f);
    }
}
