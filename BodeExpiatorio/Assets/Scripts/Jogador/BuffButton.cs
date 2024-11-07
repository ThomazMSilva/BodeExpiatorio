using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BuffButton : MonoBehaviour
{
    public BuffButtonValues buffButton;
}

[System.Serializable]
public class BuffButtonValues
{
    [SerializeField] private Image buffImage;
    public BuffType buffType;
    public Sprite buffIcon;
    [TextArea] public string buffDescription;

    public void SetValues(BuffButtonValues values)
    {
        buffType = values.buffType;
        buffIcon = values.buffIcon;
        buffDescription = values.buffDescription;

        //Debug.Log($"type: {buffType}; icon:{buffIcon}; desc: {buffDescription}");
        buffImage.sprite = buffIcon;
    }
    
}
