using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource; // Asigna este en el inspector
    public AudioClip[] soundClips; // Arrastra tus clips aqu� en el inspector

    // Enum para facilitar la identificaci�n de clips
    public enum SoundType
    {
        HitBrick,
        HitPaddle,
        GameOver,
        // A�ade otros sonidos seg�n sea necesario
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
                audioSource.PlayOneShot(soundClips[0]);
                break;
                // Maneja otros sonidos aqu�
        }
    }
}