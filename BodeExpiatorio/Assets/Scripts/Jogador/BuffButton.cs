using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BuffButton : MonoBehaviour
{
    [SerializeField] private Image buffImage;
    public BuffType buffType;
    public Sprite buffIcon;
    [TextArea] public string buffDescription;

    public void SetValues(BuffButton values)
    {
        buffType = values.buffType;
        buffIcon = values.buffIcon;
        buffDescription = values.buffDescription;

        buffImage.sprite = buffIcon;
    }
}
