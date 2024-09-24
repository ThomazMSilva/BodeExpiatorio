using System.Collections;
using UnityEngine;

public class Entrada : MonoBehaviour
{
    public static Entrada Instance;
    // Use this for initialization
    private void Awake() => Instance = this;

    [SerializeField] private string horizontalAxis = "Horizontal";

    //[SerializeField] private string verticalAxis = "Vertical";

    [SerializeField] private string jumpAxis = "Jump";

    [SerializeField] private string kneelAxis = "Fire3";

    private float horizontalInput;

    public float HorizontalInput { get => horizontalInput; private set => horizontalInput = HorizontalInput; }

    public delegate void EventHandler();
    public event EventHandler OnJumpButtonDown;
    public event EventHandler OnJumpButtonUp;
    public event EventHandler OnKneelButtonDown;
    public event EventHandler OnKneelButtonUp;

    // Update is called once per frame
    private void Update()
    {
        horizontalInput = Input.GetAxis(horizontalAxis);
        //verticalInput = Input.GetAxisRaw(verticalAxis);

        if (Input.GetButtonDown(jumpAxis)) OnJumpButtonDown?.Invoke();

        if (Input.GetButtonUp(jumpAxis)) OnJumpButtonUp?.Invoke(); 

        if (Input.GetButtonDown(kneelAxis)) OnKneelButtonDown?.Invoke();

        if (Input.GetButtonUp(kneelAxis)) OnKneelButtonUp?.Invoke();
    }
}
