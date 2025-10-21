//This script handles all player movements methods.

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float gravity = -9.8f;
    public float jumpHight = 2f;

    [Header("Crouch Settings")]
    public float standingHeight = 2f;
    public float crouchHeight = 1f;
    public float heightLerpSpeed = 8f;

    private CharacterController characterController;
    private Vector3 playerVelocity;
    private bool isGround;
    private bool isSprinting;
    private bool isCrouching;
    private float currentSpeed;

    void Start()
    {
        characterController = GetComponent<CharacterController>();    
    }

    void Update()
    {
        isGround = characterController.isGrounded;
        ApplyGravity();
        SmoothCrouchTransition();


    }

    //recives all input from InputManager and apply to charechter controller
    public void ProcessMove(Vector2 input)
    {
        Vector3 mouseDirection = Vector3.zero;
        mouseDirection.x = input.x;
        mouseDirection.z = input.y;

        // pick correct speed
        if (isCrouching)
            currentSpeed = crouchSpeed;
        else if (isSprinting)
            currentSpeed = sprintSpeed;
        else
            currentSpeed = speed;

        characterController.Move(transform.TransformDirection(mouseDirection) * currentSpeed * Time.deltaTime);
        
        
    }

    private void ApplyGravity()
    {
        playerVelocity.y += gravity * Time.deltaTime;
        if (isGround && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        characterController.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (isGround)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHight * -3.0f * gravity);
        }
    }

    public void SetSprint(bool state)
    {
        isSprinting = state; // prevent sprint while crouched
        isCrouching = false;
    }

    public void SetCrouch(bool state)
    {
        isCrouching = state;
        if (isCrouching) isSprinting = false; // stop sprint when crouch
    }

    private void SmoothCrouchTransition()
    {
        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * heightLerpSpeed);
    }

}
