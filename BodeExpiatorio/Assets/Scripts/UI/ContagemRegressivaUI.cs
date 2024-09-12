using UnityEngine;
using UnityEngine.UI;

public class ContagemRegressivaUI : MonoBehaviour
{
    private ContagemRegressivaVidaJogador countdown;
    private float fillMultiplier;
    [SerializeField] private Image countdownIMG;

    private void Awake()
    {
        countdown = FindAnyObjectByType<ContagemRegressivaVidaJogador>();
        fillMultiplier = 1 / countdown.BaseTime;
    }

    private void Update() => countdownIMG.fillAmount = countdown.CurrentTime * fillMultiplier;
}
