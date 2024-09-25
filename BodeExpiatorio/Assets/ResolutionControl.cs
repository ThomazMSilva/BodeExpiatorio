using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ResolutionControl : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private void Start()
    {
       
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

    
        resolutionDropdown.ClearOptions();

    
        float currentRefreshRate = Screen.currentResolution.refreshRate;
        Debug.Log("Taxa de Atualização Atual: " + currentRefreshRate);

    
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRate == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
                Debug.Log("Resolução Filtrada: " + resolutions[i].width + " x " + resolutions[i].height);
            }
        }

        
        List<string> options = new List<string>();
        int currentResolutionIndex = 0; 

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + " x " + filteredResolutions[i].height + " @ " + filteredResolutions[i].refreshRate + "Hz";
            options.Add(resolutionOption);

          
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i; 
            }
        }

        
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

     
        LoadSavedResolution();

      
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChange);
    }

  
    private void OnResolutionChange(int resolutionIndex)
    {
        SetResolution(resolutionIndex); 
        Debug.Log("Resolução Alterada para: " + filteredResolutions[resolutionIndex].width + " x " + filteredResolutions[resolutionIndex].height);
    }

   
    private void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

  
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
        Debug.Log("Resolução Salva: " + resolution.width + " x " + resolution.height);
    }

    
    private void LoadSavedResolution()
    {
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
            resolutionDropdown.value = savedResolutionIndex; 
            resolutionDropdown.RefreshShownValue();
            SetResolution(savedResolutionIndex); 
            Debug.Log("Resolução Carregada: " + filteredResolutions[savedResolutionIndex].width + " x " + filteredResolutions[savedResolutionIndex].height);
        }
    }
}
