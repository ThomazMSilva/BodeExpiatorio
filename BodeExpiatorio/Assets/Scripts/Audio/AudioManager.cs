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
    public VCA generalVCA;
    public List<EventInstance> eventInstances;


    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Debug.LogError("Mais de um AudioManager");
            Destroy(gameObject);
        }
        else _Instance = this;

        eventInstances = new();

        StartCoroutine(InitializeVCAs());
    }

    private IEnumerator InitializeVCAs()
    {
        yield return new WaitForSeconds(1f);
        generalVCA = RuntimeManager.GetVCA(generalVCAPath);
    }

    public void PlayerOneShot(EventReference sound, Vector3 worldPos) => RuntimeManager.PlayOneShot(sound, worldPos);

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
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

    private void OnDestroy() => CleanEventInstanceList();
}
