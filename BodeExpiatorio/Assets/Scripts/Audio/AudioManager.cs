using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private string generalVCAPath = "vca:/SFX";
    public VCA generalVCA;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Mais de um AudioManager");
        }
        Instance = this;
        StartCoroutine(InitializeVCAs());
    }

    private IEnumerator InitializeVCAs()
    {
        yield return new WaitForSeconds(1f);
        generalVCA = RuntimeManager.GetVCA(generalVCAPath);

    }


    public void PlayerOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public void SetGeneralVolume(float volume)
    {
        generalVCA.setVolume(volume);
    }
}
