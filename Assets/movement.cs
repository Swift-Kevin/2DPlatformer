using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class movement : MonoBehaviour
{

    PlayerMovement playerMovement;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private SpriteRenderer _bensonsBill;
    [SerializeField] private SpriteRenderer _bensonWing;
    [SerializeField] public Stamina stamina;
    [SerializeField] private float glidePowerUsed = 10f;
    [SerializeField] private float groundLength = 0.6f;
    [SerializeField] private float maxSpeed = 7.16f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 colliderOffset;
    
    Vector3 contactNormal;
    private Vector2 playerInput;
    private Vector3 velocity, desiredVelocity;
    private Rigidbody2D body;
    private bool onGround;
    private float normalGrav;
    private bool desiredJump;

    [Header("Sliders")]
    [SerializeField, Range(0f, 100f)] float maxAcceleration = 7.16f * 3f;
    [SerializeField, Range(0f, 100f)] float maxAirAcceleration = 1f;
    [SerializeField, Range(0f, 10f)] float jumpHeight = 5f;
    [SerializeField, Range(0f, 90f)] float maxGroundAngle = 25f;
    float minGroundDotProduct;

    private void Start()
    {
        playerMovement = new PlayerMovement();
        playerMovement.PlayerMoving.Enable();

        _bensonWing.enabled = false;

        normalGrav = rb.gravityScale;
    }

    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        OnValidate();
    }

    private void Update()
    {
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
        
        if (onGround && !stamina.IsGPMaxed())
        {
            stamina.RegenGP();
            rb.gravityScale = normalGrav;
            _bensonWing.enabled = false;
        }

        if (playerMovement.PlayerMoving.Glide.WasPressedThisFrame() && !onGround)
        {
            stamina.UseGP(glidePowerUsed);
            rb.gravityScale = normalGrav / 2f;
            _bensonWing.enabled = true;
        }

        UpdateSprite();
    }

    private void FixedUpdate()
    {
        playerInput = playerMovement.PlayerMoving.Movement.ReadValue<Vector2>();
        playerInput.y = 0f;
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        desiredJump |= playerMovement.PlayerMoving.Jump.IsPressed();

        UpdateState();
        AdjustVelocity();
        

        if (desiredJump && onGround)
        {
            desiredJump = false;
            Jump();
        }
        body.velocity = velocity;
    }
    void UpdateState()
    {
        velocity = body.velocity;
    }

    void Jump()
    {
        float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        float alignedSpeed = Vector3.Dot(velocity, contactNormal);
        if (alignedSpeed > 0f)
        {
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
        }
        velocity += contactNormal * jumpSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct)
            {
                onGround = true;
                contactNormal = normal;
            }
            else
            {
                contactNormal = Vector3.up;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            onGround |= normal.y >= 0.9f;
        }
    }
    Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    void AdjustVelocity()
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;
        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);
        float acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX =
            Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ =
            Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);

    }

    void UpdateSprite()
    {
        #region FlipPlayer
        if (UnityEngine.Input.GetAxisRaw("Horizontal") > 0)
        {
            _renderer.flipX = false;
            _bensonsBill.flipX = false;
            _bensonWing.flipX = false;
        }
        else if (UnityEngine.Input.GetAxisRaw("Horizontal") < 0)
        {
            _renderer.flipX = true;
            _bensonsBill.flipX = true;
            _bensonWing.flipX = true;
        }
        #endregion

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
    }

    public float GetGlidePowerUsed()
    {
        return glidePowerUsed;
    }
}