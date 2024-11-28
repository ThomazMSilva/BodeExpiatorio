using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: SerializeField, Header("Jogador")] public EventReference PlayerDamaged { get; private set; }
    [field: SerializeField] public EventReference PlayerCured { get; private set; }
    [field: SerializeField] public EventReference PlayerKnelt { get; private set; }
    [field: SerializeField] public EventReference PlayerJumped { get; private set; }
    [field: SerializeField] public EventReference PlayerWalked { get; private set; }
    [field: SerializeField] public EventReference PlayerIdle { get; private set; }

    [field: SerializeField, Header("Suplicios"), Space(8f)] public EventReference BrutalityMoved { get; private set; }
    [field: SerializeField] public EventReference LongingAttracted { get; private set; }
    [field: SerializeField] public EventReference TreacheryShot { get; private set; }
    [field: SerializeField] public EventReference ThornCollided { get; private set; }
    [field: SerializeField] public EventReference SpikeCollided { get; private set; }

    [field: SerializeField, Header("Dialogos"), Space(8f)] public EventReference[] DialogueConfessionaryLust { get; private set; }
    [field: SerializeField] public EventReference[] DialogueConfessionaryTreachery { get; private set; }
    [field: SerializeField] public EventReference[] DialogueConfessionaryBrutality { get; private set; }


    public static FMODEvents Instance { get; private set; }
    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("Mais de um FMODEvents na cena");
            return;
        }
        Instance = this;
    }
}
