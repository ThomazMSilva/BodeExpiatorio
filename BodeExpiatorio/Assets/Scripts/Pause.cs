using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class AbirPause : MonoBehaviour
{
    public GameObject menu;
    public bool Pausado;

    void Start()
    {
        menu.SetActive(false);
    }

    void update()
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
    }

    public void ResumeGame()
    {
        menu.SetActive(false);
        Time.timeScale = 1f;
        Pausado = false;
    }
}
