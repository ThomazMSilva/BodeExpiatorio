

using UnityEngine;
using Cinemachine;
using System.Collections;

public class ShowObjectMechanic : MonoBehaviour
{
    [SerializeField] private Transform objectToFocus; 
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private CinemachineVirtualCamera focusCamera; 
    [SerializeField] private float focusDuration = 2f;
    [SerializeField] private float extraWaitTime = 1f;  //Tempo de espera pra liberar o jogador pra se mexer 
    [SerializeField] private MovimentoJogador movimentoJogador;

    private VidaJogador vidaJogador;
    private ContagemRegressivaVidaJogador contagemRegressiva;
    public GameObject gatilho;

    
    private void Awake()
    {
        vidaJogador = FindObjectOfType<VidaJogador>();
        contagemRegressiva = FindObjectOfType<ContagemRegressivaVidaJogador>();
        movimentoJogador = FindObjectOfType<MovimentoJogador>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HandleFocus());
        }
    }

    private IEnumerator HandleFocus()
    {
        

        
        if (movimentoJogador != null) movimentoJogador.SetPlayerBind(true);

       
        if (contagemRegressiva != null) contagemRegressiva.isCountDownActive = false;

       
        playerCamera.Priority = 0;
        focusCamera.Priority = 10;

        yield return new WaitForSecondsRealtime(focusDuration);

       
        if (contagemRegressiva != null) contagemRegressiva.isCountDownActive = true;

        focusCamera.Priority = 0;
        playerCamera.Priority = 10;

        yield return new WaitForSecondsRealtime(extraWaitTime);

        
        if (movimentoJogador != null)
        {
            movimentoJogador.SetPlayerBind(false);
        }

        gatilho.SetActive(false);
        
    }
}