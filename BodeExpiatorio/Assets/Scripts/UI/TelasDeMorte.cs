using UnityEngine;

namespace Assets.Scripts.UI
{
    public class TelasDeMorte : MonoBehaviour
    {
        [SerializeField] private Sprite idoloDesejoSprite;
        [SerializeField] private Sprite idoloTraicaoSprite;
        [SerializeField] private Sprite idoloBrutalidadeSprite;

        [SerializeField] private UnityEngine.UI.Image fundoIMG;

        private void Start()
        {
            fundoIMG.sprite = GameManager.Instance.currentFirstRoomFromAct.sceneIndex switch
            {
                2 => idoloTraicaoSprite,
                4 => idoloBrutalidadeSprite,
                _ => idoloDesejoSprite,
            };
        }
    }
}