using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXController : MonoBehaviour
{
    public static void PlaySoundMovement()
    {
        AudioMixer audioMixer = Resources.Load<AudioMixer>("Master");
        AudioMixerGroup[] audioMixGroup = audioMixer.FindMatchingGroups("SFX");
        GameObject soundObject = new GameObject("SFX Object");
        AudioClip clipPieceMovement = Resources.Load<AudioClip> ("Audio/PieceMove");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixGroup[0];
        audioSource.PlayOneShot(clipPieceMovement);
    }
    public static void PlaySoundCapture()
    {
        AudioMixer audioMixer = Resources.Load<AudioMixer>("Master");
        AudioMixerGroup[] audioMixGroup = audioMixer.FindMatchingGroups("SFX");
        GameObject soundObject = new GameObject("SFX Object");
        AudioClip clipPieceCapture = Resources.Load<AudioClip> ("Audio/PieceCapture");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixGroup[0];
        audioSource.PlayOneShot(clipPieceCapture);
    }
    public static void PlaySoundDiceRoll()
    {
        AudioMixer audioMixer = Resources.Load<AudioMixer>("Master");
        AudioMixerGroup[] audioMixGroup = audioMixer.FindMatchingGroups("SFX");
        GameObject soundObject = new GameObject("SFX Object");
        AudioClip clipDiceRoll = Resources.Load<AudioClip> ("Audio/DiceRoll");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixGroup[0];
        audioSource.PlayOneShot(clipDiceRoll);
    }
    public static void PlaySoundGameWon()
    {
        AudioMixer audioMixer = Resources.Load<AudioMixer>("Master");
        AudioMixerGroup[] audioMixGroup = audioMixer.FindMatchingGroups("SFX");
        GameObject soundObject = new GameObject("SFX Object");
        AudioClip clipGameWon = Resources.Load<AudioClip> ("Audio/GameWon");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixGroup[0];
        audioSource.PlayOneShot(clipGameWon);
    }
    public static void PlaySoundGameLost()
    {
        AudioMixer audioMixer = Resources.Load<AudioMixer>("Master");
        AudioMixerGroup[] audioMixGroup = audioMixer.FindMatchingGroups("SFX");
        GameObject soundObject = new GameObject("SFX Object");
        AudioClip clipGameLost = Resources.Load<AudioClip> ("Audio/GameLost");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixGroup[0];
        audioSource.PlayOneShot(clipGameLost);
    }
    public static void PlaySoundMenuButton()
    {
        AudioMixer audioMixer = Resources.Load<AudioMixer>("Master");
        AudioMixerGroup[] audioMixGroup = audioMixer.FindMatchingGroups("SFX");
        GameObject soundObject = new GameObject("SFX Object");
        AudioClip menuButton = Resources.Load<AudioClip> ("Audio/MenuButton");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixGroup[0];
        audioSource.PlayOneShot(menuButton);
    }

}
