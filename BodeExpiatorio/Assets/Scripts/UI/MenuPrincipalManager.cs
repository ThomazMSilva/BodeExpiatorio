using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private GameObject graphicsScreen;
    [SerializeField] private GameObject soundScreen;
    [SerializeField] private GameObject accessiblityScreen;


    [SerializeField] private GameObject firstMenuOption;
    [SerializeField] private GameObject firstConfigOption;
    [SerializeField] private GameObject creditsReturnBTN;
    [SerializeField] private GameObject firstGraphicsBTN;
    [SerializeField] private GameObject firstSoundBTN;
    [SerializeField] private GameObject firstAccessibilityBTN;

    public void SetOptionsSelected() => EventSystem.current.SetSelectedGameObject(firstConfigOption);
    public void SetMainMenuSelected() => EventSystem.current.SetSelectedGameObject(firstMenuOption);
    public void SetGraphicsSelected() => EventSystem.current.SetSelectedGameObject(firstGraphicsBTN);
    public void SetSoundsSelected() => EventSystem.current.SetSelectedGameObject(firstSoundBTN);
    public void SetAccessibilitySelected() => EventSystem.current.SetSelectedGameObject(firstAccessibilityBTN);

    public void AbrirMenuGrafico()
    {
        optionsScreen.SetActive(false);
        graphicsScreen.SetActive(true);

        SetGraphicsSelected();
    }

    public void AbrirMenuSom()
    {
        optionsScreen.SetActive(false);
        soundScreen.SetActive(true);

        SetSoundsSelected();
    }

    public void AbrirMenuAcessibilidade()
    {
        optionsScreen.SetActive(false);
        accessiblityScreen.SetActive(true);

        SetAccessibilitySelected();
    }

    public void AbrirOpções()
    {
        mainMenuScreen.SetActive(false);
        optionsScreen.SetActive(true);

        SetOptionsSelected();
    }

    public void Credits()
    {
        creditsScreen.SetActive(!creditsScreen.activeSelf);
        EventSystem.current.SetSelectedGameObject(creditsScreen.activeSelf ? creditsReturnBTN : firstMenuOption);
    }

    public void FecharOpções()
    {
        optionsScreen.SetActive(false);
        mainMenuScreen.SetActive(true);
        //mainMenuScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(firstMenuOption);
    }
    public void FecharGrafico()
    {
        graphicsScreen.SetActive(false);
        optionsScreen.SetActive(true);

        SetOptionsSelected();
    }
    public void FecharSom()
    {
        soundScreen.SetActive(false);
        optionsScreen.SetActive(true);

        SetOptionsSelected();
    }
    public void FecharAcessibilidade()
    {
        accessiblityScreen.SetActive(false);
        optionsScreen.SetActive(true);
        
        SetOptionsSelected();
    }

    public void SairDoJogo() => Application.Quit();

    public void Continue() => GameManager.Instance.Continue();

    public void NewGame() => GameManager.Instance.NewGame();

    public void BackToLastCheckpoint() => GameManager.Instance.LoadLastCheckpoint();
}
