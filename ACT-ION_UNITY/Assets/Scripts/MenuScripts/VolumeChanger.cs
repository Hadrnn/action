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


    public float masterLvl;
    public float musicLvl;
    public float sfxLvl;
    public float engineLvl;

    public void Start()
    {
        GameSingleton.GetInstance().GetIni();

        masterLvl = GameSingleton.GetInstance().currentVolume.masterLvl;
        musicLvl = GameSingleton.GetInstance().currentVolume.musicLvl;
        sfxLvl = GameSingleton.GetInstance().currentVolume.sfxLvl;
        engineLvl = GameSingleton.GetInstance().currentVolume.engineLvl;

        SetVolume();
    }


    private void SetVolume()
    {
        masterMixer.SetFloat("Master", Mathf.Log10(masterLvl) * 20);
        masterMixer.SetFloat("Music", Mathf.Log10(musicLvl) * 20);
        masterMixer.SetFloat("SFX", Mathf.Log10(sfxLvl) * 20);
        masterMixer.SetFloat("Engine", Mathf.Log10(engineLvl) * 20 - 10);

        masterVolumeSlider.value = masterLvl;
        musicVolumeSlider.value = musicLvl;
        sfxVolumeSlider.value = sfxLvl;
        engineVolumeSlider.value = engineLvl;


    }
    public void SetMasterVolume()
    {
        masterLvl = masterVolumeSlider.value;
        GameSingleton.GetInstance().ChangeIni(GameSingleton.masterVolumeMark, masterLvl.ToString("0.000"));
        SetVolume();
    }

    public void SetMusicVolume()
    {
        musicLvl = musicVolumeSlider.value;
        GameSingleton.GetInstance().ChangeIni(GameSingleton.musicVolumeMark, musicLvl.ToString("0.000"));
        SetVolume();

    }

    public void SetSfxVolume()
    {
        sfxLvl = sfxVolumeSlider.value;
        GameSingleton.GetInstance().ChangeIni(GameSingleton.sfxVolumeMark, sfxLvl.ToString("0.000"));
        SetVolume();

    }

    public void SetEngineVolume()
    {
        engineLvl = engineVolumeSlider.value;
        GameSingleton.GetInstance().ChangeIni(GameSingleton.engineVolumeMark, engineLvl.ToString("0.000"));
        SetVolume();

    }
}
