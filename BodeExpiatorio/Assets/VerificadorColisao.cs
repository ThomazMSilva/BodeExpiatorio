using UnityEngine;

public class VerificadorAtivacao : MonoBehaviour
{
    [SerializeField] private GameObject objeto1;
    [SerializeField] private GameObject objeto2;
    [SerializeField] private GameObject objetoParaControlar;

    private void Update()
    {
        // Se um dos dois objetos estiver ativo, mantém objetoParaControlar desativado
        if (objeto1.activeSelf || objeto2.activeSelf)
        {
            
                objetoParaControlar.SetActive(false);
            
        }
        
    }
}
