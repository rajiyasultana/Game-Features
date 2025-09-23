using UnityEngine;
using UnityEngine.InputSystem;
using Interface;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Character
{
    public class PlayerController : MonoBehaviour, IDamageable
    {
        [Header("Player Settings")] 
        [SerializeField] private float moveSpeed;
        [SerializeField] private float jumpForce;
        [SerializeField] private float sprintSpeed;
        [SerializeField] private float crouchYscale;
        [SerializeField] private float crouchSpeed;
        [SerializeField] private float playerHealth = 100f;

        private Camera _playerCamera;
        private Rigidbody _rb;
        private Collider _interactChecker;
        private IInteractable _currentInteractable;

        private Vector2 _inputDirection = Vector2.zero;
        private bool _isSprinting;
        private bool _isCrouching;
        private float _currentSpeed;
        private float _startYscale;

        private PlayerMovementState _lastState = PlayerMovementState.Idle;

        CancellationTokenSource _crouchCts = new CancellationTokenSource();

        // Input Actions
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference jumpAction;
        [SerializeField] private InputActionReference interactAction;
        [SerializeField] private InputActionReference sprintAction;
        [SerializeField] private InputActionReference crouchAction;
        
        public enum PlayerMovementState
        {
            Idle,
            Walk,
            Sprint,
            Crouch
        }

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Start()
        {
            // Enable input actions
            moveAction.action.Enable();
            jumpAction.action.Enable();
            interactAction.action.Enable();
            sprintAction.action.Enable();
            crouchAction.action.Enable();
            
            // Register input action callbacks
            moveAction.action.performed += ctx => _inputDirection = ctx.ReadValue<Vector2>();
            moveAction.action.canceled  += ctx => _inputDirection = Vector2.zero;
            jumpAction.action.performed += Jump;
            interactAction.action.performed += Interact;

            // Sprint Action
            sprintAction.action.performed += ctx => _isSprinting = true;
            sprintAction.action.canceled += ctx => _isSprinting = false;

            // Crouch Action
            crouchAction.action.performed += ctx => StartCrouch();
            crouchAction.action.canceled += ctx => StopCrouch();

            _playerCamera = Camera.main;
            _rb = GetComponent<Rigidbody>();
            _startYscale = transform.localScale.y;
        }

        private void OnDestroy()
        {
            jumpAction.action.performed -= Jump;
            interactAction.action.performed -= Interact;
            _crouchCts?.Cancel();
            _crouchCts?.Dispose();
        }

        public PlayerMovementState GetMovementState()
        {
            if (_inputDirection == Vector2.zero)
                return PlayerMovementState.Idle;

            if (_isSprinting)
                return PlayerMovementState.Sprint;

            if (_isCrouching)
                return PlayerMovementState.Crouch;

            return PlayerMovementState.Walk;
        }

        private void FixedUpdate()
        {
            Move();

            var currentState = GetMovementState();
            if (currentState != _lastState)
            {
                _lastState = currentState;
            }
        }

        private void Move()
        {
            if (_inputDirection == Vector2.zero) return;
            
            if (_isCrouching)
                _currentSpeed = crouchSpeed;
            else if (_isSprinting)
                _currentSpeed = sprintSpeed;
            else
                _currentSpeed = moveSpeed;

            Vector3 forward = Vector3.ProjectOnPlane(_playerCamera.transform.forward, Vector3.up).normalized;
            Vector3 right   = Vector3.ProjectOnPlane(_playerCamera.transform.right, Vector3.up).normalized;

            Vector3 moveDirection = (forward * _inputDirection.y + right * _inputDirection.x).normalized;
            _rb.MovePosition(_rb.position + moveDirection * (_currentSpeed * Time.fixedDeltaTime));
        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (GroundCheck())
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        private void StartCrouch()
        {
            _crouchCts?.Cancel();
            _crouchCts = new CancellationTokenSource();

            SmoothCrouch(crouchYscale, _crouchCts.Token).Forget();
            _isCrouching = true;
            _isSprinting = false;
        }

        private void StopCrouch()
        {
            _crouchCts?.Cancel();
            _crouchCts = new CancellationTokenSource();

            SmoothCrouch(_startYscale, _crouchCts.Token).Forget();
            _isCrouching = false;
            Debug.Log("Stop crouching..");
        }

        private async UniTask SmoothCrouch(float targetYScale, CancellationToken token)
        {
            float duration = 0.2f;
            float time = 0f;
            float startScale = transform.localScale.y;

            while (time < duration)
            {
                if (token.IsCancellationRequested) return;

                float newY = Mathf.Lerp(startScale, targetYScale, time / duration);
                transform.localScale = new Vector3(transform.localScale.x, newY, transform.localScale.z);

                time += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            transform.localScale = new Vector3(transform.localScale.x, targetYScale, transform.localScale.z);
        }

        private void Interact(InputAction.CallbackContext context)
        {
            _currentInteractable?.Interact();
        }

        private void OnTriggerEnter(Collider other)
        {
            _interactChecker = other;
            _currentInteractable = other.GetComponent<IInteractable>();
        }

        private void OnTriggerExit(Collider other)
        {
            if (_interactChecker == other)
            {
                _interactChecker = null;
                _currentInteractable = null;
            }
        }

        private bool GroundCheck()
        {
            Vector3 origin = transform.position + Vector3.up * 0.1f; 
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 0.2f))
                return hit.distance <= 0.2f;

            return false;
        }

        public void Damage()
        {
            playerHealth -= 10f;
            Debug.Log($"Player took damage! Current health: {playerHealth}");

            if (playerHealth <= 0)
                Die();
        }

        public void Die()
        {
            Debug.Log("Player has died.");
            // TODO: Add respawn or game over
        }

    }
}
