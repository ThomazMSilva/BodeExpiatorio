using System.Collections.Generic;
using UnityEngine;

public class Confessionario : MonoBehaviour
{
    [SerializeField] private int roomIndex;
    [SerializeField] private bool checkpointStartsActive;
    [SerializeField] private bool recoverMaxHealthToStartingPoint = true;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private GameObject buffScreen;
    public float damageThreshold = 50f;
    private float startingRoomTime;
    private float startingRunTime;
    private Jogador player;
    public Jogador Player { get { return player; } }

    /*public List<BuffButtonValues> aBuffs = new();
    public List<BuffButton> bBuffs = new();*/

    private void Awake() => player = FindAnyObjectByType<Jogador>();

    private void Start()
    {
        startingRoomTime = Time.time;
        startingRunTime = Time.time;
        Spawn();

        GameManager gameManager = GameManager.Instance;
        gameManager.SetCurrentRoom(roomIndex);
        gameManager.SetCurrentPlayer(player);
        if(checkpointStartsActive) gameManager.ActivateSceneCheckpoint(roomIndex);
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
        Entrada.Instance.OnKneelButtonDown += Pray;
    }

    private void OnTriggerExit(Collider other)
    {
        if (buffScreen == null || !other.gameObject.CompareTag("Player")) return;
        Entrada.Instance.OnKneelButtonDown -= Pray;
    }

    private void Pray() => GameManager.Instance.ActivateSceneCheckpoint(roomIndex);

    /*public void ActivateBuff(BuffButtonValues buff)
    {
        player.ActivateBuff(buff.buffType);
        if(buffScreen) buffScreen.SetActive(false);
    }*/

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
