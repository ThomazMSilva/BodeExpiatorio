using UnityEngine;
using System.Collections;
using UnityEditor.AddressableAssets.HostingServices;
using System.Linq;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class VidaJogador : MonoBehaviour
{
    [SerializeField]
    private float baseMaxHealth = 100f;
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

    private float startingMaxHealth;

    [Header("Buffs"), Space(8f)]

    [HideInInspector] public bool isIgnoreFirstDamageActive = false;

    [Space(8f)]

    [HideInInspector] public bool isIgnoreLethalDamageActive = false;
    [SerializeField] private bool lethalCanBecomeInvulnerable = true;
    [SerializeField] private float lethalInvulnerabilityTime = .5f;
    private bool isInvulnerable = false;

    [Space(8f)]

    [HideInInspector] public bool isReducedDamageActive = false;
    [SerializeField, Range(0, 1)] private float reducedDamageMultiplier = .5f;

    [Space(8f)]

    [HideInInspector] public bool isCreepingDamageActive = false;
    [Tooltip("Para de diminuir a Vida Real (cinza) quando chega na Fachada (vermelha). Se habilitado, curar pode parar o dano contínuo.")]
    [SerializeField] bool stopCreepingDamageAtFront = false;
    [SerializeField] private float creepingDamagePerSecond = 1f;
    [SerializeField, Range(1f,1.5f)] private float creepingDamageMultiplier = 1.1f;
    [SerializeField] private float currentDamageToCreep;
    private Coroutine creeping;

    public delegate void HealthChangedHandler(object sender, float oldHealth, float newHealth);

    public delegate void PlayerDeathHandler();

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P)) CureHealth(baseMaxHealth);
        if (Input.GetKeyUp(KeyCode.O)) FullyRecoverMaxHealth();
    }

    private string startingLife;
    public string DamageString 
    {
        get => 
            $"\n\nTotal damage taken to the health: {totalDamageTaken}" +
            $"\nTotal damage to the fervor: {totalFervorTaken}" +
            $"\nDamage taken in run: {damageTakenInRun}" +
            $"\nFervor taken in run: {fervorTakenInRun}" +
            $"\n{startingLife}" +
            $"\nCurrent life: {currentHealth}" +
            $"\nCurrent fervor: {currentMaxHealth}" +
            "\n\nCauses of damage:\n" + DamageCauses();
    }

    private float totalDamageTaken;
    public float DamageTakenInRoom { get => totalDamageTaken; }
    
    private float totalFervorTaken;
    public float FervorTakenInRoom { get => totalFervorTaken; }
    
    private float damageTakenInRun;
    public float DamageTakenInRun{ get => damageTakenInRun; }
    
    private float fervorTakenInRun;
    public float FervorTakenInRun{ get => fervorTakenInRun; }

    private void Start()
    {
        CurrentMaxHealth = GameManager.Instance.GetMaxHealth();
        CurrentHealth = GameManager.Instance.GetCurrentHealth();
        CurrentFrontHealth = currentHealth;
        startingMaxHealth = CurrentMaxHealth;
        OnPlayerSaved += () => Debug.Log("Se safou de dano, por um fio!");
        startingLife += $"starting life: {currentHealth}\nstarting fervor: {currentMaxHealth}";
    }

    public void ResetDamageTakenInRun()
    {
        damageTakenInRun = 0f;
        fervorTakenInRun = 0f;
    }

    private System.Collections.Generic.Dictionary<object, float> damageSenders = new();

    private void AddDamageToString(object sender, float damage)
    {
        if (!damageSenders.ContainsKey(sender))
        {
            damageSenders.Add(sender, 0);
        }
        damageSenders[sender] += damage;
    }

    public string DamageCauses()
    {
        string str = "";
        for (int i = 0; i < damageSenders.Count; i++)
        {
            str += $"\n{damageSenders.ElementAt(i).Key}: {damageSenders.ElementAt(i).Value} de saúde.";
        }
        return str;
    }

    public void DamageHealth(object sender, bool trueDamage = true)
    {
        if (!trueDamage)
        {
            if (isInvulnerable) return;

            if (isIgnoreFirstDamageActive)
            {
                OnPlayerSaved?.Invoke();
                return;
            }

            if (isIgnoreLethalDamageActive)
            {
                OnPlayerSaved?.Invoke();
                AddDamageToString(sender, CurrentHealth - 1);
                CurrentHealth = 1;
                if (lethalCanBecomeInvulnerable)
                    StartCoroutine(InvulnerabilityTime());
                return;
            }

        }
        AddDamageToString(sender, CurrentHealth);
        CurrentHealth = 0;
    }

    public void DamageHealth(float damageAmount, object sender)
    {
        if (isInvulnerable) return;

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
            
            if(lethalCanBecomeInvulnerable)
                StartCoroutine(InvulnerabilityTime());
        }
        if (isReducedDamageActive) damageAmount *= reducedDamageMultiplier;

        damageAmount = Mathf.Clamp(damageAmount, 0f, currentHealth);

        if (damageAmount >= 1) AudioManager.Instance.PlayerOneShot(FMODEvents.Instance.PlayerDamaged, transform.position);
        
        if (!isCreepingDamageActive || CurrentFrontHealth <= 0)
        {
            CurrentHealth -= damageAmount;
            totalDamageTaken += damageAmount;
            damageTakenInRun += damageAmount;

            AddDamageToString(sender, damageAmount);
            
            return;
        }

        CurrentFrontHealth -= damageAmount;

        AddDamageToString(sender, damageAmount);

        currentDamageToCreep += damageAmount * creepingDamageMultiplier;

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

    private IEnumerator InvulnerabilityTime()
    {
        isInvulnerable = true;
        OnPlayerInvulnerabilityStart?.Invoke();

        yield return new WaitForSeconds(lethalInvulnerabilityTime);
        
        isInvulnerable = false;
        OnPlayerInvulnerabilityEnd?.Invoke();
    }

    public void StopCreepingDamage() => currentDamageToCreep = 0;

    public void CureHealth(float cureAmount)
    {
        if (!isCreepingDamageActive) CurrentHealth += cureAmount;
        else CurrentFrontHealth += cureAmount;
        AudioManager.Instance.PlayerOneShot(FMODEvents.Instance.PlayerCured, transform.position);
    }

    public void FullyRecoverMaxHealth() => CurrentMaxHealth = baseMaxHealth;

    public void DamageMaxHealth(float damageAmount) 
    { 
        CurrentMaxHealth -= damageAmount; 
        totalFervorTaken += damageAmount;
        fervorTakenInRun += damageAmount;

        AddDamageToString("Fervor", damageAmount);
    }
    
    public void CureMaxHealth(float cureAmount) => CurrentMaxHealth += cureAmount;

    public void CureMaxHealth() => CurrentMaxHealth = startingMaxHealth;

    public event HealthChangedHandler OnHealthChanged;
    public event HealthChangedHandler OnMaxHealthChanged;
    public event HealthChangedHandler OnFrontHealthChanged;

    public event PlayerDeathHandler OnPlayerDeath;
    public event PlayerDeathHandler OnPlayerTrueDeath;
    public event PlayerDeathHandler OnPlayerSaved;
    public event PlayerDeathHandler OnPlayerInvulnerabilityStart;
    public event PlayerDeathHandler OnPlayerInvulnerabilityEnd;
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
            vidaJogador.DamageHealth(5f, this.name);
        } 

        if (GUILayout.Button("Full Heal"))
        {
            vidaJogador.CureHealth(25f);
        }

        customDamage = EditorGUILayout.FloatField(customDamage);
        
        if (GUILayout.Button("DamagePersonalized"))
        {
            vidaJogador.DamageHealth(customDamage, this.name);
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