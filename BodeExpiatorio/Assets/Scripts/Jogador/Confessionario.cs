using System.Collections;
using UnityEngine;

public class Confessionario : MonoBehaviour
{
    [SerializeField] private int roomIndex;
    [SerializeField] private bool checkpointStartsActive;
    [SerializeField] private bool recoverMaxHealthToStartingPoint = true;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private GameObject buffScreen;
    private float startingRoomTime;
    private float startingRunTime;
    private Jogador player;

    private void Awake() => player = FindAnyObjectByType<Jogador>();
    private static Transform lastRespawnPoint;


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
        lastRespawnPoint = respawnPoint;
    }

    public void Respawn()
    {
        if (player.Vida.CurrentMaxHealth <= 0) return;
        player.Vida.ResetDamageTakenInRun();
        
        if(recoverMaxHealthToStartingPoint) player.Vida.CureMaxHealth();

        player.ApplyCure();
        startingRunTime = Time.time;
        Spawn();
    }

    public void Spawn()
    {
        Transform point = lastRespawnPoint != null ? lastRespawnPoint : respawnPoint;
        player.SetPosition(respawnPoint.position);
        player.StopCreepingDamage();
        player.SetBuffs();
    }

    private void GameOver()
    {
        Debug.Log("MORREU MUITASSO INSANO MESMO.");
        GameManager.Instance.LoadDeathScene();
    }
    public void TeleportToLastCheckpoint()
    {
        Transform point = lastRespawnPoint != null ? lastRespawnPoint : respawnPoint;
        player.SetPosition(point.position);
        player.StopCreepingDamage();
        player.SetBuffs();
    }

    public void NextLevel() => GameManager.Instance.LoadNextRoom();

    public string LevelStatistics()
    {
        return $"--Level {roomIndex + 1} Statistics--\n\nRoomTime: {RoomTime()}; RunTime: {RunTime()}\n{player.Vida.DamageString}"; 
    }

    public float RoomTime() => Time.time - startingRoomTime;

    public float RunTime() => Time.time - startingRunTime;

    public float DamageTakenRoom() => player.Vida.DamageTakenInRoom;

    public float DamageTakenRun() => player.Vida.DamageTakenInRun;

    public float FervorTakenRoom() => player.Vida.FervorTakenInRoom;

    public float FervorTakenRun() => player.Vida.FervorTakenInRun;

    /// <summary>
    /// x = damage in room
    /// y = damage in run
    /// z = fervor in room
    /// w = fervor in run
    /// </summary>
    /// <returns></returns>
    public Vector4 TotalTorment() => new(DamageTakenRoom(), DamageTakenRun(), FervorTakenRoom(), FervorTakenRun());
}
