using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider SFXSlider;   
    public Slider MusicSlider;  

    private void Start()
    {
        
        float savedVolume = PlayerPrefs.GetFloat("GameVolume", 1f);
        volumeSlider.value = savedVolume;
        AudioManager.Instance.SetGeneralVolume(savedVolume);

       
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        SFXSlider.value = savedSFXVolume;
        AudioManager.Instance.SetSFXVolume(savedSFXVolume);

        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        MusicSlider.value = savedMusicVolume;
        AudioManager.Instance.SetMusicVolume(savedMusicVolume);

       
        volumeSlider.onValueChanged.AddListener(delegate { SetGeneralVolume(); });
        SFXSlider.onValueChanged.AddListener(delegate { SetSFXVolume(); });
        MusicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
    }

    public void SetGeneralVolume()
    {
        float value = volumeSlider.value;
        AudioManager.Instance.SetGeneralVolume(value);
        PlayerPrefs.SetFloat("GameVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume()
    {
        float value = SFXSlider.value;
        AudioManager.Instance.SetSFXVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume()
    {
        float value = MusicSlider.value;
        AudioManager.Instance.SetMusicVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        
        PlayerPrefs.SetFloat("GameVolume", volumeSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", SFXSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", MusicSlider.value);
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
