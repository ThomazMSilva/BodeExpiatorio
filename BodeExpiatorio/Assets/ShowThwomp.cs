using UnityEngine;

public class ShowThwomp : MonoBehaviour
{
    [SerializeField] private GameObject monitoredObject; 
    [SerializeField] private SuplicioBrutalLaser thwompScript;

    private void Update()
    {
       
        if (monitoredObject != null && !monitoredObject.activeSelf)
        {
           
            if (thwompScript != null)
            {
                thwompScript.SendMessage("SetIsFalling", true, SendMessageOptions.DontRequireReceiver);

                
                StartCoroutine(DeactivateFalling());
            }
        }
    }

    private System.Collections.IEnumerator DeactivateFalling()
    {
        
        yield return null;

        if (thwompScript != null)
        {
            thwompScript.SendMessage("SetIsFalling", false, SendMessageOptions.DontRequireReceiver);
        }
    }
}
