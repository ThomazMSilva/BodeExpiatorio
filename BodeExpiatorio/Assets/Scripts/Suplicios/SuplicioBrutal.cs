using Assets.Scripts.Camera;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SuplicioBrutal : MonoBehaviour
{
    private bool isFalling;
    private Vector3 gravity;
    private Vector3 risingForce = Vector3.zero;
    private Rigidbody rb;
    [SerializeField] private float fallingVelocity = 5f;
    [SerializeField] private float risingVelocity = 2f;
    [SerializeField] private float maxSqrMag = 400f;
    [SerializeField] private float maxShakeIntensity = 10f;
    [SerializeField] private float maxShakeTime = .4f;
    private Transform playerTransform;
    [SerializeField] private int groundLayer = 3;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gravity = Physics.gravity;
        playerTransform = FindAnyObjectByType<Jogador>().transform;
    }

    void FixedUpdate()
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == groundLayer)
        {
            if(isFalling)
                ShakeOnCollision();
            
            isFalling = !isFalling;
        }
    }

    private void ShakeOnCollision()
    {
        float sqrMag = 1f;
        if(playerTransform != null)
        {
            sqrMag = (playerTransform.position - transform.position).sqrMagnitude;
        }
        //if(sqrMag < maxSqrMag)Debug.Log($"{gameObject.name}'s distance: {sqrMag}");
        float proximityMultiplier = Mathf.Clamp01(1 - (sqrMag / maxSqrMag));
        CinemachineShake.instance.ShakeCamera(maxShakeIntensity * proximityMultiplier, maxShakeTime * proximityMultiplier);
    }
}
