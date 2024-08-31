using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovimentoJogador : MonoBehaviour
{
    [Header("Movement Settings"), SerializeField] float moveSpeed = 5f;
    [SerializeField]
    float
        jumpForce = 11f,
        coyoteTimeMax = 0.3f, //Um tempinho que da pra pular depois de cair de alguma plataforma
        jumpBufferTimeMax = 0.25f, //Um tempinho que se apertar o botao de pular antes de estar no chao, ainda conta qnd chegar no chão
        fallGravityMultiplier = 2.5f, //"Gravidade" caindo (é um addforce)
        jumpFallMultiplier = 2f, //"Gravidade" qnd dá pulo grande,
        raycastDistance = .75f, //Checagem de chão;
        shortJumpDelta = 2.5f;
        

    [SerializeField] LayerMask groundLayer; // Layer for ground detection

    private Vector3 gravity;

    [SerializeField] //tirar dps
    private bool
        isGrounded,
        jumpKeyHeld;

    //[SerializeField] //tirar dps
    private float
        jumpBufferTimeCurrent,
        coyoteTimeCurrent,
        moveInput;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gravity = Physics.gravity;
        raycastDistance = transform.localScale.y + 0.05f;
    }

    private void Update() => HandleInput();

    private void FixedUpdate()
    {
        CheckGrounded();
        ApplyGravity();
        Move();
        HandleJump();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, raycastDistance, groundLayer);
        Debug.DrawRay(transform.position, -Vector3.up * raycastDistance, Color.red);

        coyoteTimeCurrent = isGrounded? coyoteTimeMax : coyoteTimeCurrent - Time.fixedDeltaTime;
    }

    private void ApplyGravity()
    {
        if (isGrounded)
        {
            //rb.velocity = new Vector3(rb.velocity.x, Mathf.Max(rb.velocity.y, 0), rb.velocity.z);
            return;
        }

        float gravityScale = rb.velocity.y < 0 ? fallGravityMultiplier : jumpFallMultiplier;
        rb.AddForce(gravity * (gravityScale - 1f), ForceMode.Acceleration);
    }

    private void HandleInput()
    {
        moveInput = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimeCurrent = jumpBufferTimeMax;
            jumpKeyHeld = true;
        }

        else
        {
            jumpBufferTimeCurrent -= Time.deltaTime;
        }

        if (Input.GetButtonUp("Jump"))
        {
            jumpKeyHeld = false;
        }

    }

    private void Move()
    {
        Vector3 moveForce = new(moveInput * moveSpeed - rb.velocity.x, 0, 0);
        rb.AddForce(moveForce, ForceMode.VelocityChange);
    }

    private void HandleJump()
    {
        if (jumpBufferTimeCurrent > 0f && coyoteTimeCurrent > 0f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpBufferTimeCurrent = 0f;
            coyoteTimeCurrent = 0f;
        }

        if (!jumpKeyHeld && rb.velocity.y > 0)
        {
            rb.AddForce(fallGravityMultiplier * shortJumpDelta * gravity, ForceMode.Acceleration);
        }
    }
}
