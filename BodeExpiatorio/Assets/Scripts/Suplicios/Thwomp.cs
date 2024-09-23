using System.Collections;
using UnityEngine;

public class Thwomp : MonoBehaviour
{
    public float fallSpeed = 20f;
    public float riseSpeed = 2f;
    public float waitTimeBeforeFall = 2f;
    public Vector3 detectionSize = new Vector3(1f, 0.1f, 1f);
    public float detectionRadius = 0.5f;
    public Transform bottomSensor;
    public Transform topSensor;

    public Transform ceilingSensor;
    public Transform floorSensor;

    private bool isFalling = true;
    private bool isRising = false;

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

    private void Fall()
    {

        transform.position += Vector3.down * fallSpeed * Time.deltaTime;


        Collider[] bottomColliders = Physics.OverlapBox(bottomSensor.position, detectionSize / 2, Quaternion.identity);

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


        Collider[] topColliders = Physics.OverlapBox(topSensor.position, detectionSize / 2, Quaternion.identity);

        bool hitCeiling = false;
        bool hitPlayer = false;

        foreach (var collider in topColliders)
        {
            if (collider.CompareTag("Ground"))
            {
                hitCeiling = true;
            }
            else if (collider.CompareTag("Player"))
            {
                hitPlayer = true;
            }
        }


        if (hitCeiling)
        {

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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bottomSensor.position, detectionSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(topSensor.position, detectionSize);
    }

}
