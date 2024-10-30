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

    public List<ParObjetos> paresDeObjetos; // Lista de pares de objetos para verificar e ativar

    void Update()
    {
        foreach (ParObjetos par in paresDeObjetos)
        {
            // Verifica se o objeto do par está inativo
            if (par.objetoParaVerificar != null && !par.objetoParaVerificar.activeInHierarchy)
            {
                // Ativa o objeto correspondente do par, caso esteja inativo
                if (par.objetoParaAtivar != null && !par.objetoParaAtivar.activeInHierarchy)
                {
                    par.objetoParaAtivar.SetActive(true);
                }
            }
        }
    }
}
