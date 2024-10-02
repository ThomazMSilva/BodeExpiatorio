using UnityEngine;

public class Corda : MonoBehaviour
{
    private Jogador player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.TryGetComponent<Jogador>(out player);
            player.SetPlayerClimbing(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.SetPlayerClimbing(false);
        }
    }
}
