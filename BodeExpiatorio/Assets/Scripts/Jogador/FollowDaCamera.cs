using UnityEngine;

public class FollowDaCamera : MonoBehaviour
{
    private Vector3 originalPosition;
    private Vector3 kneltPositon;
    [SerializeField, Range(-10, 1)] private float kneltY = -5f;

    private void Start()
    {
        originalPosition = transform.localPosition;
        kneltPositon = originalPosition;
        kneltPositon.y += kneltY;

        Entrada.Instance.OnKneelButtonDown += Input_OnKneelButtonDown;
        Entrada.Instance.OnKneelButtonUp += Input_OnKneelButtonUp;
    }

    private void OnDestroy()
    {
        Entrada.Instance.OnKneelButtonDown -= Input_OnKneelButtonDown;
        Entrada.Instance.OnKneelButtonUp -= Input_OnKneelButtonUp;
    }

    private void Input_OnKneelButtonDown() => transform.localPosition = kneltPositon;

    private void Input_OnKneelButtonUp() => transform.localPosition = originalPosition;
}
