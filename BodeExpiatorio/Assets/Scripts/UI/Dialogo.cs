using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Dialogo : MonoBehaviour, IPointerClickHandler
{
    [TextArea, SerializeField] string[] falas;
    [SerializeField] TextMeshProUGUI TMPTexto;
    [SerializeField] float intervaloCaracteres;
    [SerializeField] bool temFundoPreto;
    [SerializeField] string fontName = "LiberationSans SDF";
    [SerializeField] Font font;
    int indiceAtual;
    bool isTextoTerminado;
    public UnityEvent OnDialogoAcabou;

    //eu nao entendi pq isso funciona e o OnMouseDown nao, mas n conheco mt de EventSystems, achei essa interface na internet
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        StopAllCoroutines();

        if (!isTextoTerminado)
        {
            TMPTexto.text = (temFundoPreto ? $"<font=\"{fontName}\"> <mark=#000000 padding=10,20,5,5>" : "") + falas[indiceAtual];
            isTextoTerminado = true;
        }

        else
        {
            TMPTexto.text = "";

            if (indiceAtual + 1 < falas.Length)
            {
                indiceAtual++;
                StartCoroutine(InvocaTexto(falas[indiceAtual]));
            }
            else
            {
                OnDialogoAcabou?.Invoke();
            }

        }
    }

    private void OnEnable()
    {
        indiceAtual = 0;
        StartCoroutine(InvocaTexto(falas[indiceAtual]));
    }

    public IEnumerator InvocaTexto(string textoNovo)
    {
        isTextoTerminado = false;
        TMPTexto.text = temFundoPreto ? $"<font=\"{fontName}\"> <mark=#000000 padding=10,20,5,5>" : "";
        WaitForSeconds intervalo = new(intervaloCaracteres);

        for (int i = 0; i < textoNovo.Length; i++)
        {
            TMPTexto.text += textoNovo[i];
            yield return intervalo;
        }
        isTextoTerminado = true;

        yield return null;
    }
    public void DestruaItem()
    {
        gameObject.SetActive(false);
    }

}