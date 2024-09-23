using UnityEngine;

public class Confessionario : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    private Jogador player;
    [SerializeField] private GameObject buffScreen;

    private void Awake() => player = FindAnyObjectByType<Jogador>();

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) player.Movimento.OnPlayerKneelInput += BuffScreen;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) player.Movimento.OnPlayerKneelInput -= BuffScreen;
    }

    private void BuffScreen() => buffScreen.SetActive(!buffScreen.activeSelf);

    public void ActivateBuff(BuffButton buff)
    {
        player.ActivateBuff(buff.buffType);
        buffScreen.SetActive(false);
    }

    private void Respawn()
    {
        if (player.Vida.CurrentMaxHealth <= 0) return;
        player.SetPosition(respawnPoint.position);
        player.StopCreepingDamage();
        player.ApplyCure();
        player.SetBuffs();
    }
    private void GameOver()
    {
        Debug.Log("MORREU MUITASSOZASSO INSANO MESMO.");
    }
}
