using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    public static void PlaySoundMovement()
    {
        GameObject soundObject = new GameObject("SFX Object");

        AudioClip clipPieceMovement = Resources.Load<AudioClip> ("Audio/SoundPieceMovement");

        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(clipPieceMovement);
    }
    public static void PlaySoundCapture()
    {
        GameObject soundObject = new GameObject("SFX Object");

        AudioClip clipPieceCapture = Resources.Load<AudioClip> ("Audio/SoundPieceCapture");

        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(clipPieceCapture);
    }
}
