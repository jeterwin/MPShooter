using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement variables")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jumping variables")]
    [SerializeField] private float jumpPower;

    [Header("Grounded variables")]
    [SerializeField] private GameObject groundCheckPoint;
    [SerializeField] private float rayRange = 1f;
    [SerializeField] private LayerMask groundMask;

    private InputHandler inputHandler;
    private Vector3 movement;

    bool isGrounded
    {
        get { return Physics.Raycast(groundCheckPoint.transform.position, Vector3.down, rayRange, groundMask); }
    }
    private void OnEnable()
    {
        Transform newTransform = SpawnManager.Instance.GetSpawnPoint();
        transform.position = newTransform.position;
        transform.rotation = newTransform.rotation;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheckPoint.transform.position, groundCheckPoint.transform.position + Vector3.down * rayRange);
    }
    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
    }

    private void Update()
    {
        HandleMoving();
    }

    private void HandleJumping()
    {
        if (isGrounded)
        {
            movement.y = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            movement.y = jumpPower;
        }
        movement.y += Physics.gravity.y * Time.deltaTime;
    }
    private void HandleMoving()
    {
        float yVel = movement.y;
        movement = (inputHandler.MovementDir.y * transform.forward + inputHandler.MovementDir.x * transform.right).normalized;
        movement.y = yVel;

        HandleJumping();

        characterController.Move(Time.deltaTime * moveSpeed * movement);
    }
}
