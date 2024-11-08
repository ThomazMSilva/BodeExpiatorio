using UnityEngine;

public class InstaDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.TryGetComponent<Jogador>(out Jogador player))
            player.InstaKill("The Abyss.");
    }
    
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.TryGetComponent<Jogador>(out Jogador player))
            player.InstaKill("The Abyss.");
    }
}
