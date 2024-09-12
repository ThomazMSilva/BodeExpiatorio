using UnityEngine;
using UnityEngine.UI;

public class ContagemRegressivaUI : MonoBehaviour
{
    private ContagemRegressivaVidaJogador countdown;
    private float countdownMultiplier;
    [SerializeField] private Image countdownIMG;

    void Awake()
    {
        countdown = FindAnyObjectByType<ContagemRegressivaVidaJogador>();
        countdownMultiplier = 1 / countdown.BaseTime;
    }

    void Update()
    {
        countdownIMG.fillAmount = countdown.CurrentTime * countdownMultiplier;
    }
}
