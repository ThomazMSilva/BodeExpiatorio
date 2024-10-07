using Assets.Scripts.Camera;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThwompDetecta : MonoBehaviour
{
    private bool isFalling = false;
    private bool isReturning = false;
    private bool playerDetected = false; 
    private Vector3 initialPosition;
    private Vector3 gravity;
    private Rigidbody rb;
    [SerializeField] private float fallingVelocity = 5f;
    [SerializeField] private float risingVelocity = 2f;
    [SerializeField] private float maxShakeIntensity = 10f;
    [SerializeField] private float maxShakeTime = .4f;
    private Transform playerTransform;
    [SerializeField] private int groundLayer = 3;

   
    [SerializeField] private bool enableRaycastDetection = true; 
    [SerializeField] private float raycastDistance = 5f; 
    [SerializeField] private LayerMask playerLayer;      
    [SerializeField] private float maxSqrMag = 400f; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gravity = Physics.gravity;
        initialPosition = transform.position; 
        playerTransform = FindAnyObjectByType<Jogador>().transform;
    }

    void FixedUpdate()
    {
        
        if (enableRaycastDetection && !isReturning)
        {
            RaycastHit hit;
            
            if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, playerLayer))
            {
              
                if (!playerDetected)
                {
                    playerDetected = true; 
                    isFalling = true;      
                }
            }
            else
            {
                playerDetected = false; 
            }
        }

     
        if (isFalling)
        {
            rb.isKinematic = false; 
            rb.AddForce(gravity * fallingVelocity, ForceMode.Acceleration);
        }
        else if (isReturning)
        {
           
            rb.isKinematic = false; 
            Vector3 returnDirection = (initialPosition - transform.position).normalized;
            rb.velocity = returnDirection * risingVelocity;

            
            if (Vector3.Distance(transform.position, initialPosition) < 0.1f)
            {
                isReturning = false;
                rb.velocity = Vector3.zero; 
                rb.isKinematic = true; 
            }
        }
        else
        {
            rb.isKinematic = true; 
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == groundLayer && isFalling)
        {
           
            ShakeOnCollision();
            isFalling = false; 
            isReturning = true; 
        }
    }

    private void ShakeOnCollision()
    {
        float sqrMag = 1f;
        if (playerTransform != null)
        {
            sqrMag = (playerTransform.position - transform.position).sqrMagnitude;
        }

       
        float proximityMultiplier = Mathf.Clamp01(1 - (sqrMag / maxSqrMag));
        CinemachineShake.instance.ShakeCamera(maxShakeIntensity * proximityMultiplier, maxShakeTime * proximityMultiplier);
    }

    
    public void SetRaycastDetection(bool state)
    {
        enableRaycastDetection = state;
    }

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * raycastDistance); 
    }
}
