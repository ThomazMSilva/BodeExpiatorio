using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Espinho : MonoBehaviour
{
    [Header("Configurações do Trampolim")]
    [SerializeField] private float launchForce = 10f;  
    [SerializeField] private int damageAmount = 1;    

    private void OnCollisionEnter(Collision collision)
    {
    
        if (collision.gameObject.CompareTag("Player"))
        {
    
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
  
                playerRb.AddForce(transform.up * launchForce, ForceMode.Impulse);
            }

       
            VidaJogador vidaJogador = collision.gameObject.GetComponent<VidaJogador>();
            if (vidaJogador != null)
            {
                vidaJogador.DamageHealth(damageAmount);
            }

            MovimentoJogador movimentoJogador = collision.gameObject.GetComponent<MovimentoJogador>();
            if (movimentoJogador != null)
            {
                movimentoJogador.DisableJump(); 
            }
        }
    }

}
