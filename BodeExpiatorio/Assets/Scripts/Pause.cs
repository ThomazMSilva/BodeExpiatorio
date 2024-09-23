using UnityEngine;
using UnityEngine.UI;

public class AbrirPause : MonoBehaviour
{
    public GameObject menu; 
    public bool Pausado;    

    void Start()
    {
        menu.SetActive(false); 
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
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
}
