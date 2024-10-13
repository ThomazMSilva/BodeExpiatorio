using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovimentoJogador : MonoBehaviour
{
    [Header("Configurações de Movimento"), Space(8f)]

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float climbSpeed = 5f;

    [Tooltip("Se ativo, a velocidade maxima que o Jogador pode se mover tem um limite.")]
    [SerializeField] private bool clampVelocity = true;

    [Tooltip("A velocidade maxima, em unidades/segundo, que o Jogador podera se mover pra qualquer direcao.")]
    [SerializeField] private float terminalVelocity = 30f;
    
    [Space(7f)]
 
    [Tooltip("Se ativo, o Jogador pode mudar de direcao no ar imediatamente com o Input Horizontal.\nSe inativo, ele vai acelerar gradualmente para a direcao do input.")]
    [SerializeField] private bool instantAirVelocityChange = true;
    
    [Tooltip("So usar caso Instant Velocity Change esteja INATIVO. \nA aceleracao com que ele muda de direcao no ar")]
    [SerializeField] private float airAccelerationMultiplier = 3;
    
    [Space(7f)]

    [Tooltip("Se ativo, o Jogador pode mudar de direcao no chao imediatamente com o Input Horizontal.\nSe inativo, ele vai acelerar gradualmente para a direcao do input.")]
    [SerializeField] private bool instantGroundVelocityChange = true;

    [Tooltip("So usar caso Instant Ground Change esteja INATIVO. \nA aceleracao com que ele muda de direcao no chao")]
    [SerializeField] private float groundAccelerationMultiplier = 12f;

    [Space(7f)]

    [Tooltip("Se ATIVO, o Jogador não tem nenhum controle de sua velocidade horizontal enquanto estiver em ragdoll.")]
    [SerializeField] private bool allowRagdollMomentum = false;

    [SerializeField] private float maxStunTime = 4f;

    [Space(5f)]

    [Header("Configurações de Gravidade"), Space(8f)]

    [SerializeField] private float fallGravityMultiplier = 5f;

    [SerializeField] private float jumpGravityMultiplier = 3f;

    [Space(8f)]

    [SerializeField] private float ragdollGravityMultiplier = 6f;

    [SerializeField] private float ragdollJumpGravityMultiplier = 0.5f;

    [SerializeField] private float ragdollKneelGravityMultiplier = 2f;

    [Tooltip("Se ativo, o Jogador pode controlar a altura que e lancado pelas armadilhas com seu pulo e prostracao.")]
    public bool isRagdollJumpUnlocked = true;

    [Space(8f)]

    [SerializeField] private float raycastDistance = .75f;

    [SerializeField] LayerMask groundLayer;
    
    [SerializeField] string fallingPlatformTag = "Marvada";

    [Space(5f)]

    [Header("Configurações de Pulo"), Space(8f)]

    [SerializeField] private float jumpForce = 11f;

    [SerializeField] private float coyoteTimeMax = 0.1f;

    [SerializeField] private float jumpBufferTimeMax = 0.2f;

    [SerializeField] private float shortJumpDelta = 2.5f;

    [Space(5f)]

    [Header("Configurações de Prostração"), Space(8f)]

    [SerializeField, Range(0, 1)] private float kneelHeightMultiplier = 0.5f;

    [SerializeField, Range(0, 1)] private float kneelSpeedMultiplier = 0.7f;

    [SerializeField]
    private bool
        isLookingRight,
        isGrounded = true,
        isClimbing = false,
        jumpKeyHeld = false,
        willJump = false,
        isKneeling = false,
        inRagdoll = false,
        isStunned = false,
        isStuckInWire = false;

    public bool IsClimbing { get => isClimbing; private set => isClimbing = value; }

    //[SerializeField]
    private float
        horizontalInput,
        jumpBufferTimeCurrent,
        coyoteTimeCurrent,
        stunTimeRemaining,
        wiredRagdollTime = 0;

    private Vector3
        gravity,
        colliderBaseSize,
        colliderKneelingSize,
        colliderBaseCenter,
        colliderKneelingCenter,
        moveForce = Vector3.zero,
        climbForce = Vector3.zero,
        gravityForce = Vector3.zero,
        currentWireForce = new(1, 1, 0);

    private Coroutine stunCoroutine;

    private SpriteRenderer playerSprite;

    [SerializeField] private BoxCollider playerCollider;

    [SerializeField] private Rigidbody rb;

    private RaycastHit hit;

    public delegate void EventHandler(bool boolean);
    public event EventHandler OnPlayerTurned;

    [Space(8f)]
    [SerializeField] private Entrada input;

    private void Start()
    {
        //rb = GetComponent<Rigidbody>();

        TryGetComponent<SpriteRenderer>(out playerSprite);

        //playerCollider = GetComponent<BoxCollider>();

        colliderBaseSize = playerCollider.size;
        colliderKneelingSize.Set(colliderBaseSize.x, colliderBaseSize.y * kneelHeightMultiplier, colliderBaseSize.z);

        colliderBaseCenter = playerCollider.center;
        colliderKneelingCenter.Set(colliderBaseCenter.x, colliderBaseCenter.y - (colliderKneelingSize.y * 0.5f), colliderBaseCenter.z);
        raycastDistance = playerCollider.bounds.extents.y + .25f;

        gravity = Physics.gravity;
    }
    
    private void OnEnable()
    {
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
        //FlipCheck();
        CheckGrounded();
        ApplyGravity();
        HandleHorizontal();
        HandleClimb();
        HandleJump();

        if (clampVelocity && !rb.isKinematic) rb.velocity = Vector3.ClampMagnitude(rb.velocity, terminalVelocity);

    }

    private void FlipSprite()
    {
        isLookingRight = !isLookingRight;
        if (playerSprite != null) playerSprite.flipX = !isLookingRight;

        OnPlayerTurned?.Invoke(isLookingRight);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, out hit, raycastDistance, groundLayer);
        
        transform.parent = hit.transform && !hit.transform.CompareTag(fallingPlatformTag) ? hit.transform : null;
        
        coyoteTimeCurrent = isGrounded || isStuckInWire || isClimbing ? coyoteTimeMax : coyoteTimeCurrent - Time.fixedDeltaTime;

        if (inRagdoll && isGrounded) inRagdoll = false;

        if (isStunned && isGrounded) EndStun();

    }

    private void ApplyGravity()
    {

        if (isStuckInWire || isClimbing)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            return;
        }
        if (!rb.useGravity) rb.useGravity = true;

        if (isGrounded) return;

        float notJumpingGravityMultiplier = isKneeling ? fallGravityMultiplier * shortJumpDelta : fallGravityMultiplier;
        float gravityScale = rb.velocity.y < 0 ? notJumpingGravityMultiplier : jumpGravityMultiplier;
        gravityForce = gravity * (gravityScale - 1f);
        rb.AddForce(gravityForce, ForceMode.Acceleration);

        if (rb.velocity.y <= 0) return;


        if (inRagdoll)
        {
            float forceMultiplier = ragdollGravityMultiplier;

            if (isRagdollJumpUnlocked)
                forceMultiplier *= isKneeling ? ragdollKneelGravityMultiplier :
                                  jumpKeyHeld ? ragdollJumpGravityMultiplier : 1f;

            rb.AddForce(forceMultiplier * gravity, ForceMode.Acceleration);
            return;
        }


        if (!jumpKeyHeld)
            rb.AddForce(fallGravityMultiplier * shortJumpDelta * gravity, ForceMode.Acceleration);

    }

    private void HandleHorizontal()
    {
        if (allowRagdollMomentum && isStunned || isStuckInWire) return;

        float speed = isKneeling ? moveSpeed * kneelSpeedMultiplier : moveSpeed;
        horizontalInput = input.HorizontalInput;
        moveForce.Set(horizontalInput * speed - rb.velocity.x, 0, 0);
        
        bool wantToFlip = (isLookingRight ? horizontalInput < 0 : horizontalInput > 0) && !isStuckInWire;
        if (wantToFlip) FlipSprite();

        Vector3 appliedForce = 
            moveForce * (
                            (isGrounded && !instantGroundVelocityChange) 
                            ? groundAccelerationMultiplier
                            : (!isGrounded && !instantAirVelocityChange) 
                                ? airAccelerationMultiplier
                                : 1f
                        );

        ForceMode mode = (isGrounded && instantGroundVelocityChange) || (!isGrounded && instantAirVelocityChange)
                            ? ForceMode.VelocityChange
                            : ForceMode.Acceleration;

        rb.AddForce(appliedForce, mode);
    
    }

    private void HandleClimb()
    {
        if (!isClimbing) return;
     
        climbForce.y = input.VerticalInput * climbSpeed;
        rb.AddForce(climbForce, ForceMode.VelocityChange);
    }

    private void HandleJump()
    {
        if (!willJump || isKneeling || coyoteTimeCurrent <= 0) return;
        AudioManager.Instance.PlayerOneShot(FMODEvents.Instance.PlayerJumped, transform.position);

        if (isStuckInWire)
        {
            rb.AddForce(currentWireForce, ForceMode.Impulse);
            Ragdoll(wiredRagdollTime);
            return;
        }
        if (isClimbing) SetPlayerClimbing(false);

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
        if (isStuckInWire) { SetWiredState(false, isLookingRight); return; }
        if (isClimbing) { SetPlayerClimbing(false); return; }

        playerCollider.size = willKneel ? colliderKneelingSize : colliderBaseSize;
        playerCollider.center = willKneel ? colliderKneelingCenter : colliderBaseCenter;
        isKneeling = willKneel;

    }

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

        while (stunTimeRemaining > 0)
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
        transform.position = position;

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

    public void SetPlayerClimbing(bool climbing)
    {
        isClimbing = climbing;
    }

    public void SetWiredForce(float forceMultiplerX, float forceMultiplierY, float ragdollTime)
    {
        currentWireForce.Set((isLookingRight ? 1 : -1) * forceMultiplerX, 1 * forceMultiplierY, 0);
        wiredRagdollTime = ragdollTime;
    }

}