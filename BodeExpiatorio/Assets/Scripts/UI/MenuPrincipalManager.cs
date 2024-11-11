using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuScreen;
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private GameObject graphicsScreen;
    [SerializeField] private GameObject soundScreen;
    [SerializeField] private GameObject accessiblityScreen;


    [SerializeField] private GameObject firstMainMenuOption;
    [SerializeField] private GameObject firstPauseMenuOption;
    [SerializeField] private GameObject firstConfigOption;
    [SerializeField] private GameObject creditsReturnBTN;
    [SerializeField] private GameObject firstGraphicsBTN;
    [SerializeField] private GameObject firstSoundBTN;
    [SerializeField] private GameObject firstAccessibilityBTN;

    private bool isMainMenu;

    public void SetOptionsSelected() => EventSystem.current.SetSelectedGameObject(firstConfigOption);
    public void SetPauseMenuSelected() => EventSystem.current.SetSelectedGameObject(firstPauseMenuOption);
    public void SetMainSelected() => EventSystem.current.SetSelectedGameObject(firstMainMenuOption);
    public void SetGraphicsSelected() => EventSystem.current.SetSelectedGameObject(firstGraphicsBTN);
    public void SetSoundsSelected() => EventSystem.current.SetSelectedGameObject(firstSoundBTN);
    public void SetCreditsSelected() => EventSystem.current.SetSelectedGameObject(creditsReturnBTN);
    public void SetAccessibilitySelected() => EventSystem.current.SetSelectedGameObject(firstAccessibilityBTN);

    private void Start() => isMainMenu = mainMenuScreen.activeSelf;

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
        if(!isMainMenu) pauseMenuScreen.SetActive(false);
        else mainMenuScreen.SetActive(false);

        optionsScreen.SetActive(true);

        SetOptionsSelected();
    }

    public void OpenCredits()
    {
        if (!isMainMenu) pauseMenuScreen.SetActive(false);
        else mainMenuScreen.SetActive(false);
        creditsScreen.SetActive(true);
        SetCreditsSelected();
    }

    public void CloseCredits()
    {
        creditsScreen.SetActive(false);
        if (!isMainMenu)
        {
            pauseMenuScreen.SetActive(true);
            SetPauseMenuSelected();
        }
        else
        {
            mainMenuScreen.SetActive(true);
            SetMainSelected();
        }
    }

    public void FecharOpções()
    {
        optionsScreen.SetActive(false);

        if (!isMainMenu)
        {
            pauseMenuScreen.SetActive(true);
            SetPauseMenuSelected();
        }
        else
        {
            mainMenuScreen.SetActive(true);
            SetMainSelected();
        }
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
    public void ReturnToMainMenu() => GameManager.Instance.LoadMainMenu();

    public void SairDoJogo() => Application.Quit();

    public void Continue() => GameManager.Instance.Continue();

    public void NewGame() => GameManager.Instance.NewGame();

    public void BackToLastCheckpoint() => GameManager.Instance.LoadLastCheckpoint();
}
