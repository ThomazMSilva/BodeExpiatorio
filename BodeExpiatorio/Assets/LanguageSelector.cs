using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageSelector : MonoBehaviour
{
    public TMP_Dropdown languageDropdown;

    private void Start()
    {
        
        languageDropdown.onValueChanged.AddListener(ChangeLanguage);
       
        SetInitialDropdownValue();
    }

    private void SetInitialDropdownValue()
    {
       
        Locale currentLocale = LocalizationSettings.SelectedLocale;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            if (LocalizationSettings.AvailableLocales.Locales[i] == currentLocale)
            {
                languageDropdown.value = i;
                break;
            }
        }
    }

    private void ChangeLanguage(int index)
    {
        
        Locale selectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        LocalizationSettings.SelectedLocale = selectedLocale;
    }
}
