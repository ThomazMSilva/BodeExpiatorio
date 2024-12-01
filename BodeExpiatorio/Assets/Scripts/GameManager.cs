using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

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
    private static readonly string currentFirstRoomPref = "CurrentFirstFromAct";
    //private static readonly string currentCheckpointPref = "CurrentCheckpoint";
    private static readonly string maxHealthPref = "MaxHealth";
    private static readonly string currentHealthPref = "CurrentHealth";
    private static readonly string currentActStatisticsPref = "CurrentActStatistics";
    private static readonly string totalStatisticsPref = "TotalStatistics";
    
    [SerializeField] private string deathSceneName = "Morte";
    [SerializeField] private string confessionSceneName = "Confessionario";
    [SerializeField] private string menuSceneName = "Menu Inicial";
    [SerializeField] private string finalSceneName = "Final";

    [SerializeField] private List<Sala> rooms;

    public Sala currentRoom;
    public Sala currentFirstRoomFromAct;

    public Sala lastCheckpoint;
    [SerializeField] private int checkpointsPerRoom = 11;

    private bool isLoadingScene;
    public bool IsLoading { get => isLoadingScene; }

    [SerializeField] private JogadorReference player;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N)) LoadNextRoom();
        if (Input.GetKeyDown(KeyCode.T)) ResetCheckpointsOver();
        if (Input.GetKeyDown(KeyCode.R)) LoadLastCheckpoint();

        if (Input.GetKeyDown(KeyCode.F1)) SceneManager.LoadScene("Menu 1");
        if (Input.GetKeyDown(KeyCode.F2)) SceneManager.LoadScene("Desejo1");
        if (Input.GetKeyDown(KeyCode.F3)) StartCoroutine(LoadScreen("Desejo2"));//SceneManager.LoadScene("Desejo 2");
        if (Input.GetKeyDown(KeyCode.F4)) SceneManager.LoadScene("Traicao 1");
        if (Input.GetKeyDown(KeyCode.F5)) SceneManager.LoadScene("Brutal 1 Modelos Novos");
        if (Input.GetKeyDown(KeyCode.F6)) SceneManager.LoadScene("Brutal 2 Mobiliada");
        if (Input.GetKeyDown(KeyCode.F7)) SceneManager.LoadScene("Brutal 3");
        if (Input.GetKeyDown(KeyCode.F8)) SceneManager.LoadScene("TestesMAluk05");
        if (Input.GetKeyDown(KeyCode.F9)) SceneManager.LoadScene("TestesMAluk05 1");
        //if (Input.GetKeyDown(KeyCode.F10)) SceneManager.LoadScene("Teste Momentum 2");
        if (Input.GetKeyDown(KeyCode.F10)) SceneManager.LoadScene("Teste Semeltr");
        if (Input.GetKeyDown(KeyCode.F11)) SceneManager.LoadScene("Teste Arame");
        if (Input.GetKeyDown(KeyCode.F12)) LoadDeathScene();
        if (Input.GetKeyDown(KeyCode.PageDown)) ResetPlayerConfigs();
        //Debug.Log(Time.timeScale);
    }

    public void ResetPlayerConfigs() => PlayerPrefs.DeleteAll();

    public void NewGame()
    {
        ResetCheckpointsOver();

        currentRoom = rooms[0];

        PlayerPrefs.SetInt(currentRoomPref, 0);
        PlayerPrefs.SetString(currentActStatisticsPref, "");
        SetHealth(100, 100);

        LoadCurrentRoom();
    }

    public void ResetCheckpointsOver(int initialIndex = 0)
    {
        //seta os checkpoints a negativo
        for (int i = initialIndex; i < rooms.Count; i++)
        {
            string checkpointIndex = $"Checkpoint {i}";
            for (int j = 1; j < checkpointsPerRoom; j++) 
            { 
                string checkpointLevel = $"Checkpoint_Level_{i}.{j}";
                PlayerPrefs.SetInt(checkpointLevel, 0);
            }

            PlayerPrefs.SetInt(checkpointIndex, 0);

            InitializeCheckpointState(checkpointIndex, i);
        }
    }

    public void Continue()
    {
        string debug = "";
        for (int i = 0; i < rooms.Count; i++)
        {
            string checkpointIndex = $"Checkpoint {i}";
            InitializeCheckpointState(checkpointIndex, i);
            debug += $" {checkpointIndex} tem o checkpoint {(rooms[i].isCheckpointActive ? "ativo" : "inativo")};";
        }
        Debug.Log(debug);

        if (PlayerPrefs.HasKey(currentRoomPref)) currentRoom = rooms[PlayerPrefs.GetInt(currentRoomPref)];

        else PlayerPrefs.SetInt(currentRoomPref, 0);


        if (PlayerPrefs.HasKey(currentActStatisticsPref)) actStatistics = PlayerPrefs.GetString(currentActStatisticsPref);

        else PlayerPrefs.SetString(currentActStatisticsPref, "");


        if (PlayerPrefs.HasKey(totalStatisticsPref)) totalPenitence = PlayerPrefs.GetString(totalStatisticsPref);
        
        else PlayerPrefs.SetString(totalStatisticsPref, "");

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
        //Debug.Log($"A cena atual é a {scene}");
        currentRoom = rooms[scene];
        ResetCheckpointsOver(scene);
        PlayerPrefs.SetInt(currentRoomPref, scene);
    }

    public void SetCurrentFirstFromAct(int scene)
    {
        //Debug.Log($"A cena atual é a {scene}");
        currentFirstRoomFromAct = rooms[scene];
        ResetCheckpointsOver(scene);
        PlayerPrefs.SetInt(currentFirstRoomPref, scene);
    }

    public void SetCurrentPlayer(JogadorReference p) => player = p;

    public void ActivateSceneCheckpoint(int scene)
    {
        Debug.Log($"Ativou o checkpoint {scene}");
        rooms[scene].isCheckpointActive = true;
        //lastCheckpoint = rooms[scene];
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

    public void LoadLastCheckpoint()
    {
        if (currentFirstRoomFromAct.sceneName == string.Empty)
        {
            Debug.LogWarning("Nao encontrada \"Primeira sala do Ato Atual\". Carregando primeiro nivel.");
            StartCoroutine(LoadScreen(rooms[0].sceneName));
            return;
        }
        StartCoroutine(LoadScreen(currentFirstRoomFromAct.sceneName));
    }

    public void LoadNextRoom()
    {
        int nextRoom = currentRoom.sceneIndex + 1;
        if (nextRoom > rooms.Count - 1)
        {
            var confessionario = FindAnyObjectByType<Confessionario>();
            if (confessionario.InConfessionRoom)
            {
               LoadFinalScene();
               return;
            }
            //Debug.Log($"Tentando carregar sala que não existe na lista do Game Manager; CurrentScene: {currentRoom}; CurrentUIndex: {currentRoom.sceneIndex};  Tried Index: {nextRoom}");
            
        }
        JogadorReference _player = FindAnyObjectByType<JogadorReference>();
        SetHealth(_player.Vida.CurrentMaxHealth, _player.Vida.CurrentHealth);

        StartCoroutine(LoadScreen(rooms[currentRoom.sceneIndex + 1].sceneName, true));
        //SceneManager.LoadSceneAsync(rooms[currentRoom.sceneIndex + 1].sceneName);
    }

    public void LoadConfessionBooth()
    {
        JogadorReference _player = FindAnyObjectByType<JogadorReference>();
        SetHealth(_player.Vida.CurrentMaxHealth, _player.Vida.CurrentHealth);
        StartCoroutine(LoadScreen(confessionSceneName, true));
    }

    public void LoadMainMenu() => StartCoroutine(LoadScreen(menuSceneName));

    public void LoadDeathScene() => SceneManager.LoadScene(deathSceneName);

    public void LoadFinalScene() => SceneManager.LoadScene(finalSceneName);

    //Loading



    [Header("Tela de Carregamento"), Space(8f)]

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI statisticsText;
    [SerializeField] private Scrollbar statisticsScrollbar;

    [SerializeField, TextArea] List<string> randomTexts = new();

    [SerializeField] private float fadeTime = 0.5f;

    [SerializeField] float operationTime = 0;
    [SerializeField] float changeTime = 3;

    private string actStatistics = "";
    private string currentActName = "";
    private string CurrentActName(int index) =>
        index switch
        {
            2 => "Traição",
            3 => "Brutalidade",
            _ => "Desejo"
        };

    private float actRunTime;
    private float actRoomTime;

    private string totalPenitence;
    /// <summary>
    /// x = room damage
    /// y = run damage
    /// z = room fervor
    /// w = run fervor
    /// </summary>
    private Vector4 totalActTorment;
    /// <summary>
    /// x = room damage
    /// y = run damage
    /// z = room fervor
    /// w = run fervor
    /// </summary>
    private Vector4 totalGameTorment;

    public bool InPathOfRenitence(TormentType tormentType, float threshold) => totalGameTorment[(int)tormentType] < threshold;

    private string ActText 
    { 
        get =>  
                $"Estatisticas do Ato de {currentActName}: \n\n" +
                $"Total Damage Taken: {totalActTorment.x}\n" +
                $"Total Damage in Run: {totalActTorment.y}\n" +
                $"Total Fervor Taken: {totalActTorment.z}\n" +
                $"Total Fervor in Run: {totalActTorment.w}\n" +
                $"Total Time: {actRoomTime}\n" +
                $"Total Run Time: {actRunTime}\n\n\n"+
                $"Specifications:\n\n{actStatistics}";
    }

    
    private Image[] images = new Image[0];
    private float[] originalAlpha = new float[0];

    private IEnumerator LoadScreen(string sceneName, bool loadingNextLevel = false)
    {
        isLoadingScene = true;

        //EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(statisticsScrollbar.gameObject);

        if (images.Length < 1) InitializeOriginalAlphas();

        loadingText.text = "Loading...";
        loadingScreen.SetActive(true);

        Time.timeScale = 1;
        
        UpdateStatistics(loadingNextLevel);


        //Fade In

        yield return FadeIn(images, originalAlpha, fadeTime);

        //Espera a cena carregar
        
        yield return LoadSceneAsync(sceneName, loadingNextLevel);

        //Espera input do JogadorReference

        loadingText.text = "Press any key to continue.";

        Time.timeScale = 0;

        yield return WaitForInput(loadingNextLevel);

        //Fade Out

        yield return FadeOut(images, originalAlpha, fadeTime);

        
        if(!loadingNextLevel) isLoadingScene = false;

        //Reseta os valores das imagens

        SetImageAlphas(images, originalAlpha);
    }

    private IEnumerator LoadSceneAsync(string sceneName, bool loadingNextLevel)
    {
        AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(sceneName);
        operationTime = 0;


        while (!loadSceneOperation.isDone)
        {
            if (!loadingNextLevel)
            {
                operationTime += Time.unscaledDeltaTime;
                if (operationTime > changeTime)
                {
                    statisticsText.text = randomTexts[UnityEngine.Random.Range(0, randomTexts.Count)];
                    operationTime = 0;
                }
            }
            yield return null;
        }
    }

    private IEnumerator WaitForInput(bool loadingNextLevel)
    {
        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;

        EventSystem.current.SetSelectedGameObject(statisticsScrollbar.gameObject);

        while (!IsNonNavigationInputPressed())
        {
            if (!loadingNextLevel)
            {
                operationTime += Time.unscaledDeltaTime;
                if (operationTime > changeTime)
                {
                    statisticsText.text = randomTexts[UnityEngine.Random.Range(0, randomTexts.Count)];
                    operationTime = 0;
                }
            }

            yield return null;
        }

        EventSystem.current.SetSelectedGameObject(selectedObj);

        if (!loadingNextLevel || FindAnyObjectByType<Confessionario>().InConfessionRoom) 
        {
            EndLoading();
            Time.timeScale = 1; 
        }

        else
        {
            buffScreen.SetActive(true);
            EventSystem.current.SetSelectedGameObject(buffButtonA.gameObject);
        }
    }

    private bool IsNonNavigationInputPressed()
    {
        if (Keyboard.current.anyKey.isPressed && !IsUINavigationKeyPressed())
            return true;

        if (Gamepad.all.Count > 0)
        {
            foreach (var gamepad in Gamepad.all)
            {
                if (gamepad.allControls.Any(control =>
                    control is ButtonControl button &&
                    button.IsPressed() &&
                    !button.synthetic &&
                    !IsUINavigationButton(button)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsUINavigationKeyPressed()
    {
        return Keyboard.current.upArrowKey.isPressed ||
               Keyboard.current.downArrowKey.isPressed ||
               Keyboard.current.leftArrowKey.isPressed ||
               Keyboard.current.rightArrowKey.isPressed ||
               Keyboard.current.tabKey.isPressed;
    }

    private bool IsUINavigationButton(ButtonControl button)
    {
        return button == Gamepad.current.dpad.up ||
               button == Gamepad.current.dpad.down ||
               button == Gamepad.current.dpad.left ||
               button == Gamepad.current.dpad.right ||
               button == Gamepad.current.leftStick.up ||
               button == Gamepad.current.leftStick.down ||
               button == Gamepad.current.leftStick.left ||
               button == Gamepad.current.leftStick.right;
    }

    private IEnumerator FadeOut(Image[] images, float[] originalAlpha, float fadeTime)
    {
        for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
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

        SetImageAlphas(images);

        loadingScreen.SetActive(false);
    }

    private IEnumerator FadeIn(Image[] images, float[] originalAlpha, float fadeTime)
    {
        loadingScreen.SetActive(true);

        for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
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

        SetImageAlphas(images, originalAlpha);
    }

    private void InitializeOriginalAlphas()
    {
        images = loadingScreen.GetComponentsInChildren<Image>();
        originalAlpha = new float[images.Length];
        for (int i = 0; i < images.Length; i++) originalAlpha[i] = images[i].color.a;
    }

    private void ResetStatistics()
    {
        actStatistics = "";
        totalActTorment = Vector4.zero;
        actRoomTime = 0;
        actRunTime = 0;
        PlayerPrefs.SetString(currentActStatisticsPref, actStatistics);
    }


    private void UpdateStatistics(bool loadingNextLevel)
    {
        if (loadingNextLevel)
        {
            var confessionario = FindAnyObjectByType<Confessionario>();
            if (confessionario == null)
            {
                Debug.LogError("Ce ta fazendo merda com o GameManager. " +
                    "Ta tentando carregar o \"proximo nivel\" a partir um nivel que nao tem nem confessionario.");
                return;
            }

            if (confessionario.InConfessionRoom)
            {
                statisticsText.text = randomTexts[UnityEngine.Random.Range(0, randomTexts.Count - 1)];
                return;
            }

            //actStatistics = PlayerPrefs.GetString(currentActStatisticsPref);
            currentActName = CurrentActName(confessionario.roomActIndex);
            actStatistics += $"\n{confessionario.LevelStatistics()}\n";
            totalActTorment += confessionario.TotalTorment();
            actRoomTime += confessionario.RoomTime();
            actRunTime += confessionario.RunTime();
            totalGameTorment += confessionario.TotalTorment();

            PlayerPrefs.SetString(currentActStatisticsPref, ActText);
            

            if (!confessionario.IsFirstFromAct)
            {
                statisticsText.text = ActText;
                totalPenitence += statisticsText.text;
                ResetStatistics(); 
                PlayerPrefs.SetString(totalStatisticsPref, totalPenitence);
            }
            else
            {
                statisticsText.text = confessionario.LevelStatistics();
            }
            SetBuffButtonValues(confessionario.TotalTorment().y, confessionario.DamageThreshold);
        }
        else
        {
            statisticsText.text = randomTexts[UnityEngine.Random.Range(0, randomTexts.Count - 1)];
        }
    }

    private void SetImageAlphas(Image[] images, float[] intendedAlpha = null)
    {
        for (int i = 0; i < images.Length; i++)
        {
            var color = images[i].color;
            color.a = intendedAlpha != null ? intendedAlpha[i] : 0;
            images[i].color = color;
        }
    }

    public void EndLoading() => isLoadingScene = false;

    //Buffs



    [Header("Tela de Buff"), Space(8f)]

    [SerializeField] private GameObject buffScreen;
    [SerializeField] private TextMeshProUGUI descriptionTMP;
    [SerializeField] private BuffButton buffButtonA;
    [SerializeField] private BuffButton buffButtonB;
    [SerializeField] private BuffButton buffButtonC;
    
    [Space(8f)]

    [SerializeField] private List<BuffButtonValues> salvationBuffs;
    [SerializeField] private List<BuffButtonValues> reluctanceBuffs;

    public void CloseBuffScreen() 
    { 
        if(buffScreen) buffScreen.SetActive(false); 
        Time.timeScale = 1;
        EndLoading();
    }

    private void SetBuffButtonValues(float lastRoomTorment, float lastRoomDmgThreshold)
    {
        bool isOnSalvationPath = lastRoomTorment < lastRoomDmgThreshold;

        var buffA = GetRandomBuff(isOnSalvationPath); 
        buffButtonA.buffButton.SetValues(buffA);

        buffButtonB.buffButton.SetValues(GetRandomBuff(!isOnSalvationPath));

        buffButtonC.buffButton.SetValues(GetRandomBuff(isOnSalvationPath, buffA));
    }

    private BuffButtonValues GetRandomBuff(bool salvation, BuffButtonValues excludedBuff = null)
    {
        var buffList = salvation ? salvationBuffs : reluctanceBuffs;
        var filteredList = excludedBuff != null ? buffList.Where(buff => buff != excludedBuff) : buffList;
        return buffList[UnityEngine.Random.Range(0, buffList.Count)];
    }
    
    public void SetScreenSelected() => EventSystem.current.SetSelectedGameObject(buffButtonA.gameObject);
    
    public void ActivateBuff(BuffButton buff)
    {
        JogadorReference player = FindAnyObjectByType<JogadorReference>();
        player.ActivateBuff(buff.buffButton.buffType);
        CloseBuffScreen();
    }

    public void SetDescriptionText(BuffButton buff) => descriptionTMP.text = buff.buffButton.buffDescription;
    
    public void EmptyDescriptionText() => descriptionTMP.text = "";

}

[System.Serializable]
public class Sala
{
    public string sceneName;
    public int sceneIndex;
    public bool isCheckpointActive;
}

[System.Serializable]
public enum TormentType
{
    CheckRoomDamage,
    CheckRunDamage,
    CheckRoomFervor,
    CheckRunFervor
}
