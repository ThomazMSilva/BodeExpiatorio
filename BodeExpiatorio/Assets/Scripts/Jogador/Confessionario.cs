using UnityEngine;
using UnityEngine.SceneManagement;

public class Confessionario : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    private Jogador player;
    [SerializeField] private GameObject buffScreen;
    [SerializeField] private string morteSceneName;

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
        if (buffScreen == null || !other.gameObject.CompareTag("Player")) return;
        Entrada.Instance.OnKneelButtonDown += BuffScreen;
    }

    private void OnTriggerExit(Collider other)
    {
        if (buffScreen == null || !other.gameObject.CompareTag("Player")) return;
        Entrada.Instance.OnKneelButtonDown -= BuffScreen;
    }

    private void BuffScreen() => buffScreen.SetActive(!buffScreen.activeSelf);

    public void ActivateBuff(BuffButton buff)
    {
        player.ActivateBuff(buff.buffType);
        if(buffScreen) buffScreen.SetActive(false);
    }

    public void Respawn()
    {
        if (player.Vida.CurrentMaxHealth <= 0) return;
        player.SetPosition(respawnPoint.position);
        player.StopCreepingDamage();
        player.ApplyCure();
        player.SetBuffs();
    }

    private void GameOver()
    {
        Debug.Log("MORREU MUITASSO INSANO MESMO.");
        SceneManager.LoadScene(morteSceneName);
    }
}
