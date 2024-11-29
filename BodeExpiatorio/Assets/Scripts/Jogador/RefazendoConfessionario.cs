using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RefazendoConfessionario : MonoBehaviour
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

    private void Start()
    {
        if (isFirstFromAct) GameManager.Instance.SetHealth(player.Vida.BaseHealth, GameManager.Instance.GetCurrentHealth());

        if (inConfessionRoom)
        {
            return;
        }

        startingRoomTime = Time.time;
        startingRunTime = Time.time;

        GameManager gameManager = GameManager.Instance;
        gameManager.SetCurrentRoom(roomIndex);
        gameManager.SetCurrentPlayer(player);
    }

    public void LoadLastCheckpoint()
    {
        var currentCheckpoint = checkpointList[0];
        for (int i = 0; i < checkpointList.Count; i++) 
        {
            if (checkpointList[i].isActive) currentCheckpoint = checkpointList[i];
        }
        player.SetPosition(currentCheckpoint.checkpointTransform.position);
    }

    private void LoadFirstCheckpoint()
    {
        player.SetPosition(checkpointList[0].checkpointTransform.position);
    }

    private void Respawn()
    {
        if (player.Vida.CurrentMaxHealth <= 0) return;
        player.Vida.CureMaxHealth();
        player.Vida.ResetDamageTakenInRun();

        player.ApplyCure();
        startingRunTime = Time.time;

        player.StopCreepingDamage();
        player.SetBuffs();
    }
}
