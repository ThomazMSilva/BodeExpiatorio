using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovimentoJogador : MonoBehaviour
{
    [Header("Configurações de Movimento"), Space(8f)]

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

    [SerializeField] private float maxStunTime = 4f;

        [Space(5f)]

    [Header("Configurações de Gravidade"), Space(8f)]

    [SerializeField] private float fallGravityMultiplier = 5f; //"Gravidade" caindo (é um addforce)
    
    [SerializeField] private float jumpGravityMultiplier = 3f; //"Gravidade" qnd dá pulo grande

        [Space(8f)]

    [SerializeField] private float ragdollGravityMultiplier = 6f; //"Gravidade" pulando em armadilha

    [SerializeField] private float ragdollJumpGravityMultiplier = 0.5f;

    [SerializeField] private float ragdollKneelGravityMultiplier = 2f;

    [Tooltip("Se ativo, o Jogador pode controlar a altura que e lancado pelas armadilhas com seu pulo e prostracao.")]
    public bool isRagdollJumpUnlocked = true;

        [Space(8f)]

    [SerializeField] private float raycastDistance = .75f; //Checagem de chão;

    [SerializeField] LayerMask groundLayer;

        [Space(5f)]

    [Header("Configurações de Pulo"), Space(8f)]

    [SerializeField] private float jumpForce = 11f;

    [SerializeField] private float coyoteTimeMax = 0.1f; //Um tempinho que da pra pular depois de cair de alguma plataforma

    [SerializeField] private float jumpBufferTimeMax = 0.2f; //Um tempinho que se apertar o botao de pular antes de estar no chao, ainda conta qnd chegar no chão

    [SerializeField] private float shortJumpDelta = 2.5f; //Multiplicador pra uma "gravidade" extra que permite um pulo curto

        [Space(5f)]

    [Header("Configurações de Prostração"), Space(8f)]

    [SerializeField, Range(0, 1)] private float kneelHeightMultiplier = 0.5f;

    [SerializeField, Range(0, 1)] private float kneelSpeedMultiplier = 0.7f;

    [SerializeField]
    private bool
        isLookingRight,
        isGrounded = true,
        //isClimbing = false,
        jumpKeyHeld = false,
        willJump = false,
        isKneeling = false,
        inRagdoll = false,
        isStunned = false,
        isStuckInWire = false;

    [SerializeField]
    private float
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

    private Coroutine stunCoroutine;

    private BoxCollider playerCollider;

    private Rigidbody rb;

    private RaycastHit hit;

    public delegate void EventHandler();
    public event EventHandler OnPlayerTurned; //pra armadilha atiradora

    private Entrada input;

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

        input = Entrada.Instance;
        input.OnJumpButtonDown += Instance_OnJumpButtonDown;
        input.OnJumpButtonUp += Instance_OnJumpButtonUp;
        input.OnKneelButtonDown += Instance_OnKneelButtonDown;
        input.OnKneelButtonUp += Instance_OnKneelButtonUp;
    }

    private void OnDisable()
    {
        input.OnJumpButtonDown -= Instance_OnJumpButtonDown;
        input.OnJumpButtonUp -= Instance_OnJumpButtonUp;
        input.OnKneelButtonDown -= Instance_OnKneelButtonDown;
        input.OnKneelButtonUp -= Instance_OnKneelButtonUp;
    }

    private void Instance_OnKneelButtonUp() => HandleKneel(false);

    private void Instance_OnKneelButtonDown() => HandleKneel(true);

    private void Instance_OnJumpButtonUp() => jumpKeyHeld = false;

    private void Instance_OnJumpButtonDown()
    {
        StopCoroutine(JumpBuffer());
        StartCoroutine(JumpBuffer());
        jumpKeyHeld = true;
    }
    
    private void FixedUpdate()
    {
        FlipCheck();
        CheckGrounded();
        ApplyGravity();
        HandleHorizontal();
        HandleJump();

        if(clampVelocity && !rb.isKinematic) rb.velocity = Vector3.ClampMagnitude(rb.velocity, terminalVelocity);
        //Debug.Log(rb.velocity);
    }

    private void FlipCheck()
    {
        if ((input.HorizontalInput > 0 && !isLookingRight) || (input.HorizontalInput < 0 && isLookingRight))
            if (!isStuckInWire) FlipSprite();
    }

    private void FlipSprite()
    {
        isLookingRight = !isLookingRight;
        OnPlayerTurned?.Invoke();
    }

    //Physics operations
    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, -Vector3.up,out hit, raycastDistance, groundLayer);
        
        transform.parent = hit.transform;

        coyoteTimeCurrent = isGrounded ? coyoteTimeMax : coyoteTimeCurrent - Time.fixedDeltaTime;

        if (inRagdoll && isGrounded) inRagdoll = false; //Reativa a gravidade de pulo

        if (isStunned && isGrounded) EndStun(); //Reativa controle do movimento horizontal
    }

    private void ApplyGravity()
    {
        //Desativa ou ativa a gravidade do rb com os Arames
        if (isStuckInWire)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            return;
        }
        if(!rb.useGravity) rb.useGravity = true;

        if (isGrounded) return;

        //Pulando e caindo
        float gravityScale = rb.velocity.y < 0 ? fallGravityMultiplier : jumpGravityMultiplier;
        gravityForce = gravity * (gravityScale - 1f);
        rb.AddForce(gravityForce, ForceMode.Acceleration);

        if (rb.velocity.y <= 0) return;

        //Forca de armadilha
        if (inRagdoll)
        {
            float forceMultiplier = ragdollGravityMultiplier;

            if (isRagdollJumpUnlocked)
                forceMultiplier *= isKneeling ? ragdollKneelGravityMultiplier :
                                  jumpKeyHeld ? ragdollJumpGravityMultiplier  : 1f;

            rb.AddForce( forceMultiplier * gravity, ForceMode.Acceleration );
            return;
        }

        //Pulo curto
        if (!jumpKeyHeld)
            rb.AddForce(fallGravityMultiplier * shortJumpDelta * gravity, ForceMode.Acceleration);
        
    }

    private void HandleHorizontal()
    {
        if (allowRagdollMomentum && isStunned || isStuckInWire) return;

        float speed = isKneeling ? moveSpeed * kneelSpeedMultiplier : moveSpeed;
        moveForce.Set(input.HorizontalInput * speed - rb.velocity.x, 0, 0);

        //Instantanea
        if (airControl) rb.AddForce(moveForce, ForceMode.VelocityChange);

        //Gradual
        else rb.AddForce 
        (
            isGrounded ? moveForce : moveForce * airAccelerationMultiplier, 
            isGrounded ? ForceMode.VelocityChange : ForceMode.Acceleration
        );
    }

    private void HandleJump()
    {
        if (!willJump || isKneeling || isStuckInWire || coyoteTimeCurrent <= 0) return;
     
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        coyoteTimeCurrent = 0f;
    }

    private IEnumerator JumpBuffer()
    {
        jumpBufferTimeCurrent = jumpBufferTimeMax;
        while (jumpBufferTimeCurrent > 0)
        {
            willJump = true;
            jumpBufferTimeCurrent -= Time.deltaTime;
            yield return null;
        }
        willJump = false;
    }

    private void HandleKneel(bool willKneel)
    {
        playerCollider.size = willKneel ? colliderKneelingSize : colliderBaseSize;
        playerCollider.center = willKneel ? colliderKneelingCenter : colliderBaseCenter;
        isKneeling = willKneel;

        if (isStuckInWire) SetWiredState(false, isLookingRight);
    }

    //Public methods
    public void ApplyForce(Vector3 force, ForceMode forceMode) => rb.AddForce(force, forceMode);

    public void Ragdoll(float stunTimeSecs)
    {
        inRagdoll = true;

        stunTimeRemaining = Mathf.Clamp(stunTimeRemaining + stunTimeSecs, 0, maxStunTime);

        stunCoroutine ??= StartCoroutine(Stun());
    }

    private IEnumerator Stun()
    {
        isStunned = true;
        
        while(stunTimeRemaining > 0)
        {
            stunTimeRemaining -= Time.deltaTime;
            yield return null;
        }
        EndStun();
    }

    private void EndStun()
    {
        stunTimeRemaining = 0;
        isStunned = false;
        stunCoroutine = null;
    }

    public void EnterWarp(Vector3 position)
    {
        rb.velocity = Vector3.zero;

        rb.isKinematic = true;

        position.Set(position.x, position.y, rb.position.z);
        rb.position = position;

        StartCoroutine(ExitWarp());
    }

    private IEnumerator ExitWarp()
    {
        yield return new WaitForFixedUpdate();
        rb.isKinematic = false;
    }

    public void SetWiredState(bool isWired, bool lookingRight)
    {
        isStuckInWire = isWired;
        if (lookingRight != isLookingRight) FlipSprite();
    }
}