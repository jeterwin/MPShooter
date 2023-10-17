using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Movement variables")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Animator animator;

    [Header("Jumping variables")]
    [SerializeField] private float jumpPower;

    [Header("Grounded variables")]
    [SerializeField] private GameObject groundCheckPoint;
    [SerializeField] private float rayRange = 1f;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private GameObject playerModel;
    [SerializeField] private Transform gunHolder;
    [SerializeField] private Transform modelGunPoint;

    private InputHandler inputHandler;
    private Vector3 movement;

    bool isGrounded
    {
        get { return Physics.Raycast(groundCheckPoint.transform.position, Vector3.down, rayRange, groundMask); }
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
    private void Start()
    {
        if(photonView.IsMine)
        {
            playerModel.SetActive(false);
        }
        else
        {
            gunHolder.parent = modelGunPoint;
            gunHolder.localPosition = Vector3.zero;
            gunHolder.localRotation = Quaternion.identity;
        }
    }
    private void Update()
    {
       if(photonView.IsMine)
            HandleMoving();
    }

    private void HandleJumping()
    {
        if (isGrounded && movement.y != 0)
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

        animator.SetBool("grounded", isGrounded);
        animator.SetFloat("speed", Mathf.Abs(movement.x));

        characterController.Move(Time.deltaTime * moveSpeed * movement);
    }
}
