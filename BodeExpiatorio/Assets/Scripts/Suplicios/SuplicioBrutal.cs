using Assets.Scripts.Camera;
using FMOD.Studio;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SuplicioBrutal : MonoBehaviour
{
    [SerializeField]
    private bool isFalling;
    private bool isWaiting = false;
    private Vector3 gravity;
    private Vector3 risingForce = Vector3.zero;
    private Transform playerTransform;
    private Rigidbody rb;
    private WaitForSeconds waitForSeconds;
    [SerializeField] private float fallingVelocity = 5f;
    [SerializeField] private float risingVelocity = 2f;
    [SerializeField] private float waitTime = .5f;
    [SerializeField] private float maxSqrMag = 400f;
    [SerializeField] private float maxShakeIntensity = 10f;
    [SerializeField] private float maxShakeTime = .4f;
    [SerializeField] private int groundLayer = 3;

    [SerializeField] private EventInstance brutalChainsEventInstance;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gravity = Physics.gravity;
        playerTransform = FindAnyObjectByType<Jogador>().transform;
        brutalChainsEventInstance = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.BrutalityMoved);
        waitForSeconds = new(waitTime);
    }


    void FixedUpdate()
    {
        if (!isWaiting)
        {
            if (isFalling)
            {
                rb.AddForce(gravity * fallingVelocity, ForceMode.Acceleration);
            }
            else
            {
                risingForce.Set(0, -gravity.y * risingVelocity - rb.velocity.y, 0);
                rb.AddForce(risingForce, ForceMode.VelocityChange);
            }
        }

        UpdateSound();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == groundLayer)
        {
            if (isFalling) 
            {
                ShakeOnCollision();
                StartCoroutine(Wait());
            }
            
            isFalling = !isFalling;
        }
    }

    private IEnumerator Wait()
    {
        isWaiting = true;
        yield return waitForSeconds;
        isWaiting = false;
    }

    private void ShakeOnCollision()
    {
        float sqrMag = 1f;
        if(playerTransform != null)
            sqrMag = (playerTransform.position - transform.position).sqrMagnitude;
        
        float proximityMultiplier = Mathf.Clamp01(1 - (sqrMag / maxSqrMag));
        CinemachineShake.instance.ShakeCamera(maxShakeIntensity * proximityMultiplier, maxShakeTime * proximityMultiplier);
    }

    private void UpdateSound()
    {
        if (rb.velocity.y != 0 && !isWaiting)
        {
            brutalChainsEventInstance.getPlaybackState(out PLAYBACK_STATE pbState);
            if (pbState.Equals(PLAYBACK_STATE.STOPPED))
            {
                brutalChainsEventInstance.start();
            }
        }
        else if (isWaiting)
        {
            brutalChainsEventInstance.keyOff();
        }
        else
        {
            brutalChainsEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }
}
