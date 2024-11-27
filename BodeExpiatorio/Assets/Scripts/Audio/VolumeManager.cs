using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class VolumeManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider volumeSlider; 
    public Slider SFXSlider;    
    public Slider MusicSlider;  

    [Header("VCA Paths")]
    public string generalVCAPath = "vca:/Ambiental"; 
    public string sfxVCAPath = "vca:/SFX";        
    public string musicVCAPath = "vca:/Musicas";    

    private VCA generalVCA;
    private VCA sfxVCA;
    private VCA musicVCA;

    private void InitializeVCAs()
    {
        //yield return new WaitForSeconds(1f);

        generalVCA = RuntimeManager.GetVCA(generalVCAPath);
        sfxVCA = RuntimeManager.GetVCA(sfxVCAPath);
        musicVCA = RuntimeManager.GetVCA(musicVCAPath);

        if (generalVCA.isValid() && sfxVCA.isValid() && musicVCA.isValid())
        {
            Debug.Log("VCAs inicializados com sucesso.");
        }
        else
        {
            Debug.LogError("Erro ao inicializar um ou mais VCAs.");
        }

        generalVCA = RuntimeManager.GetVCA(generalVCAPath);
        sfxVCA = RuntimeManager.GetVCA(sfxVCAPath);
        musicVCA = RuntimeManager.GetVCA(musicVCAPath);


        float savedVolume = PlayerPrefs.GetFloat("GameVolume", 1f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);


        volumeSlider.value = savedVolume;
        SFXSlider.value = savedSFXVolume;
        MusicSlider.value = savedMusicVolume;


        SetGeneralVolume(savedVolume);
        SetSFXVolume(savedSFXVolume);
        SetMusicVolume(savedMusicVolume);


        volumeSlider.onValueChanged.AddListener(SetGeneralVolume);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);
        MusicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    private void Awake() => InitializeVCAs();//StartCoroutine(InitializeVCAs());

    public void SetGeneralVolume(float value)
    {
        if (generalVCA.isValid())
        {
            generalVCA.setVolume(value);
            PlayerPrefs.SetFloat("GameVolume", value);
        }
    }

    public void SetSFXVolume(float value)
    {
        if (sfxVCA.isValid())
        {
            sfxVCA.setVolume(value);
            PlayerPrefs.SetFloat("SFXVolume", value);
        }
    }

    public void SetMusicVolume(float value)
    {
        if (musicVCA.isValid())
        {
            musicVCA.setVolume(value);
            PlayerPrefs.SetFloat("MusicVolume", value);
        }
    }

    private void OnDestroy()
    {
        
        PlayerPrefs.SetFloat("GameVolume", volumeSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", SFXSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", MusicSlider.value);
    }

}
