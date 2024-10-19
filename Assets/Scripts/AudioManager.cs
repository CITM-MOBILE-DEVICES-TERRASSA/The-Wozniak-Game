using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource; // Asigna este en el inspector
    public AudioClip[] soundClips; // Arrastra tus clips aquí en el inspector

    // Enum para facilitar la identificación de clips
    public enum SoundType
    {
        HitBrick,
        HitPaddle,
        GameOver,
        // Añade otros sonidos según sea necesario
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
                // Maneja otros sonidos aquí
        }
    }
}