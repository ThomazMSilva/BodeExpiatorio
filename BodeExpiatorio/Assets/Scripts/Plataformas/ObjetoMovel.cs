using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class ObjetoMovel : MonoBehaviour
{
    [SerializeField] private Vector3[] path ;
    private Vector3[] Path 
    { 
        get 
        {
            Vector3 transformPos = transform.position;
            Vector3[] nPath = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++) { Path[i] = transformPos + path[i]; }
            return nPath;
        }
    }
    [SerializeField] private float duration;
    [SerializeField] private LoopType loopType;
    [SerializeField] private Ease animEase;
    //bool isPathing = false;

    [SerializeField] bool isCoiso = false;
    [SerializeField] private PathType pathType;
    [SerializeField] private PathMode pathMode;
    [SerializeField] bool autoKill = false;

    void Start()
    {   
        if (isCoiso)
        {
            transform.DOMove(Path[0], duration).SetEase(animEase).SetLoops(-1,loopType);
            return;
        }

        DoPath();
    }

    /*private void Update()
    {
        if (isCoiso) return;
     
        if (Input.GetKeyDown(KeyCode.T))
        {
            autoKill = !autoKill;
        }
    }*/

    private void DoPath()
    {
        //isPathing = true;
        transform.
        DOPath(Path, duration, pathType, pathMode).
        SetEase(animEase).
        OnComplete
        (
            () =>
            {
                //isPathing = false;
                if (!autoKill)
                {
                    DoPath();
                }
            }
        );
    }

}
