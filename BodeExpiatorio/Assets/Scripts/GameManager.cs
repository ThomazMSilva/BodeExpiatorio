using System.Collections.Generic;
using System.Collections;
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
    private static readonly string currentActStatisticsPref = "CurrentActStatistics";
    
    [SerializeField] private string deathSceneName = "Morte";

    [SerializeField] private List<Sala> rooms;

    public Sala currentRoom;

    public Sala lastCheckpoint;

    [SerializeField] private Jogador player;

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

    public void SetCurrentPlayer(Jogador p) => player = p;

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

    public void LoadCurrentRoom() => StartCoroutine(LoadScreen(currentRoom.sceneName));

    public void LoadLastCheckpoint() => StartCoroutine(LoadScreen(lastCheckpoint.sceneName));

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

        StartCoroutine(LoadScreen(rooms[currentRoom.sceneIndex + 1].sceneName, true));
        //SceneManager.LoadSceneAsync(rooms[currentRoom.sceneIndex + 1].sceneName);
    }

    public void LoadDeathScene() => SceneManager.LoadScene(deathSceneName);

    public void LoadMainMenu() => StartCoroutine(LoadScreen("Menu 1"));

    //Loading

    [Header("Tela de Carregamento"), Space(8f)]

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI statisticsText;

    [SerializeField, TextArea] List<string> randomTexts = new();

    [SerializeField] private float fadeTime = 0.5f;

    [SerializeField] float operationTime = 0;
    [SerializeField] float changeTime = 3;

    private string actStatistics = "";

    private float actRunTime;
    private float actRoomTime;

    /// <summary>
    /// x = room damage
    /// y = run damage
    /// z = room fervor
    /// w = run fervor
    /// </summary>
    private Vector4 totalActTorment;


    private string ActText 
    { 
        get =>  
                $"Estatisticas do Ato: \n\n" +
                $"Total Damage Taken: {totalActTorment.x}\n" +
                $"Total Damage in Run: {totalActTorment.y}\n" +
                $"Total Fervor Taken: {totalActTorment.z}\n" +
                $"Total Fervor in Run: {totalActTorment.w}\n" +
                $"Total Time: {actRoomTime}\n" +
                $"Total Run Time: {actRunTime}\n\n\n"+
                $"Specifications:\n\n{actStatistics}";
    }

    private IEnumerator LoadScreen(string sceneName, bool loadingNextLevel = false)
    {

        if (Time.timeScale == 0) Time.timeScale = 1;
        var images = loadingScreen.GetComponentsInChildren<Image>();
        float[] originalAlpha = new float[images.Length];
        
        for (int i = 0; i < originalAlpha.Length; i++) originalAlpha[i] = images[i].color.a;
        
        loadingText.text = "Loading...";

        if (loadingNextLevel)
        {
            var confessionario = FindAnyObjectByType<Confessionario>();

            if (confessionario == null)
            {
                Debug.LogError("Ce ta fazendo merda com o GameManager. " +
                    "Ta tentando carregar o \"proximo nivel\" a partir um nivel que nao tem nem confessionario.");
            }
            else
            {
                actStatistics += $"\n{confessionario.LevelStatistics()}\n";
                totalActTorment += confessionario.TotalTorment();
                actRoomTime += confessionario.RoomTime();
                actRunTime += confessionario.RunTime();

                PlayerPrefs.SetString(currentActStatisticsPref, actStatistics);

                //Cada um dos cases é pro nível 4 de cada Ato
                switch (currentRoom.sceneIndex)
                {
                    case 3:
                        statisticsText.text = ActText;
                        actStatistics = "";
                        totalActTorment = Vector4.zero;
                        actRoomTime = 0;
                        actRunTime = 0;
                        break;

                    case 7:
                        statisticsText.text = ActText;
                        actStatistics = "";
                        totalActTorment = Vector4.zero;
                        actRoomTime = 0;
                        actRunTime = 0;
                        break;

                    case 11:
                        statisticsText.text = ActText;
                        actStatistics = "";
                        totalActTorment = Vector4.zero;
                        actRoomTime = 0;
                        actRunTime = 0;
                        //Muda a imagem da animacao aq pra ele subindo tb
                        break;

                    default:
                        statisticsText.text = confessionario.LevelStatistics();
                        break;
                }

                SetBuffButtonValues(confessionario.TotalTorment().y, confessionario.damageThreshold);
            }
        }
        else statisticsText.text = randomTexts[UnityEngine.Random.Range(0, randomTexts.Count - 1)];

        //Ativa as imagens e da Fade In
        
        loadingScreen.SetActive(true);

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

        //Completa o Fade In
        
        foreach (var image in images)
        {
            var color = image.color;
            color.a = originalAlpha[Array.IndexOf(images, image)];
            image.color = color;
        }


        //Espera a cena carregar
        
        AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(sceneName);

        operationTime = 0;

        while (!loadSceneOperation.isDone)
        {
            if (!loadingNextLevel)
            {
                operationTime += Time.unscaledDeltaTime;
                if (operationTime > changeTime)
                {
                    statisticsText.text = randomTexts[UnityEngine.Random.Range(0, randomTexts.Count - 1)];
                    operationTime = 0;
                }
            }
            yield return null;
        }


        //Espera input do Jogador

        loadingText.text = "Press any key to continue.";

        Time.timeScale = 0;

        while (!Input.anyKey || Input.GetMouseButton(0))
        {
            if (!loadingNextLevel)
            {
                operationTime += Time.unscaledDeltaTime;
                if (operationTime > changeTime)
                {
                    statisticsText.text = randomTexts[UnityEngine.Random.Range(0, randomTexts.Count - 1)]; ;
                    operationTime = 0;
                }
            }
            yield return null;
        }

        Time.timeScale = 1;


        //Fade Out nas imagens
        
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

        loadingScreen.SetActive(false);


        //Reseta os valores das imagens
        
        for (int i = 0; i < images.Length; i++)
        {
            var color = images[i].color;
            color.a = originalAlpha[i];
            images[i].color = color;
        }
    }

    [Header("Tela de Buff"), Space(8f)]

    [SerializeField] private GameObject buffScreen;
    [SerializeField] private BuffButton buffButtonA;
    [SerializeField] private BuffButton buffButtonB;
    [SerializeField] private BuffButton buffButtonC;
    
    [Space(8f)]

    [SerializeField] private List<BuffButtonValues> salvationBuffs;
    [SerializeField] private List<BuffButtonValues> reluctanceBuffs;

    private void SetBuffButtonValues(float lastRoomTorment, float lastRoomDmgThreshold)
    {
        bool salvationPath = lastRoomTorment < lastRoomDmgThreshold;

        buffButtonA.buffButton.SetValues(GetRandomBuff(salvationPath));

        buffButtonB.buffButton.SetValues(GetRandomBuff(!salvationPath));

        buffButtonC.buffButton.SetValues(GetRandomBuff(salvationPath));
    }

    private BuffButtonValues GetRandomBuff(bool salvation)
    {
        var buffList = salvation ? salvationBuffs : reluctanceBuffs;
        return buffList[UnityEngine.Random.Range(0, buffList.Count - 1)];
    }

    public void ActivateBuff(BuffButton buff)
    {
        Jogador player = FindAnyObjectByType<Jogador>();
        player.ActivateBuff(buff.buffButton.buffType);
        CloseBuffScreen();
    }
    public void CloseBuffScreen() { if(buffScreen) buffScreen.SetActive(false); }
}

[System.Serializable]
public class Sala
{
    public string sceneName;
    public int sceneIndex;
    public bool isCheckpointActive;
}
