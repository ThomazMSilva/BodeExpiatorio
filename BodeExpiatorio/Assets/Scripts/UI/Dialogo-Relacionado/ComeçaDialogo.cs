using UnityEngine;
using System.Collections.Generic;

public class ComecaDialogo : MonoBehaviour
{
    [System.Serializable]
    public class ParObjetos
    {
        public GameObject objetoParaVerificar;
        public GameObject objetoParaAtivar;
    }

    public List<ParObjetos> paresDeObjetos; 

    void Update()
    {
        foreach (ParObjetos par in paresDeObjetos)
        {
            
            if (par.objetoParaVerificar != null && !par.objetoParaVerificar.activeInHierarchy)
            {
                
                if (par.objetoParaAtivar != null && !par.objetoParaAtivar.activeInHierarchy)
                {
                    par.objetoParaAtivar.SetActive(true);
                }
            }
        }
    }
}
