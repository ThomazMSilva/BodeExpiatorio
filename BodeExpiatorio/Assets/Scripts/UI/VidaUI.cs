using UnityEngine;
using UnityEngine.UI;

public class VidaUI : MonoBehaviour
{
    [SerializeField]
    Image
        MaxHealthBar,
        CurrentHealthBar,
        FrontHealthBar;

    private VidaJogador vida;
    private float fillMultiplier;

    private void Awake()
    {
        vida = FindAnyObjectByType<VidaJogador>();
        fillMultiplier = 1 / vida.BaseHealth;
    }

    private void OnEnable()
    {
        vida.OnHealthChanged += ChangeHealthUI;
        vida.OnMaxHealthChanged += ChangeMaxHealthUI;
        vida.OnFrontHealthChanged += ChangeFrontHealth;
    }

    private void ChangeFrontHealth(object sender, float oldHealth, float newHealth)
    {
        FrontHealthBar.fillAmount = newHealth * fillMultiplier;
    }

    private void ChangeMaxHealthUI(object sender, float oldHealth, float newHealth)
    {
        MaxHealthBar.fillAmount = newHealth * fillMultiplier;
    }

    private void ChangeHealthUI(object sender, float oldHealth, float newHealth)
    {
        CurrentHealthBar.fillAmount = newHealth * fillMultiplier;
    }
}
