using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using UnityEngine.UI;
using UnityEditor.Profiling.Memory.Experimental;

public class movement : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] private float walkingSpeed = 7.16f;
    [SerializeField] private float gravityMultiplier = 5f;
    [SerializeField] public float jumpForce = 10f;
    [SerializeField, Range(0f, 100f)] float maxAcceleration = 10f;
    [SerializeField, Range(0f, 1f)] float bounciness = 0.5f;

    private bool isOnGround;

    [Header("Components")]
    public SpriteRenderer _renderer;
    public SpriteRenderer _bensonsBill;
    public Rigidbody2D rigidBody2D;

    [Header("Jumping Check Components")]
    public float groundLength = 0.6f;
    public LayerMask groundLayer;

    [Header("Vectors/Gamepads")]
    Vector2 playerInput;
    public Vector3 colliderOffset;
    Vector3 velocity;
    Gamepad gamepad;

    public Stamina stamina;
    [SerializeField] private int staminaToUse = 5;
    [SerializeField] private float downwardGravity = 40f;
    [SerializeField] private float normalGravity = 10f;

    private void Start()
    {
        gamepad = Gamepad.current;
        if (gamepad != null) 
        { 
            return;
        }
    }
    private void Update()
    {
        isOnGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);

        playerInput.x = UnityEngine.Input.GetAxis("Horizontal");
        playerInput.y = 0f;

        Jump();
        
        playerInput.Normalize();

        Vector3 accel = new Vector3(playerInput.x, 0f, playerInput.y) * walkingSpeed;
        Vector3 desiredVelocity =new Vector3(playerInput.x, 0f, playerInput.y) * walkingSpeed;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        if (playerInput.x > 0)
        {
            _renderer.flipX = false;
            _bensonsBill.flipX = false;
        }
        else if (playerInput.x < 0)
        {
            _renderer.flipX = true;
            _bensonsBill.flipX = true;
        }

        Vector3 displacement = accel * Time.deltaTime;
        transform.localPosition += displacement;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
    }

    private void Jump()
    {
        if ((UnityEngine.Input.GetKeyDown(KeyCode.Space) || gamepad.aButton.wasPressedThisFrame) && isOnGround == true)
        {
            rigidBody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isOnGround = !isOnGround;
        }
        else if (rigidBody2D.velocity.y < 0)
        {
            Glide();
        }
    }

    private void Glide()
    {
        if (gamepad.buttonWest.wasPressedThisFrame && isOnGround == false)
        {
            stamina.UseStamina(staminaToUse);
            if (rigidBody2D.velocity.y < 0)
            {
                rigidBody2D.gravityScale = downwardGravity;
            }
            else
            {
                rigidBody2D.gravityScale = normalGravity;
            }
        }
    }
}
