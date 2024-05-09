using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeChanger : MonoBehaviour
{

    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider engineVolumeSlider;

    public AudioMixer masterMixer;


    private float masterLvl;
    private float musicLvl;
    private float sfxLvl;
    private float engineLvl;


    public void SetMasterVolume()
    {
        masterLvl = masterVolumeSlider.value;
        masterMixer.SetFloat("Master", Mathf.Log10(masterLvl) * 20);
    }

    public void SetMusicVolume()
    {
        musicLvl = musicVolumeSlider.value;
        masterMixer.SetFloat("Music", Mathf.Log10(musicLvl) * 20);
    }

    public void SetSfxVolume()
    {
        sfxLvl = sfxVolumeSlider.value;
        masterMixer.SetFloat("SFX", Mathf.Log10(sfxLvl) * 20);
    }

    public void SetEngineVolume()
    {
        engineLvl = engineVolumeSlider.value;
        masterMixer.SetFloat("Engine", Mathf.Log10(engineLvl) * 20 - 10);
    }
}
