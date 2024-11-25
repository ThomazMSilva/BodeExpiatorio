using DG.Tweening;
using System.Collections;
using UnityEngine;
using FMOD.Studio;

public class SuplicioAnsioso : MonoBehaviour
{
    [SerializeField] private float interval = 2.5f;
    [SerializeField] private float damagePerSecond = 1;
    [SerializeField] private float force = 45;
    [SerializeField, Range(0, 1)] private float kneelingForceMultiplier = 0.5f;
    [SerializeField, Range(0, 1)] private float fadeTimeRelativeToInterval = 0.5f;

    [SerializeField] private bool isIntervalActive = true;
    [SerializeField] private bool isAttracting;
    public bool IsAttracting { get => isAttracting; }

    private bool _isKneeling;
    private float currentForceMultiplier;

    private EventInstance windEventInstance;
    private Jogador _player;
    public Jogador Player { get => _player; }
    private Entrada _input;

    [SerializeField] private ParticleSystem portalParticleSystem;
    [SerializeField] private ParticleSystem suctionParticleSystem;
    [SerializeField] private ParticleSystem vortexParticleSystem;
    [SerializeField] private Renderer mirrorRenderer;
    [SerializeField] private Material mirrorMatOpen;
    [SerializeField] private Material mirrorMatClosed;


    private ParticleSystem.EmissionModule portalEmission;
    private ParticleSystem.EmissionModule suctionEmission;
    private ParticleSystem.EmissionModule vortexEmission;
    private ParticleSystem.MinMaxCurve originalPortalRate;
    private ParticleSystem.MinMaxCurve originalSuctionRate;
    private ParticleSystem.MinMaxCurve originalVortexRate;

    private IEnumerator InitializeSingletonReference()
    {
        while (_input == null)
        {
            _input = Entrada.Instance;
            yield return null;
        }
        _input.OnKneelButtonDown += SetKneelingTrue;
        _input.OnKneelButtonUp += SetKneelingFalse;

        StartCoroutine(Attract());
    }

    private void OnEnable()
    {
        StartCoroutine(InitializeSingletonReference());
    }

    private void OnDisable()
    {
        _input.OnKneelButtonDown -= SetKneelingTrue;
        _input.OnKneelButtonUp -= SetKneelingFalse;
        //StopCoroutine(Attract());
        StopAllCoroutines();
        windEventInstance.release();
    }

    private void Start()
    {
        windEventInstance = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.LongingAttracted);
        windEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        //mirrorOriginalAlpha = mirrorGO.GetComponent<Renderer>().material.color.a;
        //mirrorOriginalColor = mirrorGO.GetComponent<Renderer>().material.color;

        var portalShapeModule = portalParticleSystem.shape;
        portalShapeModule.length = transform.localScale.x;
        var suctionShapeModule = suctionParticleSystem.shape;
        suctionShapeModule.length = transform.localScale.x;
        var vortexShapeModule = vortexParticleSystem.shape;
        vortexShapeModule.length = transform.localScale.x;

        portalEmission= portalParticleSystem.emission;
        suctionEmission = suctionParticleSystem.emission;
        vortexEmission = vortexParticleSystem.emission;

        originalPortalRate = portalEmission.rateOverTime;
        originalSuctionRate = suctionEmission.rateOverTime;
        originalVortexRate = vortexEmission.rateOverTime;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        other.TryGetComponent<Jogador>(out _player);
        _player.Movimento.SetInstantVelocityChange(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        _player.Movimento.SetInstantVelocityChange(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || !isAttracting || _player == null) return;

        currentForceMultiplier = _isKneeling ? force * kneelingForceMultiplier : force;
        _player.ApplyDamageEffect
        (
            damagePerSecond * Time.fixedDeltaTime, 
            -transform.right * currentForceMultiplier, 
            .01f,
            "Tentação",
            ForceMode.Acceleration
        );
    }

    Color fadedColor = Color.black;
    Color unfadedColor = Color.white;

    private IEnumerator Attract()
    {
        while (true)
        {
            if (!isIntervalActive) yield return null;

            portalEmission.rateOverTime = isAttracting ? 0 : originalPortalRate;
            suctionEmission.rateOverTime = isAttracting ? 0 : originalSuctionRate;
            vortexEmission.rateOverTime = isAttracting ? 0 : originalVortexRate;
            vortexEmission.burstCount = isAttracting ? 0 : 5;

            mirrorRenderer.material = isAttracting ? mirrorMatClosed : mirrorMatOpen;
            //mirrorGO.GetComponent<Renderer>().material.DOFade(isAttracting ? 0 : mirrorOriginalAlpha, interval * fadeTimeRelativeToInterval);
            isAttracting = !isAttracting;

            if (isAttracting) windEventInstance.start();

            else windEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
            
            OnChangedAttractingState?.Invoke();
            yield return new WaitForSeconds(interval);
        }
    }

    private void SetKneelingTrue() => _isKneeling = true;
    private void SetKneelingFalse() => _isKneeling = false;

    public delegate void EventHandler();
    public event EventHandler OnChangedAttractingState;
}
