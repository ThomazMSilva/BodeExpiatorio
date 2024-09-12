using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovimentoJogador : MonoBehaviour
{
    [Header("Configura��es de Movimento"), Space(8f)]

    [SerializeField] private string horizontalAxis = "Horizontal";

    //[SerializeField] private string verticalAxis = "Vertical";

    [SerializeField] private string jumpAxis = "Jump";

    [SerializeField] private string kneelAxis = "Fire3";

    [SerializeField] private string dashAxis = "Fire1";

    [SerializeField] private float moveSpeed = 5f;
    
        [Space(5f)]

    [Header("Configura��es de Gravidade"), Space(8f)]

    [SerializeField] private float fallGravityMultiplier = 5f; //"Gravidade" caindo (� um addforce)
    
    [SerializeField] private float jumpGravityMultiplier = 3f; //"Gravidade" qnd d� pulo grande

    [SerializeField] private float ragdollGravityMultiplier = 6f; //"Gravidade" pulando em armadilha

        [Space(8f)]

    [SerializeField] float spikeJumpGravityMultiplier = 0.5f;

    [SerializeField] float spikeKneelGravityMultiplier = 2f;

    public bool isSpikeJumpUnlocked = true;

        [Space(8f)]

    [SerializeField] private float raycastDistance = .75f; //Checagem de ch�o;

    [SerializeField] LayerMask groundLayer;

        [Space(5f)]

    [Header("Configura��es de Pulo"), Space(8f)]

    [SerializeField] private float jumpForce = 11f;

    [SerializeField] private float coyoteTimeMax = 0.1f; //Um tempinho que da pra pular depois de cair de alguma plataforma

    [SerializeField] private float jumpBufferTimeMax = 0.2f; //Um tempinho que se apertar o botao de pular antes de estar no chao, ainda conta qnd chegar no ch�o

    [SerializeField] private float shortJumpDelta = 2.5f;

        [Space(5f)]
    
    [Header("Configura��es de Arrancada"), Space(8f)]

    [SerializeField] private float dashForce = 100f;

    [SerializeField] private float dashCooldown = 1f;

    public bool isDashUnlocked = false;

        [Space(5f)]

    [Header("Configura��es de Prostra��o"), Space(8f)]

    [SerializeField, Range(0, 1)] private float kneelHeightMultiplier = 0.5f;

    [SerializeField, Range(0, 1)] private float kneelSpeedMultiplier = 0.7f;

    //[SerializeField] //tirar o SField dps
    private bool
        isGrounded = true,
        jumpKeyHeld = false,
        willJump = false,
        isLookingRight,
        isDashing = false,
        canDash = true,
        inRagdoll = false,
        //isClimbing = false,
        isKneeling;

    //[SerializeField] //tirar o SField dps
    private float
        horizontalInput,
        //verticalInput,
        jumpBufferTimeCurrent,
        coyoteTimeCurrent,
        dashTimeCurrent = 0;

    private Vector3 
        gravity,
        colliderBaseSize,
        colliderKneelingSize,
        colliderBaseCenter,
        colliderKneelingCenter;

    private BoxCollider playerCollider;

    private Rigidbody rb;

    public delegate void PlayerFlipped();
    public event PlayerFlipped OnPlayerTurned;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerCollider = GetComponent<BoxCollider>();
        
        colliderBaseSize = playerCollider.size;
        colliderKneelingSize = 
            new(colliderBaseSize.x, colliderBaseSize.y * kneelHeightMultiplier, colliderBaseSize.z);
        
        colliderBaseCenter = playerCollider.center;
        colliderKneelingCenter = 
            new(colliderBaseCenter.x, colliderBaseCenter.y - (colliderKneelingSize.y * 0.5f), colliderBaseCenter.z);

        gravity = Physics.gravity;
        //raycastDistance = transform.localScale.y + 0.05f;
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
        horizontalInput = Input.GetAxis(horizontalAxis);

        if (Input.GetButtonDown(jumpAxis))
        {
            StopAllCoroutines();
            StartCoroutine(JumpBuffer());
            jumpKeyHeld = true;
        }

        if (Input.GetButtonUp(jumpAxis))
            jumpKeyHeld = false;

        if (Input.GetButtonDown(kneelAxis))
        {
            isKneeling = true;
            HandleKneel(isKneeling);
        }

        if (Input.GetButtonUp(kneelAxis))
        {
            isKneeling = false;
            HandleKneel(isKneeling);
        }

        if (isDashUnlocked && Input.GetButtonDown(dashAxis) && canDash)
            isDashing = true;
        
        //Determina��es de entrada
        if ((horizontalInput > 0 && !isLookingRight) || (horizontalInput < 0 && isLookingRight))
            FlipSprite();

        //verticalInput = Input.GetAxisRaw("Vertical");
        //isKneeling = verticalInput < 0 && !isClimbing;
    }

    private void FlipSprite()
    {
        isLookingRight = !isLookingRight;
        OnPlayerTurned?.Invoke();
    }

    public void Ragdoll() => inRagdoll = true;

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, -Vector3.up,out RaycastHit hit, raycastDistance, groundLayer);
        
        //Tentando fazer oq o felipe de prog  comentou na aula
        transform.parent = hit.transform;

        Debug.DrawRay(transform.position, -Vector3.up * raycastDistance, Color.red);

        coyoteTimeCurrent = isGrounded ? coyoteTimeMax : coyoteTimeCurrent - Time.fixedDeltaTime;

        if (inRagdoll && isGrounded) 
            inRagdoll = false; //Reativa a gravidade de pulo
    }

    private void ApplyGravity()
    {
        if (isGrounded)
            return;
        
        //Pulando e caindo
        float gravityScale = rb.velocity.y < 0 ? fallGravityMultiplier : jumpGravityMultiplier;
        rb.AddForce(gravity * (gravityScale - 1f), ForceMode.Acceleration);

        if (rb.velocity.y <= 0)
            return;

        //Em armadilha
        if (inRagdoll)
        {
            float forceMultiplier = ragdollGravityMultiplier;

            if (!isKneeling)
                forceMultiplier *= jumpKeyHeld && isSpikeJumpUnlocked ? spikeJumpGravityMultiplier : 1f;

            else 
                forceMultiplier *= isSpikeJumpUnlocked ? spikeKneelGravityMultiplier : 1f;

            rb.AddForce( forceMultiplier * gravity, ForceMode.Acceleration );
            return;
        }

        //Pulo curto
        if (!jumpKeyHeld)
            rb.AddForce(fallGravityMultiplier * shortJumpDelta * gravity, ForceMode.Acceleration);
        
    }

    private void Move()
    {
        float speed = isKneeling ? moveSpeed * kneelSpeedMultiplier : moveSpeed;
        Vector3 moveForce = new(horizontalInput * speed - rb.velocity.x, 0, 0);
        rb.AddForce(moveForce, ForceMode.VelocityChange);
    }

    private void HandleJump()
    {
        if (willJump && (isGrounded || coyoteTimeCurrent > 0f) && !isKneeling)
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

    private void HandleKneel(bool willKneel)
    {
        Vector3 size = willKneel ? colliderKneelingSize : colliderBaseSize;
        Vector3 center  = willKneel ? colliderKneelingCenter : colliderBaseCenter;

        playerCollider.size = size;
        playerCollider.center = center;
    }

    public void ApplyForce(Vector3 force, ForceMode forceMode)
    {
        rb.AddForce(force, forceMode);
    }
}