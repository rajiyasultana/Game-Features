using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    private Gun gun;
    private PlayerInput inputActions;

    void Awake()
    {
        gun = GetComponent<Gun>();
        inputActions = new PlayerInput();
        inputActions.Player.Shoot.performed += ctx => Shoot();
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Shoot()
    {
        // Shoot in the direction the player is aiming (e.g., camera forward)
        Vector3 direction = Camera.main.transform.forward;
        gun.Shoot(direction);
    }
}