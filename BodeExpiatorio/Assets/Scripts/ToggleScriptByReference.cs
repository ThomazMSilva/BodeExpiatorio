using UnityEngine;
using UnityEngine.UI;

public class TogglePersistence : MonoBehaviour
{
    public Toggle toggleCheckbox;
    private string toggleKey = "ToggleScriptState";

    private void Start()
    {
        // Carrega o estado do Toggle ao iniciar
        if (PlayerPrefs.HasKey(toggleKey))
        {
            bool isToggleOn = PlayerPrefs.GetInt(toggleKey) == 1;
            toggleCheckbox.isOn = isToggleOn;
        }
        else
        {
            // Inicializa o Toggle com um valor padrão
            toggleCheckbox.isOn = false;
        }

        // Adiciona o listener para salvar o estado ao alterar
        toggleCheckbox.onValueChanged.AddListener(delegate {
            SaveToggleState(toggleCheckbox.isOn);
        });
    }

    private void SaveToggleState(bool isOn)
    {
        // Salva o estado do Toggle no PlayerPrefs
        PlayerPrefs.SetInt(toggleKey, isOn ? 1 : 0);
        PlayerPrefs.Save();

        // Log de depuração
        Debug.Log($"Estado do Toggle salvo: {isOn}");
    }
}
