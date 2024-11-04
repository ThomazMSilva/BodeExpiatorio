using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AbrirPause : MonoBehaviour
{
    public GameObject menu; 
    public bool Pausado;
    private Entrada input;

    void Start()
    {
        menu.SetActive(false); 
    }
    
    public void PauseGame()
    {
        menu.SetActive(true);
        Time.timeScale = 0f; 
        Pausado = true;

      
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    public void ResumeGame()
    {
        menu.SetActive(false);
        Time.timeScale = 1f;   
        Pausado = false;
    }

    private void OnEnable() => StartCoroutine(InitializeReference());
    
    private void OnDisable()
    {
        input.OnPauseButtonDown -= EscapePressed;
        if (menu.activeSelf) ResumeGame();
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

    void EscapePressed()
    {
        if (Pausado)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }

    }

    public void ReturnToMainMenu() => GameManager.Instance.LoadMainMenu();
}
