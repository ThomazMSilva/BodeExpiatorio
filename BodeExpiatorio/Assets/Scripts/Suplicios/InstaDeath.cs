using UnityEngine;

public class InstaDeath : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Jogador>(out Jogador player))
            player.InstaKill("The Abyss.");
    }
}
