using UnityEngine;

[RequireComponent(typeof(VidaJogador))]
public class ContagemRegressivaVidaJogador : MonoBehaviour
{
    [SerializeField]
    public float
        damageAmount = 10f,
        maxTime = 50f,
        countDownMultiplier = 5f,
        recoveryMultiplier = 10f,
        currentTime,
        countDownPauseTime = 10f,
        currentPauseTime = 10f;
    public float BaseTime { get { return maxTime; } private set { maxTime = value; } }
    public float CurrentTime { get { return currentTime; } private set { currentTime = value; } }

    public bool isCountDownActive = true;

    [HideInInspector]
    public bool isDoubleFervorActive = false;
    [HideInInspector]
    public bool isCountDownPauseActive = false;

    private VidaJogador vida;
    
    private void OnEnable() => vida.OnFrontHealthChanged += RecoverTimeOnCD;
    
    private void OnDisable() => vida.OnFrontHealthChanged -= RecoverTimeOnCD;

    private void Awake() => vida = GetComponent<VidaJogador>();

    private void Start() => currentTime = maxTime;

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.K)) isCountDownActive = !isCountDownActive;
        if (CanCountDown()) CountDown(); 
    }

    private void CountDown()
    {
        currentTime -= Time.deltaTime * countDownMultiplier;

        if (currentTime <= 0)
        {
            vida.DamageMaxHealth(damageAmount);
            currentTime = maxTime;
        }
    }

    private bool CanCountDown()
    {
        if (!isCountDownActive || vida.CurrentMaxHealth <= 0) return false;

        if (isCountDownPauseActive)
        {
            currentPauseTime -= Time.deltaTime;

            if (currentPauseTime > 0) return false;

            currentPauseTime = countDownPauseTime;
            isCountDownPauseActive = false;
        }
        return true;
    }

    private void RecoverTimeOnCD(object sender, float oldHealth, float newHealth)
    {
        //recupera o timer no tanto que a vida foi mudada, tanto no dano quanto na cura
        float recoveryAmount = Mathf.Abs(newHealth - oldHealth) * recoveryMultiplier;
        currentTime += recoveryAmount * (isDoubleFervorActive ? 2f : 1f);
        currentTime = Mathf.Clamp(currentTime, 0, maxTime);
        //OnCountUp?.Invoke(currentTime);
    }

}   