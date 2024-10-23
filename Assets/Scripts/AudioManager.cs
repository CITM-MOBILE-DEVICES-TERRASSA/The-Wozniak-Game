using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] soundClips;

    public enum SoundType
    {
        HitBrick,
        HitPaddle,
        GameOver,
    }

    public void PlaySound(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.HitBrick:
                audioSource.PlayOneShot(soundClips[0]);
                break;
            case SoundType.HitPaddle:
                audioSource.PlayOneShot(soundClips[1]);
                break;
            case SoundType.GameOver:
                audioSource.PlayOneShot(soundClips[2]);
                break;
        }
    }
}