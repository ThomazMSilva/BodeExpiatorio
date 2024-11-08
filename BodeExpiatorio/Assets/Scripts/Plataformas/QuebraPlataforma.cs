using UnityEngine;
using DG.Tweening;

public class QuebraPlataforma : MonoBehaviour
{
    public Transform sensor; 
    public float tempoParaQuebrar = 1f; 
    public float tempoParaReaparecer = 3f;

    [SerializeField] LayerMask playerLayerMask;
    private float contadorQuebra; 
    private float contadorReaparecer; 
    private bool abrindoAlcapao = false;
    private bool jogadorNaOverlapBox;

    [SerializeField] Collider platformCollider; 
    Collider[] colliders = new Collider[4]; 
    [SerializeField] private Rigidbody frontPlatformRB;
    [SerializeField] private Rigidbody backPlatformRB;
    [SerializeField] private float openingAnimationDurationA = .3f;
    [SerializeField] private float openingAnimationDurationB = .15f;
    [SerializeField] private float closingAnimationDurationA = .5f;
    [SerializeField] private float closingAnimationDurationB = .1f;

    private void FixedUpdate()
    {
        jogadorNaOverlapBox = Physics.OverlapBoxNonAlloc(sensor.position, sensor.localScale / 2, colliders, sensor.rotation, playerLayerMask) > 0;

        contadorReaparecer = jogadorNaOverlapBox ? tempoParaReaparecer : contadorReaparecer - Time.fixedDeltaTime;

        if (jogadorNaOverlapBox)
        {
            if (!abrindoAlcapao && platformCollider.enabled) 
            {
                abrindoAlcapao = true;
                Debug.Log("Comecu a abrir: " + Time.time);
                contadorQuebra = tempoParaQuebrar; 
            }
        }

        
        if (abrindoAlcapao && platformCollider.enabled)
        {
            contadorQuebra -= Time.fixedDeltaTime;

            if (contadorQuebra <= 0f)
            {
                platformCollider.enabled = false;
             
                frontPlatformRB.DORotate(new Vector3(95, 0, 0), openingAnimationDurationA, RotateMode.Fast)
                    .OnComplete(() => frontPlatformRB.DORotate(new(90, 0, 0), openingAnimationDurationB, RotateMode.Fast));

                backPlatformRB.DORotate(new Vector3(-95, 0, 0), closingAnimationDurationA, RotateMode.Fast)
                    .OnComplete(() => backPlatformRB.DORotate(new(-90, 0, 0), closingAnimationDurationB, RotateMode.Fast));

                contadorReaparecer = tempoParaReaparecer;
                Debug.Log("Tempo que parou de abrir: "+Time.time);
                abrindoAlcapao = false; 
            }
        }


        if (!jogadorNaOverlapBox && !platformCollider.enabled)
        {
            if (contadorReaparecer <= 0f)
            {
                platformCollider.enabled = true;

                frontPlatformRB.DORotate(new Vector3(-5, 0, 0), .5f, RotateMode.Fast)
                    .OnComplete(() => frontPlatformRB.DORotate(new(0, 0, 0), .1f, RotateMode.Fast));

                backPlatformRB.DORotate(new Vector3(5, 0, 0), .5f, RotateMode.Fast)
                    .OnComplete(() => backPlatformRB.DORotate(new(0, 0, 0), .1f, RotateMode.Fast));
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (sensor != null)
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(sensor.position, sensor.rotation, sensor.localScale);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one); 
        }
    }
}
