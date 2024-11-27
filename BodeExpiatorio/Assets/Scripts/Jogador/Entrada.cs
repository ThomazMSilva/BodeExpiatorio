using System.Collections;
using UnityEngine;

public class Entrada : MonoBehaviour
{
    private static Entrada instance;
    public static Entrada Instance { get => instance; }
    private void Awake() => instance = this;

    [SerializeField] private string horizontalAxis = "Horizontal";

    [SerializeField] private string verticalAxis = "Vertical";

    [SerializeField] private string jumpAxis = "Jump";

    [SerializeField] private string kneelAxis = "Fire3";

    [SerializeField] private string pauseAxis = "Cancel";

    private float horizontalInput;
    private float verticalInput;

    public float HorizontalInput { get => horizontalInput; private set => horizontalInput = value; }
    public float VerticalInput { get => verticalInput; private set => verticalInput = value; }

    public delegate void EventHandler();
    public event EventHandler OnJumpButtonDown;
    public event EventHandler OnJumpButtonUp;
    public event EventHandler OnKneelButtonDown;
    public event EventHandler OnKneelButtonUp;
    public event EventHandler OnPauseButtonDown;

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.Instance.IsLoading) return;

        if (Input.GetButtonDown(pauseAxis)) OnPauseButtonDown?.Invoke();

        if (Pause.Pausado) return;

        horizontalInput = Input.GetAxis(horizontalAxis);
        verticalInput = Input.GetAxisRaw(verticalAxis);

        if (Input.GetButtonDown(jumpAxis)) OnJumpButtonDown?.Invoke();

        if (Input.GetButtonUp(jumpAxis)) OnJumpButtonUp?.Invoke(); 

        if (Input.GetButtonDown(kneelAxis)) OnKneelButtonDown?.Invoke();

        if (Input.GetButtonUp(kneelAxis)) OnKneelButtonUp?.Invoke();

    }
}
