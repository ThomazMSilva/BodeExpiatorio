using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class QuebraPlataforma : MonoBehaviour
{
    //public GameObject plataforma; 
    public Transform sensor; 
    public float tempoParaQuebrar = 1f; 
    public float tempoParaReaparecer = 3f;

    private float contadorQuebra; 
    private float contadorReaparecer; 
    private bool jogadorNaArea = false;

    [SerializeField] private GameObject frontPlatform;
    [SerializeField] private GameObject backPlatform;


    private void FixedUpdate()
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
            contadorQuebra -= Time.fixedDeltaTime;

            if (contadorQuebra <= 0f)
            {
                //UnityEngine.UI.Image cu; cu.DOColor
                frontPlatform.transform.DORotate(Quaternion.EulerAngles(new(90, 0, 0)), .5f, RotateMode.Fast);
                contadorReaparecer = tempoParaReaparecer; 
                jogadorNaArea = false; 
            }
        }

        
        if (!plataforma.activeSelf)
        {
            contadorReaparecer -= Time.fixedDeltaTime;

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
