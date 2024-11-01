using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;


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
        if (Input.GetKeyDown(KeyCode.F2)) SceneManager.LoadScene("Desejo1");
        if (Input.GetKeyDown(KeyCode.F3)) StartCoroutine(LoadScreen("Desejo2"));//SceneManager.LoadScene("Desejo 2");
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

        StartCoroutine(LoadScreen(rooms[currentRoom.sceneIndex + 1].sceneName));
        //SceneManager.LoadSceneAsync(rooms[currentRoom.sceneIndex + 1].sceneName);
    }

    public void LoadDeathScene() => SceneManager.LoadScene(deathSceneName);

    public void LoadMainMenu() => SceneManager.LoadScene("Menu 1");

    [Header("Tela de Carregamento"), Space(8f)]

    [SerializeField] private GameObject LoadingScreen;

    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI statisticsText;

    [SerializeField] private float fadeTime = 0.5f;

    private IEnumerator LoadScreen(string sceneName)
    {
        var images = LoadingScreen.GetComponentsInChildren<Image>();
        float[] originalAlpha = new float[images.Length];
        
        for (int i = 0; i < originalAlpha.Length; i++) originalAlpha[i] = images[i].color.a;
        
        loadingText.text = "Loading...";
        statisticsText.text = FindObjectOfType<Confessionario>().LevelStatistics();

        LoadingScreen.SetActive(true);

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeTime;
            for (int i = 0; i < images.Length; i++)
            {
                var color = images[i].color;
                color.a = Mathf.Lerp(0, originalAlpha[i], normalizedTime);
                images[i].color = color;
            }
            yield return null;
        }

        foreach (var image in images)
        {
            var color = image.color;
            color.a = originalAlpha[Array.IndexOf(images, image)];
            image.color = color;
        }

        AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!loadSceneOperation.isDone)
        {
            yield return null;
        }

        loadingText.text = "Press any key to continue.";

        Time.timeScale = 0;
        while (!Input.anyKey || Input.GetMouseButton(0)) yield return null;

        Time.timeScale = 1;

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeTime;
            for (int i = 0; i < images.Length; i++)
            {
                var color = images[i].color;
                color.a = Mathf.Lerp(originalAlpha[i], 0, normalizedTime);
                images[i].color = color;
            }
            yield return null;
        }

        foreach (var image in images)
        {
            var color = image.color;
            color.a = 0;
            image.color = color;
        }

        LoadingScreen.SetActive(false);

        for (int i = 0; i < images.Length; i++)
        {
            var color = images[i].color;
            color.a = originalAlpha[i];
            images[i].color = color;
        }
    }
}

[System.Serializable]
public class Sala
{
    public string sceneName;
    public int sceneIndex;
    public bool isCheckpointActive;
}
