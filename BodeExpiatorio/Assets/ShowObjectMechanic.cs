using UnityEngine;
using Cinemachine;
using System.Collections;

public class ShowObjectMechanic : MonoBehaviour
{
    [SerializeField] private Transform objectToFocus; 
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private CinemachineVirtualCamera focusCamera; 
    [SerializeField] private float focusDuration = 2f;

    private bool isPaused = false; 
    private VidaJogador vidaJogador;
    private ContagemRegressivaVidaJogador contagemRegressiva;
    public GameObject gatilho;

    private void Awake()
    {
        vidaJogador = FindObjectOfType<VidaJogador>();
        contagemRegressiva = FindObjectOfType<ContagemRegressivaVidaJogador>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPaused)
        {
            StartCoroutine(HandleFocus());
        }
    }

    private IEnumerator HandleFocus()
    {
        isPaused = true;

        if (contagemRegressiva != null) contagemRegressiva.isCountDownActive = false;

     
        MovimentoJogador movimentoJogador = FindObjectOfType<MovimentoJogador>();
        if (movimentoJogador != null)
        {
            movimentoJogador.SetPaused(true);
        }

      
        playerCamera.Priority = 0;
        focusCamera.Priority = 10;

        yield return new WaitForSecondsRealtime(focusDuration);

      
        if (movimentoJogador != null)
        {
            movimentoJogador.SetPaused(false);
        }

        if (contagemRegressiva != null) contagemRegressiva.isCountDownActive = true;

       
        focusCamera.Priority = 0;
        playerCamera.Priority = 10;

        gatilho.SetActive(false);
        isPaused = false;
    }
}
