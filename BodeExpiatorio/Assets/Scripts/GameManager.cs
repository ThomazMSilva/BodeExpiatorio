using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
    
    [SerializeField] private string deathSceneName = "Morte";

    [SerializeField] private List<Sala> rooms;

    public Sala currentRoom;

    public Sala lastCheckpoint;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) LoadNextRoom();
        if (Input.GetKeyDown(KeyCode.H)) LoadLastCheckpoint();
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
