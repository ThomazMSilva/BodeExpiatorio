using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Confessionario : MonoBehaviour
{
    [SerializeField] private List<Checkpoint> checkpointList = new();
    private JogadorReference player;

    [Space(8f)]

    [SerializeField] private int roomIndex;
    public int roomActIndex { get;  private set; }

    [SerializeField] private bool isFirstFromAct;
    public bool IsFirstFromAct { get => isFirstFromAct; }

    [SerializeField] private bool inConfessionRoom;
    public bool InConfessionRoom { get => inConfessionRoom;}

    [SerializeField] private float minimumDamagePossible = 100;
    public float MinimumDamagePossible { get => minimumDamagePossible; }
    [SerializeField] private float possibleDamageMargin = 20;

    [SerializeField] private float damageThreshold = 50;
    public float DamageThreshold { get=> damageThreshold;}

    private float startingRunTime;
    private float startingRoomTime;

    private void Awake()
    {
        player = FindAnyObjectByType<JogadorReference>();
    }

    private void OnEnable()
    {
        player.Vida.OnPlayerDeath += Respawn;
        player.Vida.OnPlayerTrueDeath += GameOver;
    }

    private void Start()
    {
        roomIndex = SceneManager.GetActiveScene().buildIndex - 1;
        isFirstFromAct = roomIndex % 2 == 0;
        inConfessionRoom = roomIndex == 8;

        roomActIndex = (roomIndex / 2) + 1;

        checkpointList[0].isActive = true;
        for (int i = 1; i < checkpointList.Count; i++)
        {
            checkpointList[i].OnPrayed += Checkpoint_OnPrayed;
            checkpointList[i].isActive = PlayerPrefs.GetInt($"Checkpoint_Level_{roomIndex}.{i}") == 1;
        }

        if (player.Vida.CurrentMaxHealth < minimumDamagePossible + possibleDamageMargin) 
        {
            if (player.Vida.CurrentMaxHealth > minimumDamagePossible)
                Debug.LogWarning("Sua vida máxima tá no limite do que dá pra passar nesse nível. Você pode querer considerar voltar do começo do Ato, usando o menu de pause.");
            else
                Debug.LogWarning("Sua vida máxima é insuficiente pra passar nível. Retorne ao confessionário do comeco do Ato, usando o menu de pause.");
        }
        //PlayerPrefs.SetInt($"Checkpoint_Level_{roomIndex}.{0}", 0);


        if (inConfessionRoom)
        {
            return;
        }

        GameManager gameManager = GameManager.Instance;

        if (isFirstFromAct)
        {
            GameManager.Instance.SetHealth(player.Vida.BaseHealth, GameManager.Instance.GetCurrentHealth());
            gameManager.SetCurrentFirstFromAct(roomIndex);
            gameManager.ActivateSceneCheckpoint(roomIndex);
            gameManager.SetCurrentPlayer(player);
        }

        startingRoomTime = Time.time;
        startingRunTime = Time.time;

        
        gameManager.SetCurrentRoom(roomIndex);
        Spawn();
    }

    private void Checkpoint_OnPrayed(Checkpoint checkpoint)
    {
        PlayerPrefs.SetInt($"Checkpoint_Level_{roomIndex}.{checkpointList.IndexOf(checkpoint)}", 1);
        GameManager.Instance.ActivateSceneCheckpoint(roomIndex);
    }

    private void ResetCheckpoints()
    {
        checkpointList[0].isActive = true;
        PlayerPrefs.SetInt($"Checkpoint_Level_{roomIndex}.{checkpointList[0]}", 1);
        for (int i = 1; i < checkpointList.Count; i++)
        {
            checkpointList[i].isActive = false;
            PlayerPrefs.SetInt($"Checkpoint_Level_{roomIndex}.{checkpointList.IndexOf(checkpointList[i])}", 0);
        }
    }

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



/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Confessionario : MonoBehaviour
{
    [SerializeField] private int roomIndex;
    [SerializeField] private bool checkpointStartsActive;
    [Tooltip("Quando o Jogador morre, a vida máxima volta para o valor que estava quando entrou na câmara.")]
    [SerializeField] private bool recoverMaxHealthToStartingPoint = true;
    [SerializeField] private bool inConfessionRoom;
    public bool InConfessionRoom { get => inConfessionRoom; }

    [SerializeField] private bool isFinalRoom;
    public bool IsFinalRoom { get => isFinalRoom; }

    [SerializeField] private Transform respawnPoint;
    [SerializeField] private GameObject buffScreen;

    public float damageThreshold = 50f;
    private float startingRoomTime;
    private float startingRunTime;

    private JogadorReference player;
    public JogadorReference Player { get => player; }

    public UnityEvent OnConfessioned;

    public static Confessionario ultimoAtivo;

    private void Awake() => player = FindAnyObjectByType<JogadorReference>();

    private void Start()
    {
        Spawn();

        if (!IsFinalRoom) GameManager.Instance.SetHealth(player.Vida.BaseHealth, GameManager.Instance.GetCurrentHealth());

        if (inConfessionRoom)
        {
            return;
        }
        
        startingRoomTime = Time.time;
        startingRunTime = Time.time;

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
        //if (ultimoAtivo != this) return;
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
        other.GetComponent<ContagemRegressivaVidaJogador>().isCountDownActive = false;
        Entrada.Instance.OnKneelButtonDown += Pray;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        other.GetComponent<ContagemRegressivaVidaJogador>().isCountDownActive = true;
        Entrada.Instance.OnKneelButtonDown -= Pray;
    }

    private void Pray()
    {
        OnConfessioned?.Invoke();
        if (!inConfessionRoom)
        {
            GameManager.Instance.ActivateSceneCheckpoint(roomIndex);
            SetUltimoAtivo();
            return;
        }
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

    public string LevelStatistics() => $"--Level {roomIndex + 1} Statistics--\n\nRoomTime: {RoomTime()} seconds; RunTime: {RunTime()} seconds.\n{player.Vida.DamageString}";

    public float RoomTime() => Time.time - startingRoomTime;

    public float RunTime() => Time.time - startingRunTime;

    public float DamageTakenRoom() => player.Vida.DamageTakenInRoom;

    public float DamageTakenRun() => player.Vida.DamageTakenInRun;

    public float FervorTakenRoom() => player.Vida.FervorTakenInRoom;

    public float FervorTakenRun() => player.Vida.FervorTakenInRun;

    public Vector4 TotalTorment() => new(DamageTakenRoom(), DamageTakenRun(), FervorTakenRoom(), FervorTakenRun());
}
*/
