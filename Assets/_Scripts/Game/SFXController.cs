using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    static float volume = 0.5f;

    public static void ChangeVolume(float changeVolume)
    {
        volume = changeVolume;
    }

    public static void PlaySoundMovement()
    {
        GameObject soundObject = new GameObject("SFX Object");
        AudioClip clipPieceMovement = Resources.Load<AudioClip> ("Audio/PieceMove");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(clipPieceMovement, volume);
    }
    public static void PlaySoundCapture()
    {
        GameObject soundObject = new GameObject("SFX Object");
        AudioClip clipPieceCapture = Resources.Load<AudioClip> ("Audio/PieceCapture");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(clipPieceCapture, volume);
    }
    public static void PlaySoundDiceRoll()
    {
        GameObject soundObject = new GameObject("SFX Object");
        AudioClip clipDiceRoll = Resources.Load<AudioClip> ("Audio/DiceRoll");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(clipDiceRoll, volume);
    }
    public static void PlaySoundGameWon()
    {
        GameObject soundObject = new GameObject("SFX Object");
        AudioClip clipGameWon = Resources.Load<AudioClip> ("Audio/GameWon");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(clipGameWon, volume);
    }
    public static void PlaySoundGameLost()
    {
        GameObject soundObject = new GameObject("SFX Object");
        AudioClip clipGameLost = Resources.Load<AudioClip> ("Audio/GameLost");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(clipGameLost, volume);
    }
    
}
