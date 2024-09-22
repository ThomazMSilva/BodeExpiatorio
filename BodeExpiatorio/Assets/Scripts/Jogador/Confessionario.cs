using UnityEngine;

public class Confessionario : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    private Jogador player;

    private void Awake()
    {
        player = FindAnyObjectByType<Jogador>();
    }

    private void OnEnable()
    {
        player.Vida.OnPlayerDeath += Respawn;
        player.Vida.OnPlayerTrueDeath += GameOver;
    }

    private void OnDisable()
    {
        player.Vida.OnPlayerDeath -= Respawn;
        player.Vida.OnPlayerTrueDeath -= GameOver;
    }

    private void Respawn()
    {
        if (player.Vida.CurrentMaxHealth <= 0) return;
        player.SetPosition(respawnPoint.position);
        player.ApplyCure();
    }
    private void GameOver()
    {
        Debug.Log("MORREU MUITASSOZASSO INSANO MESMO.");
    }
}
