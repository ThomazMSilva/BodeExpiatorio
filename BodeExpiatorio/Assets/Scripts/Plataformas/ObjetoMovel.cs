using UnityEngine;
using DG.Tweening;

public class ObjetoMovel : MonoBehaviour
{
    [SerializeField] private Vector3[] path ;
    private Vector3[] Path 
    { 
        get 
        {
            Vector3 transformPos = transform.position;
            Vector3[] nPath = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++) { nPath[i] = transformPos + path[i]; }
            return nPath;
        }
    }
    
    [SerializeField] private float duration;
    [SerializeField] private LoopType loopType;
    [SerializeField] private Ease animEase;
    //bool isPathing = false;

    private Tween pathTween;
    [SerializeField] bool isCoiso = false;
    [SerializeField] private PathType pathType;
    [SerializeField] private PathMode pathMode;
    [SerializeField, Tooltip("Para de se mover depois de terminar o caminho.")] bool autoKill = false;

    void Start()
    {   
        if (isCoiso)
        {
            pathTween = transform.DOMove(Path[0], duration).SetEase(animEase).SetLoops(-1,loopType);
            return;
        }

        DoPath();
    }

    private void DoPath()
    {
        //isPathing = true;
        pathTween = transform.
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
    private void OnDestroy() => pathTween?.Kill();
}
