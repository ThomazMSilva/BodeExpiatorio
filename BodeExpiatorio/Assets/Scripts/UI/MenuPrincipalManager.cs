using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField] private UINavigationManager navigationManager;

    [SerializeField] private GameObject pauseMenuScreen;
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private GameObject graphicsScreen;
    [SerializeField] private GameObject soundScreen;
    [SerializeField] private GameObject accessiblityScreen;

    public void AbrirMenuGrafico()
    {
        navigationManager.OpenPanel(graphicsScreen);
    }

    public void AbrirMenuSom()
    {
        navigationManager.OpenPanel(soundScreen);
    }

    public void AbrirMenuAcessibilidade()
    {
        navigationManager.OpenPanel(accessiblityScreen);
    }

    public void AbrirOpções()
    {
        navigationManager.OpenPanel(optionsScreen);
    }

    public void OpenCredits()
    {
        navigationManager.OpenPanel(creditsScreen);
    }

    public void CloseCredits()
    {
        navigationManager.ClosePanel();
    }

    public void FecharOpções()
    {
        navigationManager.ClosePanel();
    }
    public void FecharGrafico()
    {
        navigationManager.ClosePanel();
    }
    public void FecharSom()
    {
        navigationManager.ClosePanel();
    }
    public void FecharAcessibilidade()
    {
        navigationManager.ClosePanel();
    }
    public void ReturnToMainMenu() => GameManager.Instance.LoadMainMenu();

    public void SairDoJogo() => Application.Quit();

    public void Continue() => GameManager.Instance.Continue();

    public void NewGame() => GameManager.Instance.NewGame();

    public void BackToLastCheckpoint() => GameManager.Instance.LoadLastCheckpoint();
}
