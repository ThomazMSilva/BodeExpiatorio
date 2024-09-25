using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ResolutionControl : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown; // Refer�ncia ao TMP_Dropdown

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private void Start()
    {
        // Obt�m todas as resolu��es suportadas
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        // Limpa as op��es do dropdown
        resolutionDropdown.ClearOptions();

        // Obt�m a taxa de atualiza��o atual
        float currentRefreshRate = Screen.currentResolution.refreshRate;
        Debug.Log("Taxa de Atualiza��o Atual: " + currentRefreshRate);

        // Filtra resolu��es pela taxa de atualiza��o atual
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRate == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
                Debug.Log("Resolu��o Filtrada: " + resolutions[i].width + " x " + resolutions[i].height);
            }
        }

        // Cria op��es para o dropdown
        List<string> options = new List<string>();
        int currentResolutionIndex = 0; // �ndice da resolu��o atual

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + " x " + filteredResolutions[i].height + " @ " + filteredResolutions[i].refreshRate + "Hz";
            options.Add(resolutionOption);

            // Verifica a resolu��o atual
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i; // Define o �ndice da resolu��o atual no dropdown
            }
        }

        // Adiciona as op��es filtradas ao dropdown
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex; // Define o valor inicial baseado na resolu��o atual
        resolutionDropdown.RefreshShownValue();

        // Carrega a resolu��o salva
        LoadSavedResolution();

        // Adiciona um listener para mudar a resolu��o quando o dropdown � alterado
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChange);
    }

    // Fun��o chamada quando a resolu��o � alterada
    private void OnResolutionChange(int resolutionIndex)
    {
        SetResolution(resolutionIndex); // Aplica a nova resolu��o
        Debug.Log("Resolu��o Alterada para: " + filteredResolutions[resolutionIndex].width + " x " + filteredResolutions[resolutionIndex].height);
    }

    // Fun��o para aplicar a resolu��o selecionada
    private void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // Salva a resolu��o escolhida
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
        Debug.Log("Resolu��o Salva: " + resolution.width + " x " + resolution.height);
    }

    // Fun��o para carregar a resolu��o salva
    private void LoadSavedResolution()
    {
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
            resolutionDropdown.value = savedResolutionIndex; // Define o valor do dropdown
            resolutionDropdown.RefreshShownValue();
            SetResolution(savedResolutionIndex); // Aplica a resolu��o salva
            Debug.Log("Resolu��o Carregada: " + filteredResolutions[savedResolutionIndex].width + " x " + filteredResolutions[savedResolutionIndex].height);
        }
    }
}
