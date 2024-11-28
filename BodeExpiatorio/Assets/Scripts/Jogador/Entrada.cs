using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class Entrada : MonoBehaviour
{
    private static Entrada instance;
    public static Entrada Instance { get => instance; }
    private void Awake()
    {
        instance = this;
        playerInputActions = new();
    }

    [SerializeField] private PlayerInputActions playerInputActions;
    private InputAction movementAction;
    private InputAction kneelAction;
    private InputAction jumpAction;
    private InputAction pauseAction;

    [Space(8f), Header("Old Input System")]
    [SerializeField] private string horizontalAxis = "Horizontal";

    [SerializeField] private string verticalAxis = "Vertical";

    [SerializeField] private string jumpAxis = "Jump";

    [SerializeField] private string kneelAxis = "Kneel";

    [SerializeField] private string pauseAxis = "Pause";

    private Vector2 movementInput;
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

    //public void Awake() => playerInputActions = new();

    private void OnEnable()
    {
        movementAction = playerInputActions.Player.Move;
        movementAction.Enable();
        //movementAction.performed += MovementAction_performed;

        kneelAction = playerInputActions.Player.Kneel;
        kneelAction.Enable();
        kneelAction.performed += KneelAction_performed;
        kneelAction.canceled += KneelAction_canceled; ;

        jumpAction = playerInputActions.Player.Jump;
        jumpAction.Enable();
        jumpAction.performed += JumpAction_performed;
        jumpAction.canceled += JumpAction_canceled;

        pauseAction = playerInputActions.Player.Pause;
        pauseAction.Enable();
        pauseAction.performed += PauseAction_performed;
        
    }

    private void OnDisable()
    {
        movementAction.Disable();
        kneelAction.Disable();
        jumpAction.Disable();
        pauseAction.Disable();
    }

    int pauseCount = 0;
    private void PauseAction_performed(InputAction.CallbackContext obj)
    {
        if (GameManager.Instance.IsLoading) return;
        pauseCount++;
        Debug.Log($"Evento de pause tocado {pauseCount} vezes");
        OnPauseButtonDown?.Invoke();
    }

    private void JumpAction_performed(InputAction.CallbackContext obj)
    {
        if (GameManager.Instance.IsLoading || Pause.Pausado) return;
        OnJumpButtonDown?.Invoke();
    }

    private void JumpAction_canceled(InputAction.CallbackContext obj)
    {
        if (GameManager.Instance.IsLoading || Pause.Pausado) return;
        OnJumpButtonUp?.Invoke();
    }

    private void KneelAction_performed(InputAction.CallbackContext obj)
    {
        if (GameManager.Instance.IsLoading || Pause.Pausado) return;
        OnKneelButtonDown?.Invoke();
    }

    private void KneelAction_canceled(InputAction.CallbackContext obj)
    {
        if (GameManager.Instance.IsLoading || Pause.Pausado) return;
        OnKneelButtonUp?.Invoke();
    }

    private void Update()
    {
        if (GameManager.Instance.IsLoading || Pause.Pausado) return;

        movementInput = movementAction.ReadValue<Vector2>();
        horizontalInput = movementInput.x;
        verticalInput = movementInput.y;
    }

    // Update is called once per frame
    /*private void Update()
    {

        horizontalInput = Input.GetAxis(horizontalAxis);
        verticalInput = Input.GetAxisRaw(verticalAxis);

        if (GameManager.Instance.IsLoading || Pause.Pausado) return;

        if (Input.GetButtonDown(jumpAxis)) OnJumpButtonDown?.Invoke();

        if (Input.GetButtonUp(jumpAxis)) OnJumpButtonUp?.Invoke(); 

        if (Input.GetButtonDown(kneelAxis)) OnKneelButtonDown?.Invoke();

        if (Input.GetButtonUp(kneelAxis)) OnKneelButtonUp?.Invoke();

    }*/
}
