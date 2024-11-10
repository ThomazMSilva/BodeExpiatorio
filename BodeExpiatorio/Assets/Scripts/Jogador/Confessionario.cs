using System.Collections.Generic;
using UnityEngine;

public class Confessionario : MonoBehaviour
{
    [SerializeField] private int roomIndex;
    [SerializeField] private bool checkpointStartsActive;
    [SerializeField] private bool recoverMaxHealthToStartingPoint = true;
    [SerializeField] private bool isFinalRoom;
    public bool IsFinalRoom { get => isFinalRoom; }

    [SerializeField] private Transform respawnPoint;
    [SerializeField] private GameObject buffScreen;

    public float damageThreshold = 50f;
    private float startingRoomTime;
    private float startingRunTime;

    private Jogador player;
    public Jogador Player { get => player; }

    public static Confessionario ultimoAtivo;

    private void Awake() => player = FindAnyObjectByType<Jogador>();

    private void Start()
    {
        startingRoomTime = Time.time;
        startingRunTime = Time.time;
        Spawn();

        GameManager gameManager = GameManager.Instance;
        gameManager.SetCurrentRoom(roomIndex);
        gameManager.SetCurrentPlayer(player);
        if (checkpointStartsActive)
        {
            gameManager.ActivateSceneCheckpoint(roomIndex);
            SetUltimoAtivo();
        }
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
        if (!other.gameObject.CompareTag("Player")) return;
        Entrada.Instance.OnKneelButtonDown += Pray;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        Entrada.Instance.OnKneelButtonDown -= Pray;
    }

    private void Pray()
    {
        GameManager.Instance.ActivateSceneCheckpoint(roomIndex);
        SetUltimoAtivo();
    }

    private void SetUltimoAtivo() => ultimoAtivo = this;

    public void Respawn()
    {
        if (player.Vida.CurrentMaxHealth <= 0) return;
        player.Vida.ResetDamageTakenInRun();

        if (recoverMaxHealthToStartingPoint) player.Vida.CureMaxHealth();

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

    public Transform GetRespawnPoint() => respawnPoint;

    private void GameOver() => GameManager.Instance.LoadDeathScene();

    public string LevelStatistics() => $"--Level {roomIndex + 1} Statistics--\n\nRoomTime: {RoomTime()}; RunTime: {RunTime()}\n{player.Vida.DamageString}";

    public float RoomTime() => Time.time - startingRoomTime;

    public float RunTime() => Time.time - startingRunTime;

    public float DamageTakenRoom() => player.Vida.DamageTakenInRoom;

    public float DamageTakenRun() => player.Vida.DamageTakenInRun;

    public float FervorTakenRoom() => player.Vida.FervorTakenInRoom;

    public float FervorTakenRun() => player.Vida.FervorTakenInRun;

    public Vector4 TotalTorment() => new(DamageTakenRoom(), DamageTakenRun(), FervorTakenRoom(), FervorTakenRun());
}
