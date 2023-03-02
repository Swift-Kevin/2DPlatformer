using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class movement : MonoBehaviour
{
    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    public Vector2 direction;
    
    [Header("Vertical Movement")]
    public float jumpSpeed = 15f;
    public float jumpDelay = 0.25f;
    private float jumpTimer;

    [Header("Components")]
    public Rigidbody2D rb;
    private SpriteRenderer _renderer;
    public SpriteRenderer _bensonsBill;
    public LayerMask groundLayer;
    public GameObject characterHolder;

    [Header("Physics")]
    public float maxSpeed = 7f;
    public float linearDrag = 4f;
    public float gravity = 1f;
    public float fallMultiplier = 5f;

    [Header("Collision")]
    public bool onGround = false;
    public float groundLength = 0.6f;
    public Vector3 colliderOffset;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer == null)
        {
            Debug.LogError("Player Sprite is missing a renderer");
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool wasOnGround = onGround;
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);

        if (Gamepad.current.buttonSouth.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpTimer = Time.time + jumpDelay;
        }
        direction = new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), Gamepad.current.buttonSouth.ReadValue());
    }
    void FixedUpdate()
    {
        moveCharacter(direction.x);
        if (jumpTimer > Time.time && onGround)
        {
            Jump();
        }
        modifyPhysics();
    }
    void moveCharacter(float horizontal)
    {
        rb.AddForce(Vector2.right * horizontal * moveSpeed);

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

        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
    }
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        jumpTimer = 0;
    }
    void modifyPhysics()
    {

        //bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        if (onGround)
        {
            if (Mathf.Abs(direction.x) < 0.4f/* || changingDirections*/)
            {
                rb.drag = linearDrag;
            }
            else
            {
                rb.drag = 0f;
            }
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = gravity;
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            else if (rb.velocity.y > 0 && Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
    }

}
