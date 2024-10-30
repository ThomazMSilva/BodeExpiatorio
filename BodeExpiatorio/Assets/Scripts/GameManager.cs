using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    private static GameManager _Instance;
    public static GameManager Instance
    {
        get
        {
            if (!_Instance)
            {
                var prefab = Resources.Load<GameObject>(prefabPath);

                var inScene = prefab ? Instantiate(prefab) : new GameObject("_-GameManager-_");

                _Instance = inScene.GetComponentInChildren<GameManager>();
                if (!_Instance) _Instance = inScene.AddComponent<GameManager>();
                DontDestroyOnLoad(_Instance.transform.root.gameObject);
            }
            return _Instance;
        }
    }
    
    private static readonly string prefabPath = "Prefabs/_-GameManager-_";
    
    private static readonly string currentRoomPref = "CurrentRoom";
    private static readonly string maxHealthPref = "MaxHealth";
    private static readonly string currentHealthPref = "CurrentHealth";
    
    [SerializeField] private string deathSceneName = "Morte";

    [SerializeField] private List<Sala> rooms;

    public Sala currentRoom;

    public Sala lastCheckpoint;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N)) LoadNextRoom();
        if (Input.GetKeyDown(KeyCode.R)) LoadLastCheckpoint();

        if (Input.GetKeyDown(KeyCode.F1)) SceneManager.LoadScene("Menu 1");
        if (Input.GetKeyDown(KeyCode.F2)) SceneManager.LoadScene("Desejo 1");
        if (Input.GetKeyDown(KeyCode.F3)) SceneManager.LoadScene("Desejo 2");
        if (Input.GetKeyDown(KeyCode.F4)) SceneManager.LoadScene("Traicao 1");
        if (Input.GetKeyDown(KeyCode.F5)) SceneManager.LoadScene("Brutal 1");
        if (Input.GetKeyDown(KeyCode.F6)) SceneManager.LoadScene("Brutal 2");
        if (Input.GetKeyDown(KeyCode.F7)) SceneManager.LoadScene("Brutal 3");
        if (Input.GetKeyDown(KeyCode.F8)) SceneManager.LoadScene("TestesMAluk05");
        if (Input.GetKeyDown(KeyCode.F9)) SceneManager.LoadScene("TestesMAluk05 1");
        //if (Input.GetKeyDown(KeyCode.F10)) SceneManager.LoadScene("Teste Momentum 2");
        if (Input.GetKeyDown(KeyCode.F10)) SceneManager.LoadScene("Teste Semeltr");
        if (Input.GetKeyDown(KeyCode.F11)) SceneManager.LoadScene("Teste Arame");
        if (Input.GetKeyDown(KeyCode.F12)) LoadDeathScene();
    }

    public void NewGame()
    {
        //seta os checkpoints a negativo
        for (int i = 0; i < rooms.Count; i++)
        {
            string checkpointState = $"Checkpoint {i}";
            
            PlayerPrefs.SetInt(checkpointState, 0);

            InitializeCheckpointState(checkpointState, i);
        }

        currentRoom = rooms[0];

        PlayerPrefs.SetInt(currentRoomPref, 0);
        SetHealth(100, 100);

        LoadCurrentRoom();
    }

    public void Continue()
    {
        string debug = "";
        for (int i = 0; i < rooms.Count; i++)
        {
            string checkpointState = $"Checkpoint {i}";
            InitializeCheckpointState(checkpointState, i);
            debug += $" {checkpointState} tem o checkpoint {(rooms[i].isCheckpointActive ? "ativo" : "inativo")};";
        }
        Debug.Log(debug);

        if (PlayerPrefs.HasKey(currentRoomPref)) currentRoom = rooms[PlayerPrefs.GetInt(currentRoomPref)];

        else PlayerPrefs.SetInt(currentRoomPref, 0);

        LoadCurrentRoom();
    }

    public void InitializeCheckpointState(string checkpointKey, int checkpointIndex)
    {
        if (PlayerPrefs.HasKey(checkpointKey))
            rooms[checkpointIndex].isCheckpointActive = PlayerPrefs.GetInt(checkpointKey) == 1;
            
        else 
            PlayerPrefs.SetInt(checkpointKey, 0);
    }

    public void SetCurrentRoom(int scene)
    {
        Debug.Log($"A cena atual é a {scene}");
        currentRoom = rooms[scene];
        PlayerPrefs.SetInt(currentRoomPref, scene);
    }

    public void ActivateSceneCheckpoint(int scene)
    {
        Debug.Log($"Ativou o checkpoint {scene}");
        rooms[scene].isCheckpointActive = true;
        lastCheckpoint = rooms[scene];
    }

    public void SetHealth(float maxHealth, float currentHealth)
    {
        PlayerPrefs.SetFloat(maxHealthPref, maxHealth);
        PlayerPrefs.SetFloat(currentHealthPref, currentHealth);
    }
    
    public float GetMaxHealth() 
    {
        if(!PlayerPrefs.HasKey(maxHealthPref)) PlayerPrefs.SetFloat(maxHealthPref, 100f);
        return PlayerPrefs.GetFloat(maxHealthPref);
    }
    
    public float GetCurrentHealth()
    {
        if(!PlayerPrefs.HasKey(currentHealthPref)) PlayerPrefs.SetFloat(currentHealthPref, 100f);
        return PlayerPrefs.GetFloat(currentHealthPref);
    }

    public void LoadCurrentRoom() => SceneManager.LoadScene(currentRoom.sceneName);

    public void LoadLastCheckpoint() => SceneManager.LoadSceneAsync(lastCheckpoint.sceneName);

    public void LoadNextRoom()
    {
        int nextRoom = currentRoom.sceneIndex + 1;
        if (nextRoom > rooms.Count - 1)
        {
            Debug.Log("Tentando carregar sala que não existe na lista do Game Manager");
            return;
        }
        Jogador _player = FindAnyObjectByType<Jogador>();
        SetHealth(_player.Vida.CurrentMaxHealth, _player.Vida.CurrentHealth);
        SceneManager.LoadSceneAsync(rooms[currentRoom.sceneIndex + 1].sceneName);
    }

    public void LoadDeathScene() => SceneManager.LoadScene(deathSceneName);

    public void LoadMainMenu() => SceneManager.LoadScene("Menu 1");
}







[System.Serializable]
public class Sala
{
    public string sceneName;
    public int sceneIndex;
    public bool isCheckpointActive;
}
