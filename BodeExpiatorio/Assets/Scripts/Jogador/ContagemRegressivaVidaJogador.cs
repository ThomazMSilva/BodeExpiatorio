using UnityEngine;

[RequireComponent(typeof(VidaJogador))]
public class ContagemRegressivaVidaJogador : MonoBehaviour
{
    [SerializeField]
    private float
        maxTime = 100,
        countDownMultiplier = 2,
        recoveryMultiplier = 10,
        currentTime;
    public float BaseTime { get { return maxTime; } private set { maxTime = value; } }
    public float CurrentTime { get { return currentTime; } private set { currentTime = value; } }

    public bool isCountDownActive = true;

    private VidaJogador vida;

    /*public delegate void CountDownHandler(float currentTime);
    public CountDownHandler OnCountUp;*/
    
    private void OnEnable() => vida.OnHealthChanged += RecoverTimeOnCD;
    
    private void OnDisable() => vida.OnHealthChanged -= RecoverTimeOnCD;

    private void Awake() => vida = GetComponent<VidaJogador>();

    private void Start() => currentTime = maxTime;

    private void Update() 
    {
        if (isCountDownActive) 
            CountDown(); 
    }

    private void CountDown()
    {
        currentTime -= Time.deltaTime * countDownMultiplier;

        if (currentTime <= 0)
        {
            vida.DamageMaxHealth(1);
            currentTime = maxTime;
        }
    }
    private void RecoverTimeOnCD(object sender, float oldHealth, float newHealth)
    {
        //recupera o timer no tanto que a vida foi mudada, tanto no dano quanto na cura
        float recoveryAmount = Mathf.Abs(newHealth - oldHealth) * recoveryMultiplier;
        currentTime += recoveryAmount;
        currentTime = Mathf.Clamp(currentTime, 0, maxTime);
        //OnCountUp?.Invoke(currentTime);
    }

}   