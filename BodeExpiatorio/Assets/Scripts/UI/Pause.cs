using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public GameObject pauseScreen;
    [SerializeField] GameObject firstPauseButton;
    public static bool Pausado;
    private Entrada input;
    [SerializeField] private UINavigationManager navigationManager;

    //private void Start() => pauseScreen.SetActive(false);

    public void SetPauseSelected() => UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(firstPauseButton);

    public void PauseGame()
    {
        Time.timeScale = 0f; 
        Pausado = true;
        navigationManager.OpenPanel(pauseScreen);
        /*pauseScreen.SetActive(true);
        SetPauseSelected();*/
      
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;   
        Pausado = false;
        navigationManager.CloseAllPanels();
        /*StopAllCoroutines();
        StartCoroutine(ClosePauseScreen());*/
        //pauseScreen.SetActive(false);
    }

    private void OnEnable() => StartCoroutine(InitializeReference());
    
    private void OnDisable()
    {
        input.OnPauseButtonDown -= EscapePressed;
        if (pauseScreen.activeSelf) ResumeGame();
    }
    
    private IEnumerator InitializeReference()
    {
        while(input == null)
        {
            input = Entrada.Instance;
            yield return null;
        }
        input.OnPauseButtonDown += EscapePressed;
    }

    private void EscapePressed()
    {
        if (Pausado)
        {
           ResumeGame();
        }
        else
            PauseGame();
    }

    private IEnumerator ClosePauseScreen() 
    {
        while (pauseScreen.activeSelf)
        {
            navigationManager.ClosePanel();
            yield return null;
        }
    }


    public VidaJogador vidaJogador;
    public Jogador jogador;

    public void OnReiniciarNivel()
    {
        if (vidaJogador == null) return;

        vidaJogador.DamageHealth("Reiniciou nivel"); //esse metodo, DamageHealth(), mata o jogador instantâneamente se n botar um valor de dano.
        ResumeGame();
    }

    public void OnReturnToLastCheckpoint()
    {
        GameManager.Instance.LoadLastCheckpoint();
        ResumeGame();
    }
    public void ReturnToMainMenu() => GameManager.Instance.LoadMainMenu();
}
