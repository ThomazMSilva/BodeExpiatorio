using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Video;
using System.Linq;

public class CutsceneInicial : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;

    private void Start()
    {
        StartCoroutine(CheckPlayingState());
    }

    private void Update()
    {
        if (IsAnyInputPressed())
        {
            videoPlayer.Stop();
        }

    }

    private bool IsAnyInputPressed()
    {
        if (Keyboard.current.anyKey.isPressed)
            return true;

        if (Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed || Mouse.current.middleButton.isPressed)
            return true;
        
        if (Gamepad.all.Count > 0)
        {
            foreach (var gamepad in Gamepad.all)
            {
                if (gamepad.allControls.Any(control =>
                    control is ButtonControl button &&
                    button.IsPressed() &&
                    !button.synthetic))
                {
                    return true;
                }
            }
        }

        return false;
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
