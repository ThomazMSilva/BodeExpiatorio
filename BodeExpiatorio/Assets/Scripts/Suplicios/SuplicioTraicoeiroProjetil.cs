﻿using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.Suplicios
{
    public class SuplicioTraicoeiroProjetil : MonoBehaviour
    {
        private Jogador player;
        [SerializeField] private int groundLayer = 3;
        [SerializeField] private float stunTime = 1f;
        [SerializeField] private float directDamage = 4f;
        [SerializeField] private float explosionRadius = 10f;
        [SerializeField] private float explosionDamage = 6f;
        [SerializeField] private float explosionForce = 10f;
        [SerializeField] private float yExplosionSupport = 2f;

        [SerializeField] private Material explosionDisplayColor;
        [SerializeField] private float explosionDisplayDuration;

        private void Start() => player = FindAnyObjectByType<Jogador>();

        private void OnCollisionEnter(Collision collision)
        {
            GameObject go = collision.gameObject;
            if (go.CompareTag("Player"))
            {
                player.ApplyDamageEffect(directDamage);
                gameObject.SetActive(false);
            }

            if(go.layer == groundLayer)
            {
                DisplayExplosion(); 

                Collider[] col = Physics.OverlapSphere(transform.position, explosionRadius);
                
                foreach(var collider in col)
                {
                    if (!collider.CompareTag("Player")) continue;
                    
                    Vector3 direction = collider.transform.position - transform.position;
                    direction.y += yExplosionSupport;
                    player.ApplyDamageEffect(explosionDamage, direction.normalized * explosionForce, stunTime);
                    break;
                }
                gameObject.SetActive(false);
            }
        }

        private void DisplayExplosion()
        {
            GameObject displaySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            displaySphere.SetActive(false);
            Destroy(displaySphere.GetComponent<SphereCollider>());
            displaySphere.transform.position = transform.position;
            displaySphere.transform.localScale = explosionRadius * 2 * Vector3.one;
            displaySphere.SetActive(true);
            displaySphere.GetComponent<Renderer>().material = explosionDisplayColor;
            displaySphere.GetComponent<Renderer>().material.DOFade(0, explosionDisplayDuration);
            Destroy(displaySphere, explosionDisplayDuration);
        }
    }
}