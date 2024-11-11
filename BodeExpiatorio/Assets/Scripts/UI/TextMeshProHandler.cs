using UnityEngine;
using UnityEngine.EventSystems;

public class TextMeshProHandler : MonoBehaviour, IPointerClickHandler
{
    public delegate void ClickAction();
    public static event ClickAction OnTextClickedEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnTextClickedEvent?.Invoke();
        }
    }
}
