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
        StopCoroutine(Attract());
        windEventInstance.release();
    }

    private void Start()
    {
        windEventInstance = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.LongingAttracted);
        windEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
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
            this.name,
            ForceMode.Acceleration
        );
    }

    private IEnumerator Attract()
    {
        while (true)
        {
            if (!isIntervalActive) yield return null;
            GetComponent<Renderer>().material.DOFade(isAttracting ? 0 : 1, interval * fadeTimeRelativeToInterval);
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
