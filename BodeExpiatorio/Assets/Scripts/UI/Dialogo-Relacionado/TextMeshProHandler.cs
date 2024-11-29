using UnityEngine;
using UnityEngine.EventSystems;

public class TextMeshProHandler : MonoBehaviour, IPointerClickHandler, ISubmitHandler, ICancelHandler
{
    public delegate void ClickAction();
    public static event ClickAction OnTextClickedEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("TMP Pointer CÇlick");
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnTextClickedEvent?.Invoke();
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        OnTextClickedEvent?.Invoke();
    }

    public void OnCancel(BaseEventData eventData)
    {
        OnTextClickedEvent?.Invoke();
    }
}
