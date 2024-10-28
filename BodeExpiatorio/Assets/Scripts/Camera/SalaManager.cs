using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SalaManager : MonoBehaviour
{
    public GameObject VirtualCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            VirtualCamera.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            VirtualCamera.SetActive(false);
        }
    }
}
