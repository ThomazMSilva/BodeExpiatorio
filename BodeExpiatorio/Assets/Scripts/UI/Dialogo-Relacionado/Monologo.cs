using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

public class Monologo : MonoBehaviour, IPointerClickHandler, ISubmitHandler, ICancelHandler
{
    [TextArea, SerializeField] string[] falas;
    [SerializeField] TextMeshProUGUI TMPTexto;
    [SerializeField] float typeCharacters;
    [SerializeField] float intervaloCaracteres;
    [SerializeField] bool temFundoPreto;
    //[SerializeField] string fontName = "LiberationSans SDF";
    [SerializeField] Font font;
    int indiceAtual;
    bool isTextoTerminado;
    public UnityEvent OnDialogoAcabou;
    [SerializeField] private UINavigationManager navigationManager;
    [SerializeField] private bool skipMonologueWithSubmit;
    [SerializeField] private bool disappearAutomatically;
    [SerializeField] private float timeToDisappear = 3f;
    [SerializeField] private bool typeInstantly;
    [SerializeField] private bool canFade;
    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float fadeOutTime = 1f;
    private Coroutine disappearAutomaticallyRoutine;

    public void AvancaDialogo()
    {
        StopAllCoroutines();

        if (!isTextoTerminado)
        {
            TMPTexto.text = (temFundoPreto ? $"<font=\"{font.name ?? "LiberationSans SDF"}\"> <mark=#000000 padding=10,20,5,5>" : "") + falas[indiceAtual];
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


    public void OnSubmit(BaseEventData eventData)
    { 
        if(skipMonologueWithSubmit)
          AvancaDialogo();
    }

    //eu nao entendi pq isso funciona e o OnMouseDown nao, mas n conheco mt de EventSystems, achei essa interface na internet
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (skipMonologueWithSubmit)
            AvancaDialogo();
    }

    public void OnCancel(BaseEventData eventData)
    {
        if (skipMonologueWithSubmit)
            AvancaDialogo();
    }

    private void OnEnable()
    {
        indiceAtual = 0;

        if (canFade)
        {
            TMPTexto.color = new(TMPTexto.color.r, TMPTexto.color.g, TMPTexto.color.b, 0);

            DOTween.To
            (
                () => TMPTexto.color.a,
                    alpha => TMPTexto.color = new(TMPTexto.color.r, TMPTexto.color.g, TMPTexto.color.b, alpha),
                    1,
                    fadeInTime
            );
        }

        StartCoroutine(InvocaTexto(falas[indiceAtual]));
        if (disappearAutomatically) disappearAutomaticallyRoutine = StartCoroutine(Disappear());
        //EventSystem.current.SetSelectedGameObject(gameObject);
    }

    private IEnumerator Disappear()
    {
        yield return new WaitForSeconds(timeToDisappear);
        DestruaItem();
        //gameObject.SetActive(false);
    }


    public IEnumerator InvocaTexto(string textoNovo)
    {
        isTextoTerminado = false;
        TMPTexto.text = temFundoPreto ? $"<font=\"{font.name ?? "LiberationSans SDF"}\"> <mark=#000000 padding=10,20,5,5>" : "";

        if (!typeInstantly)
        {
            WaitForSeconds intervalo = new(intervaloCaracteres);

            for (int i = 0; i < textoNovo.Length; i++)
            {
                TMPTexto.text += textoNovo[i];
                yield return intervalo;
            }
        }
        else
        {
            TMPTexto.text = textoNovo;
        }
        isTextoTerminado = true;
    }

    public void DestruaItem()
    {
        if (disappearAutomaticallyRoutine != null)
            StopCoroutine(disappearAutomaticallyRoutine);

        if (navigationManager == null)
        {
            Debug.LogError($"Não tem navigation manager referenciado no dialogo {gameObject.name}");
            return;
        }

        if (!canFade)
        {
            navigationManager.ClosePanel();
            return;
        }
        DOTween.To
            (
                () => TMPTexto.color.a,
                    alpha => TMPTexto.color = new(TMPTexto.color.r, TMPTexto.color.g, TMPTexto.color.b, alpha),
                    0,
                    fadeOutTime
            ).OnComplete(navigationManager.ClosePanel);
        //TMPTexto.material.DOFade(0, fadeOutTime).OnComplete(navigationManager.ClosePanel);
        //gameObject.SetActive(false);
    }

}