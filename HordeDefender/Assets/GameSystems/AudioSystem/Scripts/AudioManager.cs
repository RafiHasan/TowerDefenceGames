using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonNoCreate<AudioManager>
{
    
    public AudioManagerSO managerSO;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    public void SetAudioOn(bool audio)
    {
        managerSO.SetAudioOn(audio);
    }

    public bool GetAudioOn()
    {
        return managerSO.GetAudioOn();
    }

    private void Initialize()
    {
        foreach (Sound sound in managerSO.Sounds)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.audioClip;
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
        }
    }

    public void Play(string name)
    {
        managerSO.Play(name);
    }
}
