using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public Slider volumeSlider; // Slider de volume

    private void Start()
    {
        // Carrega o volume salvo
        float savedVolume = PlayerPrefs.GetFloat("GameVolume", 1f);
        volumeSlider.value = savedVolume; // Ajusta o Slider para o volume salvo
        AudioListener.volume = savedVolume; // Define o volume do jogo
    }

   
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume; 
        PlayerPrefs.SetFloat("GameVolume", volume); 
        PlayerPrefs.Save(); 
    }

    private void OnDestroy()
    {
      
        PlayerPrefs.SetFloat("GameVolume", volumeSlider.value);
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // Impede que o GameObject seja destruído ao trocar de cena
    }

}
