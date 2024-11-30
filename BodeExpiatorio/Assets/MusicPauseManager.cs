using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MusicPauseManager : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter musicEmitter; 
    [SerializeField] private List<GameObject> itemsToMonitor; 

    private bool isPaused = false;

    private void Update()
    {
       
        bool anyActive = itemsToMonitor.Exists(item => item.activeSelf);

        if (anyActive && !isPaused)
        {
            PauseMusic();
        }
        else if (!anyActive && isPaused)
        {
            ResumeMusic();
        }
    }

    private void PauseMusic()
    {
        if (musicEmitter.EventInstance.isValid())
        {
            musicEmitter.EventInstance.setPaused(true);
            isPaused = true;
        }
    }

    private void ResumeMusic()
    {
        if (musicEmitter.EventInstance.isValid())
        {
            musicEmitter.EventInstance.setPaused(false);
            isPaused = false;
        }
    }
}
