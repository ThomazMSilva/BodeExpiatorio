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

    [SerializeField] private float coyoteTimeMax = 0.1f; //Um tempinho que da pra pular depois de cair de alguma plataforma
    
    [SerializeField] private float jumpBufferTimeMax = 0.2f; //Um tempinho que se apertar o botao de pular antes de estar no chao, ainda conta qnd chegar no chão
    
    [SerializeField] private float shortJumpDelta = 2.5f;
    
        [Space(5f)]

    [Header("Configurações de Gravidade")]

    [SerializeField] private float fallGravityMultiplier = 5f; //"Gravidade" caindo (é um addforce)
    
    [SerializeField] private float jumpGravityMultiplier = 3f; //"Gravidade" qnd dá pulo grande

    [SerializeField] private float ragdollGravityMultiplier = 6f; //"Gravidade" pulando em armadilha
    
    [SerializeField] private float raycastDistance = .75f; //Checagem de chão;

        [Space(5f)]
    
    [Header("Configurações de Arrancada")]

    [SerializeField] private float dashForce = 100f;

    [SerializeField] private float dashCooldown = 1f;

    [SerializeField] LayerMask groundLayer;

    //[SerializeField] //tirar o SField dps
    private bool
        isGrounded = true,
        jumpKeyHeld = false,
        willJump,
        isLookingRight,
        isDashing = false,
        canDash = true,
        inRagdoll = false;

    //[SerializeField] //tirar o SField dps
    private float
        jumpBufferTimeCurrent,
        coyoteTimeCurrent,
        dashTimeCurrent = 0,
        moveInput;

    private Vector3 gravity;

    private Rigidbody rb;

    [SerializeField] bool canSpikeJump = true;
    [SerializeField] float spikeGravityMultiplier = 0.5f;

    public delegate void PlayerFlipped();
    public event PlayerFlipped OnPlayerTurned;

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

        //Debug.Log(rb.velocity);
    }

    private void HandleInput()
    {
        moveInput = Input.GetAxis("Horizontal");

        if ((moveInput > 0 && !isLookingRight) || (moveInput < 0 && isLookingRight)) 
            FlipSprite();

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

    private void FlipSprite()
    {
        isLookingRight = !isLookingRight;
        OnPlayerTurned?.Invoke();
    }

    public void Ragdoll() => inRagdoll = true;

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, raycastDistance, groundLayer);
        Debug.DrawRay(transform.position, -Vector3.up * raycastDistance, Color.red);

        coyoteTimeCurrent = isGrounded ? coyoteTimeMax : coyoteTimeCurrent - Time.fixedDeltaTime;

        if (inRagdoll && isGrounded) 
            inRagdoll = false; //Reativa a gravidade de pulo
    }

    private void ApplyGravity()
    {
        if (isGrounded)
            return;
        
        float gravityScale = rb.velocity.y < 0 ? fallGravityMultiplier : jumpGravityMultiplier;
        rb.AddForce(gravity * (gravityScale - 1f), ForceMode.Acceleration);

        //Gravidade específica de pulo curto e ragdoll
        if (rb.velocity.y > 0)
        {
            if (inRagdoll)
            {
                rb.AddForce
                (
                    (jumpKeyHeld && canSpikeJump ? ragdollGravityMultiplier * spikeGravityMultiplier : ragdollGravityMultiplier) * gravity, 
                    ForceMode.Acceleration
                );
                return;
            }

            if (!jumpKeyHeld)
                rb.AddForce(fallGravityMultiplier * shortJumpDelta * gravity, ForceMode.Acceleration);
        }
    }


    private void Move()
    {
        Vector3 moveForce = new(moveInput * moveSpeed - rb.velocity.x, 0, 0);
        rb.AddForce(moveForce, ForceMode.VelocityChange);
    }

    private void HandleJump()
    {
        if (willJump && (isGrounded || coyoteTimeCurrent > 0f))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpBufferTimeCurrent = 0f;
            coyoteTimeCurrent = 0f;
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

    public void ApplyForce(Vector3 force, ForceMode forceMode)
    {
        rb.AddForce(force, forceMode);
    }
}