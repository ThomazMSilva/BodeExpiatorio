using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ResolutionControl : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown; // Referência ao TMP_Dropdown

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private void Start()
    {
        // Obtém todas as resoluções suportadas
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        // Limpa as opções do dropdown
        resolutionDropdown.ClearOptions();

        // Obtém a taxa de atualização atual
        float currentRefreshRate = Screen.currentResolution.refreshRate;
        Debug.Log("Taxa de Atualização Atual: " + currentRefreshRate);

        // Filtra resoluções pela taxa de atualização atual
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRate == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
                Debug.Log("Resolução Filtrada: " + resolutions[i].width + " x " + resolutions[i].height);
            }
        }

        // Cria opções para o dropdown
        List<string> options = new List<string>();
        int currentResolutionIndex = 0; // Índice da resolução atual

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + " x " + filteredResolutions[i].height + " @ " + filteredResolutions[i].refreshRate + "Hz";
            options.Add(resolutionOption);

            // Verifica a resolução atual
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i; // Define o índice da resolução atual no dropdown
            }
        }

        // Adiciona as opções filtradas ao dropdown
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex; // Define o valor inicial baseado na resolução atual
        resolutionDropdown.RefreshShownValue();

        // Carrega a resolução salva
        LoadSavedResolution();

        // Adiciona um listener para mudar a resolução quando o dropdown é alterado
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChange);
    }

    // Função chamada quando a resolução é alterada
    private void OnResolutionChange(int resolutionIndex)
    {
        SetResolution(resolutionIndex); // Aplica a nova resolução
        Debug.Log("Resolução Alterada para: " + filteredResolutions[resolutionIndex].width + " x " + filteredResolutions[resolutionIndex].height);
    }

    // Função para aplicar a resolução selecionada
    private void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // Salva a resolução escolhida
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
        Debug.Log("Resolução Salva: " + resolution.width + " x " + resolution.height);
    }

    // Função para carregar a resolução salva
    private void LoadSavedResolution()
    {
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
            resolutionDropdown.value = savedResolutionIndex; // Define o valor do dropdown
            resolutionDropdown.RefreshShownValue();
            SetResolution(savedResolutionIndex); // Aplica a resolução salva
            Debug.Log("Resolução Carregada: " + filteredResolutions[savedResolutionIndex].width + " x " + filteredResolutions[savedResolutionIndex].height);
        }
    }
}
