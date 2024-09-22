using System.Collections;
using UnityEngine;

public class Thwomp : MonoBehaviour
{
    public float fallSpeed = 20f; 
    public float riseSpeed = 2f;
    public float waitTimeBeforeFall = 2f; 
    private bool isFalling = true;
    private bool isRising = false;
    public Transform bottomSensor; 
    public Transform topSensor; 

    private void Start()
    {

        isFalling = true;
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


    private void Fall()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;


        RaycastHit hit;
        if (Physics.Raycast(bottomSensor.position, Vector3.down, out hit, 0.1f))
        {
            if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Player"))
            {
                if (hit.collider.CompareTag("Player"))
                {
     
                    VidaJogador vidaJogador = hit.collider.GetComponent<VidaJogador>();
                    if (vidaJogador != null)
                    {
                        vidaJogador.DamageHealth(vidaJogador.CurrentHealth);
                    }
                }

        
                StartCoroutine(RiseAfterDelay());
            }
        }
    }


    private void Rise()
    {
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        
        RaycastHit hit;
        if (Physics.Raycast(topSensor.position, Vector3.up, out hit, 0.1f))
        {
            if (hit.collider.CompareTag("Ground"))
            {
              
                StartCoroutine(WaitBeforeFall());
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
}
