//This script will handle all inputs....

using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.PlayerActions player;

    private PlayerController playerController;
    private PlayerLook look;
    void Awake()
    {
        playerInput = new PlayerInput();
        player = playerInput.Player;
        playerController = GetComponent<PlayerController>();
        look = GetComponent<PlayerLook>();

        player.Jump.performed += ctx => playerController.Jump();

        // Sprint
        player.Sprint.performed += _ => playerController.SetSprint(true);
        player.Sprint.canceled += _ => playerController.SetSprint(false);

        // Crouch
        player.Crouch.performed += _ => playerController.SetCrouch(true);
        player.Crouch.canceled += _ => playerController.SetCrouch(false);
    }

    
    void FixedUpdate()
    {
        playerController.ProcessMove(player.Move.ReadValue<Vector2>());
    }

    void LateUpdate()
    {
        look.ProcessLook(player.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        player.Enable();
    }

    private void OnDisable()
    {
        player.Disable();
    }
}
