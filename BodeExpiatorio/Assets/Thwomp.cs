using System.Collections;
using UnityEngine;

public class Thwomp : MonoBehaviour
{
    public float fallSpeed = 20f; // Velocidade de queda
    public float riseSpeed = 2f; // Velocidade de subida
    public float waitTimeBeforeFall = 2f; // Tempo de espera antes de cair novamente
    private bool isFalling = true;
    private bool isRising = false;
    public Transform bottomSensor; // Sensor inferior para detectar o chão ou o jogador
    public Transform topSensor; // Sensor superior para detectar blocos acima

    private void Start()
    {
        // Inicialmente o Thwomp está caindo
        isFalling = true;
    }

    private void Update()
    {
        if (isFalling)
        {
            Fall(); // Controla a queda
        }
        else if (isRising)
        {
            Rise(); // Controla a subida
        }
    }

    // Função para movimentar o Thwomp para baixo (queda)
    private void Fall()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // Verificar se o sensor inferior detectou o chão ou o jogador
        RaycastHit hit;
        if (Physics.Raycast(bottomSensor.position, Vector3.down, out hit, 0.1f))
        {
            if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Player"))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    // Se for o jogador, zerar a vida
                    VidaJogador vidaJogador = hit.collider.GetComponent<VidaJogador>();
                    if (vidaJogador != null)
                    {
                        vidaJogador.DamageHealth(vidaJogador.CurrentHealth);
                    }
                }

                // Parar de cair e iniciar o processo de subida
                StartCoroutine(RiseAfterDelay());
            }
        }
    }

    // Função para movimentar o Thwomp para cima (subida)
    private void Rise()
    {
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        // Verificar se o sensor superior detectou um bloco acima
        RaycastHit hit;
        if (Physics.Raycast(topSensor.position, Vector3.up, out hit, 0.1f))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                // Quando atingir um bloco acima, parar de subir e aguardar antes de cair de novo
                StartCoroutine(WaitBeforeFall());
            }
        }
    }

    // Coroutine para iniciar a subida após um pequeno delay
    private IEnumerator RiseAfterDelay()
    {
        isFalling = false;
        yield return new WaitForSeconds(0.5f); // Pausa antes de começar a subir
        isRising = true;
    }

    // Coroutine para esperar antes de cair novamente
    private IEnumerator WaitBeforeFall()
    {
        isRising = false;
        yield return new WaitForSeconds(waitTimeBeforeFall); // Espera antes de cair novamente
        isFalling = true; // Reiniciar a queda
    }
}
