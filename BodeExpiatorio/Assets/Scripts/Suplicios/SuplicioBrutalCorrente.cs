using System;
using System.Collections.Generic;
using UnityEngine;

public class SuplicioBrutalCorrente : MonoBehaviour
{
    [SerializeField] private GameObject chainPrefab;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float rayDistance;
    [SerializeField] private float chainSize = .05f;
    private float xPos;
    private int _id;
    private Vector3 endPoint;
    private RaycastHit hit;
    private List<GameObject> chainList = new();

    private void Start()
    {
        _id = Guid.NewGuid().GetHashCode();
        xPos = transform.position.x;
        UpdateEndPoint();   
    }

    void FixedUpdate()
    {
        if(xPos != transform.position.x)
        {
            xPos = transform.position.x;
            UpdateEndPoint();
        }

        int chainLinkAmount = Mathf.RoundToInt((endPoint.y - transform.position.y) / chainSize);

        UpdateChains(chainLinkAmount);
        DeactivateChainBelow(transform.position.y);
    }

    private void UpdateChains(int chainLinkAmount)
    {
        while (chainList.Count < chainLinkAmount)
        {
            GameObject newChain = PoolManager.Instance.InstantiateFromPool(chainPrefab, transform.position, Quaternion.identity, $"{this}_{_id}");
            chainList.Add(newChain);
        }

        for (int i = 0; i < chainLinkAmount; i++)
        {
            Vector3 chainPosition = new(endPoint.x, endPoint.y - (chainSize * i), endPoint.z);

            GameObject chain = chainList[i];

            if (chain.transform.position != chainPosition)
            {
                chain.transform.position = chainPosition;
            }

            if (!chain.activeSelf)
            {
                chain.SetActive(true);
                //AudioManager.Instance.PlayerOneShot(FMODEvents.Instance.BrutalityMoved, transform.position);
            }
        }
    }

    private void UpdateEndPoint()
    {
        endPoint = Physics.Raycast(transform.position, transform.up, out hit, rayDistance, layerMask)
            ? hit.point
            : transform.position + Vector3.up * 10;
        DeactivateChainBelow(1000f);
    }

    private void DeactivateChainBelow(float y)
    {
        foreach (var chain in chainList)
        {
            if (!chain.activeSelf) continue;
            if (chain.transform.position.y < transform.position.y || chain.transform.position.y > endPoint.y)
            {
                chain.SetActive(false);
                //AudioManager.Instance.PlayerOneShot(FMODEvents.Instance.BrutalityMoved, transform.position);
            }
        }
    }
}
