using System.Collections;
using UnityEngine;

public class Thwomp : MonoBehaviour
{
    public float fallSpeed = 20f;
    public float riseSpeed = 2f;
    public float waitTimeBeforeFall = 2f;
    public float initialWaitTime = 3f;
    public Vector3 detectionSize = new Vector3(1f, 0.1f, 1f); 
    public Vector3 expandedDetectionSize = new Vector3(2f, 0.1f, 2f); 
    public Transform bottomSensor;   
    public Transform topSensor;     
    public Transform ceilingSensor; 

    private bool isFalling = false;
    private bool isRising = false;

    private void Start()
    {
        StartCoroutine(StartFallAfterDelay());
    }

    private void Update()
    {
        if (isFalling)
        {
            Fall();
        }
        else if (isRising)
        {
            Rise();
        }
    }

    private IEnumerator StartFallAfterDelay()
    {
        yield return new WaitForSeconds(initialWaitTime);
        isFalling = true;
    }

    private void Fall()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

       
        Collider[] bottomColliders = Physics.OverlapBox(bottomSensor.position, expandedDetectionSize / 2, Quaternion.identity);
        bool hitGround = false;
        bool hitPlayer = false;

        foreach (var collider in bottomColliders)
        {
            if (collider.CompareTag("Ground"))
            {
                hitGround = true;
            }
            else if (collider.CompareTag("Player"))
            {
                hitPlayer = true;
            }
        }

        if (hitGround)
        {
            if (hitPlayer)
            {
                DamagePlayer(bottomColliders);
            }

            StartCoroutine(RiseAfterDelay());
        }
    }

    private void Rise()
    {
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

       
        float distanceToCeiling = Vector3.Distance(transform.position, ceilingSensor.position);

        if (distanceToCeiling <= 0.1f)
        {
          
            Collider[] topColliders = Physics.OverlapBox(topSensor.position, expandedDetectionSize / 2, Quaternion.identity);
            bool hitPlayer = false;

            foreach (var collider in topColliders)
            {
                if (collider.CompareTag("Player"))
                {
                    hitPlayer = true;
                }
            }

            if (hitPlayer)
            {
                DamagePlayer(topColliders);
            }

            StartCoroutine(WaitBeforeFall());
        }
    }

    private void DamagePlayer(Collider[] colliders)
    {
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                VidaJogador vidaJogador = collider.GetComponent<VidaJogador>();
                if (vidaJogador != null)
                {
                    vidaJogador.DamageHealth(vidaJogador.CurrentHealth);  
                }
            }
        }

    }

    private IEnumerator RiseAfterDelay()
    {
        isFalling = false;
        yield return new WaitForSeconds(0.5f);
        isRising = true;
    }

    private IEnumerator WaitBeforeFall()
    {
        isRising = false;
        yield return new WaitForSeconds(waitTimeBeforeFall);
        isFalling = true;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.CompareTag("Player"))
        {
            
            Rigidbody playerRb = collision.collider.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);
            }
        }
        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.collider.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
               
                playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);
            }

           
            collision.collider.transform.SetParent(transform);
        }
        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.collider.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
               
                playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);
            }

           
            collision.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
           
            collision.collider.transform.SetParent(null);
        }
    }


   
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bottomSensor.position, expandedDetectionSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(topSensor.position, expandedDetectionSize);   
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(ceilingSensor.position, detectionSize);       
    }
}
