using System.Collections;
using UnityEngine;

public class Confessionario : MonoBehaviour
{
    [SerializeField] private int roomIndex;
    [SerializeField] private bool checkpointStartsActive;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private GameObject buffScreen;
    private float startingRoomTime;
    private float startingRunTime;
    private Jogador player;

    private void Awake() => player = FindAnyObjectByType<Jogador>();

    private void Start()
    {
        startingRoomTime = Time.time;
        startingRunTime = Time.time;
        Spawn();
        GameManager.Instance.SetCurrentRoom(roomIndex);
        if(checkpointStartsActive) GameManager.Instance.ActivateSceneCheckpoint(roomIndex);
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

    private void BuffScreen()
    {
        buffScreen.SetActive(!buffScreen.activeSelf);
        GameManager.Instance.ActivateSceneCheckpoint(roomIndex);
    }

    public void ActivateBuff(BuffButton buff)
    {
        player.ActivateBuff(buff.buffType);
        if(buffScreen) buffScreen.SetActive(false);
    }

    public void Respawn()
    {
        if (player.Vida.CurrentMaxHealth <= 0) return;
        player.Vida.ResetDamageTakenInRun();
        player.ApplyCure();
        startingRunTime = Time.time;
        Spawn();
    }

    public void Spawn()
    {
        player.SetPosition(respawnPoint.position);
        player.StopCreepingDamage();
        player.SetBuffs();
    }

    private void GameOver()
    {
        Debug.Log("MORREU MUITASSO INSANO MESMO.");
        GameManager.Instance.LoadDeathScene();
    }

    public void NextLevel() => GameManager.Instance.LoadNextRoom();

    public void DisplayTime() { Debug.Log($"RoomTime: {Time.time - startingRoomTime}; RunTime: {Time.time - startingRunTime}\n{player.Vida.DamageString}"); }
}
