using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public AudioSource[] musicSources;
    public AudioSource[] sfxSources;

    void Start()
    {
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        musicVolumeSlider.value = savedMusicVolume;

        foreach (AudioSource source in musicSources)
        {
            source.volume = savedMusicVolume;
        }

        float savedSfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
        sfxVolumeSlider.value = savedSfxVolume;

        foreach (AudioSource source in sfxSources)
        {
            source.volume = savedSfxVolume;
        }

        musicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(UpdateSfxVolume);
    }

    void UpdateMusicVolume(float volume)
    {
        foreach (AudioSource source in musicSources)
        {
            source.volume = volume;
        }
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    void UpdateSfxVolume(float volume)
    {
        foreach (AudioSource source in sfxSources)
        {
            source.volume = volume;
        }
        PlayerPrefs.SetFloat("SfxVolume", volume);
    }
}