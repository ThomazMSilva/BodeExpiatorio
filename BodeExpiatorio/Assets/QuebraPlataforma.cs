using UnityEngine;

public class QuebraPlataforma : MonoBehaviour
{
    public GameObject plataforma; 
    public Transform sensor; 
    public float tempoParaQuebrar = 1f; 
    public float tempoParaReaparecer = 3f;

    private float contadorQuebra; 
    private float contadorReaparecer; 
    private bool jogadorNaArea = false; 

    private void Update()
    {
        
        Collider[] colliders = Physics.OverlapBox(sensor.position, sensor.localScale / 2, sensor.rotation);

        
        bool jogadorDetectado = false;
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                jogadorDetectado = true;
                break; 
            }
        }

        
        if (jogadorDetectado)
        {
            if (!jogadorNaArea) 
            {
                jogadorNaArea = true;
                contadorQuebra = tempoParaQuebrar; 
            }
        }

        
        if (jogadorNaArea)
        {
            contadorQuebra -= Time.deltaTime;

            if (contadorQuebra <= 0f)
            {
               
                plataforma.SetActive(false);
                contadorReaparecer = tempoParaReaparecer; 
                jogadorNaArea = false; 
            }
        }

        
        if (!plataforma.activeSelf)
        {
            contadorReaparecer -= Time.deltaTime;

            if (contadorReaparecer <= 0f)
            {
                
                plataforma.SetActive(true);
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
