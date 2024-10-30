using UnityEngine;

public class ComeçaDialogo : MonoBehaviour
{
   
    public GameObject objetoParaVerificar;


    public GameObject objetoParaAtivar;

    void Update()
    {
        
        if (objetoParaVerificar != null && !objetoParaVerificar.activeInHierarchy)
        {
            
            if (objetoParaAtivar != null && !objetoParaAtivar.activeInHierarchy)
            {
                objetoParaAtivar.SetActive(true);
            }
        }
    }
}
