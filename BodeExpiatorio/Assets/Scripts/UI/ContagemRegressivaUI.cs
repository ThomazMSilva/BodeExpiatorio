using UnityEngine;
using UnityEngine.UI;

public class ContagemRegressivaUI : MonoBehaviour
{
    private ContagemRegressivaVidaJogador countdown;
    private float fillMultiplier;
    private Color fillColor;
    [SerializeField] private Image countdownFillIMG;
    [SerializeField] private Image countdownFrontIMG;
    [SerializeField] private Sprite countdownFront100Sprite;
    [SerializeField] private Sprite countdownFront75Sprite;
    [SerializeField] private Sprite countdownFront50Sprite;
    [SerializeField] private Sprite countdownFront25Sprite;
    //[SerializeField] private Sprite countdownFront0Sprite;

    private void Awake()
    {
        countdown = FindAnyObjectByType<ContagemRegressivaVidaJogador>();
        fillMultiplier = 1 / countdown.BaseTime;
        fillColor = countdownFillIMG.color;
    }

    private void Update()
    {
        if (!countdown.isCountDownActive) return;

        var fillAmount = countdown.CurrentTime * fillMultiplier;
        countdownFillIMG.fillAmount = fillAmount;
        var color = fillColor * fillAmount;
        color.a = 1;
        countdownFillIMG.color = color;

        countdownFrontIMG.sprite = fillAmount >= .75f ? countdownFront100Sprite :
                                   fillAmount >= .5f ? countdownFront75Sprite :
                                   fillAmount >= .25f ? countdownFront50Sprite :
                                   countdownFront25Sprite;
    }
}
