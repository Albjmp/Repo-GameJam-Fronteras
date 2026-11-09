using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public AudioMixer masterMixer;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float savedMaster = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        masterSlider.value = savedMaster;
        SetMasterVolume(savedMaster);

        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        musicSlider.value = savedMusic;
        SetMusicVolume(savedMusic);

        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        sfxSlider.value = savedSFX;
        SetSFXVolume(savedSFX);
    }

    // Update is called once per frame
    public void SetMasterVolume(float value)
    {
        float dbValue = Mathf.Log10(value) * 20;
        masterMixer.SetFloat("MasterVol", dbValue);

        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        float dbValue = Mathf.Log10(value) * 20;
        masterMixer.SetFloat("MusicVol", dbValue);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        float dbValue = Mathf.Log10(value) * 20;
        masterMixer.SetFloat("SFXVol", dbValue);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
}