using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioSource musicSource; 

    private void Start()
    {
        if (volumeSlider != null)
        {
           
            volumeSlider.value = musicSource.volume;

          
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    private void SetVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume; 
        }
    }

    private void OnDestroy()
    {
        
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(SetVolume);
        }
    }

}
