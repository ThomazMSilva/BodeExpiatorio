using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CutsceneInicial : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;

    private void Start()
    {
        StartCoroutine(CheckPlayingState());
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            videoPlayer.Stop();
        }

    }

    IEnumerator CheckPlayingState()
    {
        yield return new WaitForSeconds(1f);
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }
        Debug.Log("Acabou video");
        GameManager.Instance.LoadMainMenu();
    }
}
