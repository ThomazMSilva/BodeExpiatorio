using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovimentoJogador : MonoBehaviour
{
    [Header("Configurações de Movimento"), Space(8f)]

    [SerializeField] private string horizontalAxis = "Horizontal";

    //[SerializeField] private string verticalAxis = "Vertical";

    [SerializeField] private string jumpAxis = "Jump";

    [SerializeField] private string kneelAxis = "Fire3";

    [SerializeField] private string dashAxis = "Fire1";

    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("Se ativo, a velocidade maxima que o Jogador pode se mover tem um limite.")]
    [SerializeField] private bool clampVelocity = true;

    [Tooltip("A velocidade maxima, em unidades/segundo, que o Jogador podera se mover pra qualquer direcao.")]
    [SerializeField] private float terminalVelocity = 30f;

    [Tooltip("Se ativo, o Jogador pode mudar de direcao no ar imediatamente com o Input Horizontal.\nSe inativo, ele vai acelerar gradualmente para a direcao do input.")]
    [SerializeField] private bool airControl = true;

    [Tooltip("So usar caso Air Control esteja INATIVO. \nA aceleracao com que ele muda de direcao no ar")]
    [SerializeField] private float airAccelerationMultiplier = 3;

    [Tooltip("Se ATIVO, o Jogador não tem nenhum controle de sua velocidade horizontal enquanto estiver em ragdoll.")]
    [SerializeField] private bool allowRagdollMomentum = false;
    
        [Space(5f)]

    [Header("Configurações de Gravidade"), Space(8f)]

    [SerializeField] private float fallGravityMultiplier = 5f; //"Gravidade" caindo (é um addforce)
    
    [SerializeField] private float jumpGravityMultiplier = 3f; //"Gravidade" qnd dá pulo grande

    [SerializeField] private float ragdollGravityMultiplier = 6f; //"Gravidade" pulando em armadilha

        [Space(8f)]

    [SerializeField] private float spikeJumpGravityMultiplier = 0.5f;

    [SerializeField] private float spikeKneelGravityMultiplier = 2f;

    [Tooltip("Se ativo, o Jogador pode controlar a altura que e lancado pelas armadilhas com seu pulo e prostracao.")]
    public bool isSpikeJumpUnlocked = true;

        [Space(8f)]

    [SerializeField] private float raycastDistance = .75f; //Checagem de chão;

    [SerializeField] LayerMask groundLayer;

        [Space(5f)]

    [Header("Configurações de Pulo"), Space(8f)]

    [SerializeField] private float jumpForce = 11f;

    [SerializeField] private float coyoteTimeMax = 0.1f; //Um tempinho que da pra pular depois de cair de alguma plataforma

    [SerializeField] private float jumpBufferTimeMax = 0.2f; //Um tempinho que se apertar o botao de pular antes de estar no chao, ainda conta qnd chegar no chão

    [SerializeField] private float shortJumpDelta = 2.5f;

        [Space(5f)]
    
    [Header("Configurações de Arrancada"), Space(8f)]

    [SerializeField] private float dashForce = 100f;

    [SerializeField] private float dashCooldown = 1f;

    public bool isDashUnlocked = false;

        [Space(5f)]

    [Header("Configurações de Prostração"), Space(8f)]

    [SerializeField, Range(0, 1)] private float kneelHeightMultiplier = 0.5f;

    [SerializeField, Range(0, 1)] private float kneelSpeedMultiplier = 0.7f;

    //[SerializeField]
    private bool
        isGrounded = true,
        jumpKeyHeld = false,
        willJump = false,
        isLookingRight,
        isDashing = false,
        canDash = true,
        inRagdoll = false,
        isStunned = false,
        //isClimbing = false,
        isKneeling;

    //[SerializeField]
    private float
        horizontalInput,
        //verticalInput,
        jumpBufferTimeCurrent,
        coyoteTimeCurrent,
        stunTimeRemaining;

    private Vector3 
        gravity,
        colliderBaseSize,
        colliderKneelingSize,
        colliderBaseCenter,
        colliderKneelingCenter,
        moveForce = Vector3.zero,  
        gravityForce = Vector3.zero;

    private WaitForSeconds dashCooldownWait;

    private Coroutine stunCoroutine;

    private BoxCollider playerCollider;

    private Rigidbody rb;

    private RaycastHit hit;

    public delegate void PlayerFlipped();
    public event PlayerFlipped OnPlayerTurned;

    //Private methods
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerCollider = GetComponent<BoxCollider>();
        
        colliderBaseSize = playerCollider.size;
        colliderKneelingSize.Set(colliderBaseSize.x, colliderBaseSize.y * kneelHeightMultiplier, colliderBaseSize.z);
        
        colliderBaseCenter = playerCollider.center;
        colliderKneelingCenter.Set(colliderBaseCenter.x, colliderBaseCenter.y - (colliderKneelingSize.y * 0.5f), colliderBaseCenter.z);

        gravity = Physics.gravity;

        dashCooldownWait = new WaitForSeconds(dashCooldown);
    }

    private void Update() => HandleInput();
    
    private void HandleInput()
    {
        horizontalInput = Input.GetAxis(horizontalAxis); 

        if (Input.GetButtonDown(jumpAxis))
        {
            StopCoroutine(JumpBuffer());
            StartCoroutine(JumpBuffer());
            jumpKeyHeld = true;
        }
        if (Input.GetButtonUp(jumpAxis))
        {
            jumpKeyHeld = false;
        }


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
        

        if (Input.GetButtonDown(dashAxis))
        {
            if(isDashUnlocked && canDash)
            {
                isDashing = true;

                StartCoroutine(DashCooldown());
            }
        }
        
        if ((horizontalInput > 0 && !isLookingRight) || (horizontalInput < 0 && isLookingRight))
            FlipSprite();

        //verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void FlipSprite()
    {
        isLookingRight = !isLookingRight;
        OnPlayerTurned?.Invoke();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        ApplyGravity();
        HandleHorizontalMovement();
        HandleJump();
        HandleDash();

        if(clampVelocity) rb.velocity = Vector3.ClampMagnitude(rb.velocity, terminalVelocity);
        //Debug.Log(rb.velocity);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, -Vector3.up,out hit, raycastDistance, groundLayer);
        
        //Tentando fazer oq o felipe de prog  comentou na aula
        transform.parent = hit.transform;

        Debug.DrawRay(transform.position, -Vector3.up * raycastDistance, Color.red);

        coyoteTimeCurrent = isGrounded ? coyoteTimeMax : coyoteTimeCurrent - Time.fixedDeltaTime;

        if (inRagdoll && isGrounded) 
            inRagdoll = false; //Reativa a gravidade de pulo

        if (isStunned && isGrounded)
        {
            isStunned = false; //Reativa o movimento horizontal
            stunTimeRemaining = 0;
        }
    }

    private void ApplyGravity()
    {
        if (isGrounded)
            return;
        
        //Pulando e caindo
        float gravityScale = rb.velocity.y < 0 ? fallGravityMultiplier : jumpGravityMultiplier;
        gravityForce = gravity * (gravityScale - 1f);
        rb.AddForce(gravityForce, ForceMode.Acceleration);

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

    private void HandleHorizontalMovement()
    {
        if (allowRagdollMomentum && isStunned) return;

        float speed = isKneeling ? moveSpeed * kneelSpeedMultiplier : moveSpeed;
        moveForce.Set(horizontalInput * speed - rb.velocity.x, 0, 0);
        
        if (airControl) //Muda velocidade horizontal no ar instantaneamente
        {
            rb.AddForce(moveForce, ForceMode.VelocityChange);
            return;
        }
        
        rb.AddForce //Muda velocidade horizontal no ar gradualmente
        (
            isGrounded ? moveForce : moveForce * airAccelerationMultiplier, 
            isGrounded ? ForceMode.VelocityChange : ForceMode.Acceleration
        );
    }

    private void HandleJump()
    {
        if (isKneeling) return;

        if (willJump && (coyoteTimeCurrent > 0f))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpBufferTimeCurrent = 0f;
            coyoteTimeCurrent = 0f;
        }
    }    

    private void HandleKneel(bool willKneel)
    {
        Vector3 size = willKneel ? colliderKneelingSize : colliderBaseSize;
        Vector3 center  = willKneel ? colliderKneelingCenter : colliderBaseCenter;

        playerCollider.size = size;
        playerCollider.center = center;
    }

    private void HandleDash()
    {
        if (isDashing)
        {
            rb.AddForce((isLookingRight ? Vector3.right : -Vector3.right) * dashForce, ForceMode.Impulse);
            isDashing = false;
        }
    }

    private IEnumerator DashCooldown()
    {
        canDash = false;

        yield return dashCooldownWait;

        canDash = true;
    }

    private IEnumerator JumpBuffer()
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

    private IEnumerator Stun()
    {
        isStunned = true;
        
        while(stunTimeRemaining > 0)
        {
            stunTimeRemaining -= Time.deltaTime;
            yield return null;
        }
        isStunned = false;
        stunCoroutine = null;
    }

    //Public methods
    public void Ragdoll(float stunTimeSecs)
    {
        inRagdoll = true;

        stunTimeRemaining += stunTimeSecs;

        stunCoroutine ??= StartCoroutine(Stun());
    }
   
    public void ApplyForce(Vector3 force, ForceMode forceMode)
    {
        rb.AddForce(force, forceMode);
    }

}