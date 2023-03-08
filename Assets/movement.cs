using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class movement : MonoBehaviour
{
    [SerializeField] private float walkingSpeed = 7.16f;
    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    float jumpForce = 10f;

    [SerializeField] private float gravityMultiplier = 5f;

    Vector2 playerInput;
    public Rigidbody2D rigidBody2D;
    Vector3 velocity;

    private void Update()
    {
        playerInput.x = UnityEngine.Input.GetAxis("Horizontal");
        playerInput.y = 0f;

        if (UnityEngine.Input.GetKeyDown(KeyCode.Space) || UnityEngine.Input.GetButtonDown("Jump"))
        {
            rigidBody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        playerInput.Normalize();

        Vector3 accel = new Vector3(playerInput.x, 0f, playerInput.y) * walkingSpeed;

        Vector3 desiredVelocity =
            new Vector3(playerInput.x, 0f, playerInput.y) * walkingSpeed;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);


        Vector3 displacement = accel * Time.deltaTime;
        transform.localPosition += displacement;
    }
}
