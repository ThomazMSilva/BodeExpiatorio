using UnityEngine;

public class TesteThwomp : MonoBehaviour
{
    [SerializeField] private bool isPlayerInside;
    [SerializeField] private float dano = 20f;
    [SerializeField] private Jogador player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            player = other.gameObject.GetComponent<Jogador>();
        }

        if (other.CompareTag("Ground") && isPlayerInside)
            player.ApplyDamageEffect(dano);
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player")) isPlayerInside = false;
    }
}
