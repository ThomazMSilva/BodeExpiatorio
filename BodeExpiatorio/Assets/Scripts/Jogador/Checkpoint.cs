using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    private JogadorReference player;

    [SerializeField] private bool activateHintScreen = true;
    [SerializeField] private GameObject hintScreen;

    public Transform checkpointTransform;
    public bool isActive;

    public delegate void OnPrayedHandler(Checkpoint checkpoint);
    public event OnPrayedHandler OnPrayed;

    [Tooltip("Caso queira fazer mais alguma coisa além de ativar o checkpoint, quando interagindo com esse colisor.")]
    public UnityEvent OnInteracted;

    private void Awake() => player = FindAnyObjectByType<JogadorReference>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        if (activateHintScreen && !isActive)
            hintScreen.SetActive(true);

        other.GetComponent<ContagemRegressivaVidaJogador>().isCountDownActive = false;
        Entrada.Instance.OnKneelButtonDown += Pray;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        if (activateHintScreen)
            hintScreen.SetActive(false);

        other.GetComponent<ContagemRegressivaVidaJogador>().isCountDownActive = true;
        Entrada.Instance.OnKneelButtonDown -= Pray;
    }

    private void Pray()
    {
        isActive = true;
        OnPrayed?.Invoke(this);
        OnInteracted?.Invoke();
    }
}
