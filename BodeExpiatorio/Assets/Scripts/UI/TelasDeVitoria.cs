using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class TelasDeVitoria : MonoBehaviour
    {
        [SerializeField] private GameObject salvationVideoPlayer;
        [SerializeField] private GameObject renitenceVideoPlayer;
        [SerializeField] private float threshold = 200;
        [SerializeField] private TormentType basedOn;

        // Use this for initialization
        void Start()
        {
            if (GameManager.Instance.InPathOfRenitence(basedOn, threshold)) renitenceVideoPlayer.SetActive(true);
            else salvationVideoPlayer.SetActive(true);
        }
    }
}