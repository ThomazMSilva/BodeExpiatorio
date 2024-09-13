using UnityEngine;
using Assets.Scripts.Camera;


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
            if (currentHealth <= 0)
                OnPlayerDeath?.Invoke();
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P)) CureHealth(25f);
    }

    public delegate void HealthChangedHandler(object sender, float oldHealth, float newHealth);
    public event HealthChangedHandler OnHealthChanged;
    public event HealthChangedHandler OnMaxHealthChanged;

    public delegate void PlayerDeathHandler();
    public event PlayerDeathHandler OnPlayerDeath;
    public event PlayerDeathHandler OnPlayerTrueDeath;

    private void Start()
    {
        currentMaxHealth = baseMaxHealth;
        currentHealth = currentMaxHealth;
    }
        
    public void DamageHealth(float damageAmount)
    {
        CurrentHealth -= damageAmount;
    }
    public void CureHealth(float cureAmount) => CurrentHealth += cureAmount;

    public void FullyRecoverMaxHealth() => CurrentMaxHealth = baseMaxHealth;

    public void DamageMaxHealth(float damageAmount) => CurrentMaxHealth -= damageAmount;
    public void CureMaxHealth(float cureAmount) => CurrentMaxHealth += cureAmount;
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