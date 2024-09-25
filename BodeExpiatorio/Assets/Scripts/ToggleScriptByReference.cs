using UnityEngine;
using UnityEngine.UI;

public class TogglePersistence : MonoBehaviour
{
    public Toggle toggleCheckbox;
    private string toggleKey = "ToggleScriptState";

    private void Start()
    {
        
        if (PlayerPrefs.HasKey(toggleKey))
        {
            bool isToggleOn = PlayerPrefs.GetInt(toggleKey) == 1;
            toggleCheckbox.isOn = isToggleOn;
        }
        else
        {
            
            toggleCheckbox.isOn = false;
        }

        
        toggleCheckbox.onValueChanged.AddListener(delegate {
            SaveToggleState(toggleCheckbox.isOn);
        });
    }

    private void SaveToggleState(bool isOn)
    {
        
        PlayerPrefs.SetInt(toggleKey, isOn ? 1 : 0);
        PlayerPrefs.Save();

        
        Debug.Log($"Estado do Toggle salvo: {isOn}");
    }
}
