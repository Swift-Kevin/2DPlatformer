using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class movement : MonoBehaviour
{
    Gamepad gamepad;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private SpriteRenderer _bensonsBill;
    [SerializeField] public Stamina stamina;
    [SerializeField] private float groundLength = 0.6f;
    [SerializeField] private float maxSpeed = 7.16f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 colliderOffset;
    
    Vector3 contactNormal;
    private Vector2 playerInput;
    private Vector3 velocity, desiredVelocity;
    int jumpPhase;

    private Rigidbody2D body;
    private bool onGround;
    private bool desiredJump;
    [SerializeField, Range(0f, 100f)] float maxAcceleration = 7.16f * 3f, maxAirAcceleration = 1f;
    [SerializeField, Range(0f, 10f)] float jumpHeight = 5f;
    [SerializeField, Range(0f, 90f)] float maxGroundAngle = 25f;
    float minGroundDotProduct;

    private void Start()
    {
        Gamepad gamepad = Gamepad.current;
        if (gamepad != null) { return; }
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

        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = 0f;
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        
        desiredJump |= Input.GetButtonDown("Jump") || Gamepad.current.buttonSouth.wasPressedThisFrame;


        FlipCharacter();
    }

    private void FixedUpdate()
    {
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
        if (onGround)
        {
            jumpPhase = 0;
        }
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

    void FlipCharacter()
    {
        #region FlipPlayer
        if (UnityEngine.Input.GetAxisRaw("Horizontal") > 0)
        {
            _renderer.flipX = false;
            _bensonsBill.flipX = false;
        }
        else if (UnityEngine.Input.GetAxisRaw("Horizontal") < 0)
        {
            _renderer.flipX = true;
            _bensonsBill.flipX = true;
        }
        #endregion
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
    }
}