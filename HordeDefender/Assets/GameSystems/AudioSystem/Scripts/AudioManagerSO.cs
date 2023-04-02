using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioManager", menuName = "AudioManager")]
public class AudioManagerSO : ScriptableObject
{
    public Sound[] Sounds;

    public void SetAudioOn(bool audio)
    {
        PlayerPrefs.SetInt("AudioManager" + "SetAudioOn", audio ? 1 : 0);
    }

    public bool GetAudioOn()
    {
        return PlayerPrefs.GetInt("AudioManager" + "SetAudioOn", 1) == 1 ? true : false;
    }

    public void Play(string name)
    {
        if (!GetAudioOn())
            return;

        foreach (Sound sound in Sounds)
        {
            if (sound.name == name)
            {
                sound.audioSource?.Play();
                break;
            }
        }
    }
}
