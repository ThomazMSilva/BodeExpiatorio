using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SuplicioBrutalCorrente : MonoBehaviour
{
    [SerializeField] private GameObject chainPrefab;
    private Dictionary<Vector3, GameObject> chainsInPlace;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float rayDistance;
    [SerializeField] private float chainSize = .05f;
    private RaycastHit hit;
    private List<GameObject> chains = new();

    void Start()
    {
        chainsInPlace = new();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Physics.Raycast(transform.position, transform.up, out hit, rayDistance, layerMask);
        Vector3 endPoint = hit.transform ? hit.point : transform.position + new Vector3(0,10,0);

        int chainLinkAmount = Mathf.RoundToInt((endPoint.y - transform.position.y) / chainSize);

        for(int i = 0; i < chainLinkAmount; i ++)
        {
            Vector3 chainPosition = new(endPoint.x,  endPoint.y - (chainSize * i), endPoint.z);

            if (!chainsInPlace.ContainsKey(chainPosition))
                chainsInPlace.Add(chainPosition, PoolManager.Instance.InstantiateFromPool(chainPrefab, chainPosition, Quaternion.identity));
            else chainsInPlace[chainPosition].SetActive(true);
        }

    }
    private void Update()
    {

        for (int j = 0; j < chainsInPlace.Count; j++)
        {
            var pair = chainsInPlace.ElementAt(j);
            if (pair.Value.activeSelf && pair.Key.y < transform.position.y)
            {
                //Debug.Log($"Posicao {pair.Key} de {pair.Value} eh menor que {transform.position}; Desativando {pair.Value}");
                pair.Value.SetActive(false);
            }
        }
    }
}
