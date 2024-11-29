using System.Collections;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform checkpointTransform;
    public bool isActive;

    private JogadorReference player;
    private void Awake() => player = FindAnyObjectByType<JogadorReference>();

    private void Pray()
    {
        isActive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        other.GetComponent<ContagemRegressivaVidaJogador>().isCountDownActive = false;
        Entrada.Instance.OnKneelButtonDown += Pray;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        other.GetComponent<ContagemRegressivaVidaJogador>().isCountDownActive = true;
        Entrada.Instance.OnKneelButtonDown -= Pray;
    }
}
