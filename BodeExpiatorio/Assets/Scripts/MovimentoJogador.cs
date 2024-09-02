using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovimentoJogador : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    
    [SerializeField] private float moveSpeed = 5f;

        [Space(5f)]

    [Header("Configurações de Pulo")]
    
    [SerializeField] private float jumpForce = 11f;

    [SerializeField] private float coyoteTimeMax = 0.3f; //Um tempinho que da pra pular depois de cair de alguma plataforma
    
    [SerializeField] private float jumpBufferTimeMax = 0.25f; //Um tempinho que se apertar o botao de pular antes de estar no chao, ainda conta qnd chegar no chão
    
    [SerializeField] private float shortJumpDelta = 2.5f;
    
        [Space(5f)]

    [Header("Configurações de Gravidade")]

    [SerializeField] private float fallGravityMultiplier = 2.5f; //"Gravidade" caindo (é um addforce)
    
    [SerializeField] private float jumpFallMultiplier = 2f; //"Gravidade" qnd dá pulo grande,
    
    [SerializeField] private float raycastDistance = .75f; //Checagem de chão;

        [Space(5f)]
    
    [Header("Configurações de Arrancada")]

    [SerializeField] private float dashForce = 100f;

    [SerializeField] private float dashCooldown = 1f;

    [SerializeField] LayerMask groundLayer; // Layer for ground detection

    [SerializeField] //tirar dps
    private bool
        isGrounded = true,
        jumpKeyHeld = false,
        willJump,
        isLookingRight,
        isDashing = false,
        canDash = true;

    [SerializeField] //tirar dps
    private float
        jumpBufferTimeCurrent,
        coyoteTimeCurrent,
        dashTimeCurrent = 0,
        moveInput;

    private Vector3 gravity;

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
        HandleDash();

        Debug.Log(rb.velocity);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, raycastDistance, groundLayer);
        Debug.DrawRay(transform.position, -Vector3.up * raycastDistance, Color.red);

        coyoteTimeCurrent = isGrounded ? coyoteTimeMax : coyoteTimeCurrent - Time.fixedDeltaTime;
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

        if (moveInput > 0) isLookingRight = true;

        if (moveInput < 0) isLookingRight = false;

        if (Input.GetButtonDown("Jump"))
        {
            StopAllCoroutines();
            StartCoroutine(JumpBuffer());
            jumpKeyHeld = true;
        }

        if (Input.GetButtonUp("Jump"))
        {
            jumpKeyHeld = false;
        }

        if (Input.GetButtonDown("Fire3") && canDash)
        {
            isDashing = true;
        }
    }

    IEnumerator JumpBuffer()
    {
        jumpBufferTimeCurrent = 0;
        while (jumpBufferTimeCurrent < jumpBufferTimeMax)
        {
            willJump = true;
            jumpBufferTimeCurrent += Time.deltaTime;
            yield return null;
        }
        willJump = false;
    }

    private void Move()
    {
        Vector3 moveForce = new(moveInput * moveSpeed - rb.velocity.x, 0, 0);
        rb.AddForce(moveForce, ForceMode.VelocityChange);
    }

    private void HandleJump()
    {
        if (isGrounded && willJump && coyoteTimeCurrent > 0f)
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

    private void HandleDash()
    {
        if (!canDash)
        {
            dashTimeCurrent += Time.fixedDeltaTime;
            if (dashTimeCurrent >= dashCooldown) canDash = true;
        }

        if (isDashing)
        {
            rb.AddForce((isLookingRight ? Vector3.right : -Vector3.right) * dashForce, ForceMode.Impulse);
            isDashing = false;
            canDash = false;
        }
    }

}