using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public Slider volumeSlider; 

    private void Start()
    {
      
        float savedVolume = PlayerPrefs.GetFloat("GameVolume", 1f);
        volumeSlider.value = savedVolume; 
        AudioManager.Instance.SetGeneralVolume(savedVolume); 
    }

   
    public void SetVolume()
    {
        AudioManager.Instance.SetGeneralVolume(volumeSlider.value);
        PlayerPrefs.SetFloat("GameVolume", volumeSlider.value); 
        PlayerPrefs.Save(); 
    }

    private void OnDestroy()
    {
      
        PlayerPrefs.SetFloat("GameVolume", volumeSlider.value);
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

}
