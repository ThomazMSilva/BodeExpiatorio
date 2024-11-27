using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _Instance;
    private const string prefabPath = "Prefabs/Audio";
    public static AudioManager Instance
    {
        get
        {
            if (!_Instance)
            {
                var prefab = Resources.Load<GameObject>(prefabPath);

                var inScene = Instantiate(prefab);

                _Instance = inScene.GetComponentInChildren<AudioManager>();

                if (!_Instance) { _Instance = inScene.AddComponent<AudioManager>();Debug.Log("ainda assim n tinha csgido"); }
            }
            return _Instance;
        }
    }

    [SerializeField] private string generalVCAPath = "vca:/SFX";
    [SerializeField] private string sfxVCAPath = "vca:/SFX";
    [SerializeField] private string musicVCAPath = "vca:/Music";
    public VCA generalVCA;
    public VCA sfxVCA;
    public VCA musicVCA;
    public List<EventInstance> eventInstances = new();


    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Debug.LogError("Mais de um AudioManager");
            Destroy(gameObject);
        }
        else _Instance = this;

        eventInstances = new();

        //StartCoroutine(InitializeVCAs());
    }

    private IEnumerator InitializeVCAs()
    {
        yield return new WaitForSeconds(1f);

        generalVCA = RuntimeManager.GetVCA(generalVCAPath);
        sfxVCA = RuntimeManager.GetVCA(sfxVCAPath);
        musicVCA = RuntimeManager.GetVCA(musicVCAPath);

        if (generalVCA.isValid() && sfxVCA.isValid() && musicVCA.isValid())
        {
            Debug.Log("VCAs inicializados com sucesso.");
        }
        else
        {
            Debug.LogError("Erro ao inicializar um ou mais VCAs.");
        }
    }

    public void PlayerOneShot(EventReference sound, Vector3 worldPos) => RuntimeManager.PlayOneShot(sound, worldPos);

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        Debug.Log($"Comecou a tentar criar instancia de {eventReference}");
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        Debug.Log($"O que saiu foi a instancia {eventInstance}");
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    private void CleanEventInstanceList()
    {
        foreach(var eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    public void SetGeneralVolume(float volume) => generalVCA.setVolume(volume);
    public void SetSFXVolume(float volume) => sfxVCA.setVolume(volume);
    public void SetMusicVolume(float volume) => musicVCA.setVolume(volume);

    private void OnDestroy() => CleanEventInstanceList();
}
