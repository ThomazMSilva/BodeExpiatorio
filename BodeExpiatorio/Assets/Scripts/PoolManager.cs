using DG.Tweening.Core.Easing;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static PoolManager _Instance;
    public static PoolManager Instance
    {
        get
        {
            if (!_Instance)
            {
                Debug.Log("Criou pool");
                var prefab = Resources.Load<GameObject>("Prefabs/Pool");

                var inScene = Instantiate(prefab);

                _Instance = inScene.GetComponentInChildren<PoolManager>();
                if (!_Instance) _Instance = inScene.AddComponent<PoolManager>();
                DontDestroyOnLoad(_Instance.transform.root.gameObject);
            }
            return _Instance;
        }
    }

    private Dictionary<string, List<GameObject>> objectPools = new();

    public GameObject InstantiateFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string poolKey = prefab.name;

        if (!objectPools.ContainsKey(poolKey))
        {
            CreateObjectPool(prefab);
        }

        GameObject obj = GetObjectFromPool(poolKey);
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        return obj;
    }

    private void CreateObjectPool(GameObject prefab)
    {
        string poolKey = prefab.name;
        List<GameObject> objectPool = new();
        objectPools.Add(poolKey, objectPool);

        GameObject poolParent = new("Pool_" + poolKey);
        poolParent.transform.parent = transform;

        for (int i = 0; i < 10; i++)  // Cria uma pool direto com no minimo 10 elementos, mas da pra mudar
        {
            GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            obj.SetActive(false);
            obj.transform.parent = poolParent.transform;
            objectPool.Add(obj);
        }
    }

    private GameObject GetObjectFromPool(string poolKey)
    {
        List<GameObject> objectPool = objectPools[poolKey];

        foreach (GameObject obj in objectPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        // Cria um novo se nao tiver achado um inativo
        GameObject newObj = Instantiate(objectPool[0]);
        newObj.transform.parent = objectPool[0].transform.parent;
        objectPool.Add(newObj);

        return newObj;
    }

}
