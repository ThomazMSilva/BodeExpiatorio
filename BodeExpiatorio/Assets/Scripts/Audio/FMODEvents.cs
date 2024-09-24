using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: SerializeField] public EventReference PlayerDamaged { get; private set; }
    [field: SerializeField] public EventReference PlayerCured { get; private set; }
    [field: SerializeField] public EventReference PlayerKnelt { get; private set; }
    [field: SerializeField] public EventReference PlayerJumped { get; private set; }
    public static FMODEvents Instance { get; private set; }
    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("Mais de um FMODEvents na cena");
        }
        Instance = this;
    }
}
