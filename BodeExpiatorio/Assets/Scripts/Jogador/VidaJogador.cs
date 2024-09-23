using UnityEngine;
using System.Collections;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class VidaJogador : MonoBehaviour
{
    [SerializeField]
    private float baseMaxHealth = 10f;
    public float BaseHealth { get { return baseMaxHealth; } private set { baseMaxHealth = value; } } 

    [SerializeField]
    private float currentMaxHealth;
    public float CurrentMaxHealth
    {
        get => currentMaxHealth;
        private set
        {
            float oldValue = currentMaxHealth;
            currentMaxHealth = Mathf.Clamp(value, 0, baseMaxHealth);
            OnMaxHealthChanged?.Invoke(this, oldValue, currentMaxHealth);
            if (currentMaxHealth <= 0)
                OnPlayerTrueDeath?.Invoke();
            if (currentHealth > currentMaxHealth)
                CurrentHealth = currentMaxHealth;
        }
    }

    [SerializeField]
    private float currentHealth;
    public float CurrentHealth
    {
        get => currentHealth;
        private set
        {
            float oldValue = currentHealth;
            currentHealth = Mathf.Clamp(value, 0, currentMaxHealth);
            OnHealthChanged?.Invoke(this, oldValue, currentHealth);

            if (currentHealth <= 0) OnPlayerDeath?.Invoke();

            if (isCreepingDamageActive && currentHealth > CurrentFrontHealth) return;

            if (stopCreepingDamageAtFront) StopCreepingDamage();

            CurrentFrontHealth = currentHealth;
        }
    }
    private float currentFrontHealth;
    public float CurrentFrontHealth
    {
        get => currentFrontHealth;
        private set
        {
            float oldHealth = currentFrontHealth;
            currentFrontHealth = Mathf.Clamp(value, 0, currentMaxHealth);
            OnFrontHealthChanged?.Invoke(this, oldHealth, currentFrontHealth);

            if (currentFrontHealth > currentHealth) CurrentHealth = currentFrontHealth;
        }
    }
    [Header("Buffs"), Space(8f)]

    [HideInInspector] public bool isIgnoreFirstDamageActive = false;
    [HideInInspector] public bool isIgnoreLethalDamageActive = false;

    [Space(8f)]

    [HideInInspector] public bool isReducedDamageActive = false;
    [SerializeField, Range(0, 1)] private float reducedDamageMultiplier = .5f;

    [Space(8f)]

    [HideInInspector] public bool isCreepingDamageActive = false;
    [Tooltip("Para de diminuir a Vida Real (cinza) quando chega na Fachada (vermelha). Se habilitado, curar pode parar o dano contínuo.")]
    [SerializeField] bool stopCreepingDamageAtFront = false;
    [SerializeField] private float creepingDamagePerSecond = 1f;
    [SerializeField] private float currentDamageToCreep;
    private Coroutine creeping;

    public delegate void HealthChangedHandler(object sender, float oldHealth, float newHealth);

    public delegate void PlayerDeathHandler();

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P)) CureHealth(25f);
    }
   
    private void Start()
    {
        currentMaxHealth = baseMaxHealth;
        currentHealth = currentMaxHealth;
        currentFrontHealth = currentHealth;

        OnPlayerSaved += () => Debug.Log("Se safou de dano, por um fio!");
    }

    public void DamageHealth(float damageAmount)
    {
        if (isIgnoreFirstDamageActive)
        {
            isIgnoreFirstDamageActive = false;
            OnPlayerSaved?.Invoke();
            return;
        }
        
        if (isIgnoreLethalDamageActive && damageAmount >= currentHealth)
        {
            damageAmount = Mathf.Clamp(damageAmount, 0f, currentHealth - 1f);
            isIgnoreLethalDamageActive = false;
            OnPlayerSaved?.Invoke();
        }

        if (isReducedDamageActive) damageAmount *= reducedDamageMultiplier;
        
        if (!isCreepingDamageActive || CurrentFrontHealth <= 0)
        {
            CurrentHealth -= damageAmount;
            return;
        }

        CurrentFrontHealth -= damageAmount;
        currentDamageToCreep += damageAmount;

        creeping ??= StartCoroutine(CreepDamage());
    }

    private IEnumerator CreepDamage()
    {
        while (currentDamageToCreep >= 0)
        {
            CurrentHealth -= creepingDamagePerSecond * Time.deltaTime;
            currentDamageToCreep -= creepingDamagePerSecond * Time.deltaTime;
            yield return null;
        }
        creeping = null;
    }

    public void StopCreepingDamage() => currentDamageToCreep = 0;

    public void CureHealth(float cureAmount)
    {
        if (!isCreepingDamageActive) CurrentHealth += cureAmount;
        else CurrentFrontHealth += cureAmount;
    }

    public void FullyRecoverMaxHealth() => CurrentMaxHealth = baseMaxHealth;

    public void DamageMaxHealth(float damageAmount) => CurrentMaxHealth -= damageAmount;
    
    public void CureMaxHealth(float cureAmount) => CurrentMaxHealth += cureAmount;

    public event HealthChangedHandler OnHealthChanged;
    public event HealthChangedHandler OnMaxHealthChanged;
    public event HealthChangedHandler OnFrontHealthChanged;

    public event PlayerDeathHandler OnPlayerDeath;
    public event PlayerDeathHandler OnPlayerTrueDeath;
    public event PlayerDeathHandler OnPlayerSaved;
}

#if UNITY_EDITOR
[CustomEditor(typeof(VidaJogador))]
public class VidaJogadorEditor : Editor
{
    float customDamage;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VidaJogador vidaJogador = (VidaJogador)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Test Health", EditorStyles.boldLabel);

        if (GUILayout.Button("Heal Health by 5"))
        {
            vidaJogador.CureHealth(5f);
        }
        if (GUILayout.Button("Damage Health by 5"))
        {
            vidaJogador.DamageHealth(5f);
        } 

        if (GUILayout.Button("Full Heal"))
        {
            vidaJogador.CureHealth(25f);
        }

        customDamage = EditorGUILayout.FloatField(customDamage);
        
        if (GUILayout.Button("DamagePersonalized"))
        {
            vidaJogador.DamageHealth(customDamage);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Test Max Health", EditorStyles.boldLabel);

        if (GUILayout.Button("Heal Max Health by 5"))
        {
            vidaJogador.CureMaxHealth(5f);
        }

        if (GUILayout.Button("Damage Max Health by 1"))
        {
            vidaJogador.DamageMaxHealth(1f);
        }
    }
}
#endif