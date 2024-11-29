using UnityEngine;
using FMODUnity;
using FMOD.Studio;
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

    private void OnDestroy() => CleanEventInstanceList();
}


/*
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _Instance;
    public static AudioManager Instance { get => _Instance; }
    public List<EventInstance> eventInstances = new();

    private void Awake()
    {
        _Instance = this;

        eventInstances = new();
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

    private void OnDestroy() => CleanEventInstanceList();
}
 */
