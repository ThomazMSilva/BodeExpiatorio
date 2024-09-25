using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class GameSettingsManager : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown; 

    private void Start()
    {
        LoadSettings();

        
        volumeSlider.onValueChanged.AddListener(SetVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
    }

    private void LoadSettings()
    {
      
        if (PlayerPrefs.HasKey("GameVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("GameVolume");
            volumeSlider.value = savedVolume;
            AudioListener.volume = savedVolume;
        }

        
        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            bool isFullscreen = PlayerPrefs.GetInt("Fullscreen") == 1;
            fullscreenToggle.isOn = isFullscreen;
            Screen.fullScreen = isFullscreen;
        }

       
        if (PlayerPrefs.HasKey("Resolution"))
        {
            int resolutionIndex = PlayerPrefs.GetInt("Resolution");
            resolutionDropdown.value = resolutionIndex;
            ChangeResolution(resolutionIndex);
        }
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("GameVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ChangeResolution(int resolutionIndex)
    {
        Resolution[] resolutions = Screen.resolutions;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution", resolutionIndex);
        PlayerPrefs.Save();
    }
}
