using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private List<Checkpoint> checkpointList = new();
    [SerializeField] private JogadorReference player;

    [Space(8f)]

    [SerializeField] private int roomIndex;

    [SerializeField] private bool isFirstFromAct;
    public bool IsFirstFromAct { get => isFirstFromAct; }

    [SerializeField] private bool inConfessionRoom;
    [SerializeField] private float damageThreshold;

    private float startingRunTime;
    private float startingRoomTime;

    private void OnEnable()
    {
        player.Vida.OnPlayerDeath += Respawn;
        player.Vida.OnPlayerTrueDeath += GameOver;
    }

    private void Start()
    {
        checkpointList[0].isActive = true;
        for (int i = 1; i < checkpointList.Count; i++)
        {
            checkpointList[i].OnPrayed += Checkpoint_OnPrayed;
            checkpointList[i].isActive = PlayerPrefs.GetInt($"Checkpoint_Level_{roomIndex}.{i}") == 1;
        }

        Spawn();
        //PlayerPrefs.SetInt($"Checkpoint_Level_{roomIndex}.{0}", 0);

        if (isFirstFromAct) GameManager.Instance.SetHealth(player.Vida.BaseHealth, GameManager.Instance.GetCurrentHealth());

        if (inConfessionRoom)
        {
            return;
        }

        startingRoomTime = Time.time;
        startingRunTime = Time.time;

        GameManager gameManager = GameManager.Instance;
        gameManager.SetCurrentRoom(roomIndex);
        if(IsFirstFromAct) gameManager.SetCurrentFirstFromAct(roomIndex);
        gameManager.SetCurrentPlayer(player);
    }

    private void Checkpoint_OnPrayed(Checkpoint checkpoint)
    {
        PlayerPrefs.SetInt($"Checkpoint_Level_{roomIndex}.{checkpointList.IndexOf(checkpoint)}", 1);
    }

    /*private void ResetCheckpoints()
    {
        checkpointList[0].isActive = true;
        PlayerPrefs.SetInt($"Checkpoint_Level_{roomIndex}.{checkpointList[0]}", 1);
        for (int i = 1; i < checkpointList.Count; i++)
        {
            checkpointList[i].isActive = false;
            PlayerPrefs.SetInt($"Checkpoint_Level_{roomIndex}.{checkpointList.IndexOf(checkpointList[i])}", 0);
        }
    }*/

    public void LoadLastActiveCheckpoint()
    {
        var currentCheckpoint = checkpointList[0];
        for (int i = 0; i < checkpointList.Count; i++) 
        {
            if (checkpointList[i].isActive) currentCheckpoint = checkpointList[i];
        }
        player.SetPosition(currentCheckpoint.checkpointTransform.position);
    }

    public void LoadFirstCheckpoint()
    {
        player.SetPosition(checkpointList[0].checkpointTransform.position);
    }

    private void Spawn()
    {
        player.StopCreepingDamage();
        player.SetBuffs();
        LoadLastActiveCheckpoint();

    }

    private void Respawn()
    {
        if (player.Vida.CurrentMaxHealth <= 0) return;
        player.Vida.CureMaxHealth();
        player.Vida.ResetDamageTakenInRun();

        player.ApplyCure();
        startingRunTime = Time.time;
        Spawn();
    }

    private void GameOver()
    {
        ResetCheckpoints();
        GameManager.Instance.LoadDeathScene();
    }

    public string LevelStatistics() => 
        $"--Level {roomIndex + 1} Statistics--\n" +
        $"\nRoomTime: {RoomTime()} seconds; " +
        $"\nRunTime: {RunTime()} seconds." +
        $"\n{player.Vida.DamageString}";

    public float RoomTime() => Time.time - startingRoomTime;

    public float RunTime() => Time.time - startingRunTime;

    public float DamageTakenRoom() => player.Vida.DamageTakenInRoom;

    public float DamageTakenRun() => player.Vida.DamageTakenInRun;

    public float FervorTakenRoom() => player.Vida.FervorTakenInRoom;

    public float FervorTakenRun() => player.Vida.FervorTakenInRun;

    public Vector4 TotalTorment() => new(DamageTakenRoom(), DamageTakenRun(), FervorTakenRoom(), FervorTakenRun());
}
