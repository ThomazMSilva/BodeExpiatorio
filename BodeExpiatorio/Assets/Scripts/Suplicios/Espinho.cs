using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Espinho : MonoBehaviour
{
    [Header("Configurações do Trampolim")]
    [SerializeField] private float launchForce = 10f;  
    [SerializeField] private int damageAmount = 1;    

    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.gameObject;
        if (go.CompareTag("Player"))
        {
            if(go.TryGetComponent<Jogador>(out Jogador player))
            {
                player.ApplyDamageEffect(damageAmount, transform.up * launchForce);
            }
        }
    }

}
