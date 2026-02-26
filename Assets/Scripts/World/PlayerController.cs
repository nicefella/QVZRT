using UnityEngine;

/// <summary>
/// Third-person player controller driven by a FloatingJoystick.
/// Uses Unity's CharacterController — no Rigidbody needed.
///
/// Required components on the same GameObject:
///   - CharacterController
///   - AvatarCustomizer  (optional — applies saved avatar data on Start)
///
/// Inspector wiring:
///   joystick        → the FloatingJoystick component in the HUD canvas
///   cameraTransform → leave empty to auto-use Camera.main.transform
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed     = 5f;
    [SerializeField] private float rotationSpeed = 720f;  // degrees/sec

    [Header("References")]
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private Transform        cameraTransform;

    // -----------------------------------------------------------------------
    // Private state
    // -----------------------------------------------------------------------

    private CharacterController _cc;
    private AvatarCustomizer    _customizer;
    private bool                _movementLocked = false;
    private float               _verticalVelocity = 0f;

    // -----------------------------------------------------------------------
    // Unity lifecycle
    // -----------------------------------------------------------------------

    private void Awake()
    {
        _cc          = GetComponent<CharacterController>();
        _customizer  = GetComponent<AvatarCustomizer>();
    }

    private void Start()
    {
        // Apply the avatar appearance saved in GameManager
        if (_customizer != null && GameManager.Instance != null)
            _customizer.Apply(GameManager.Instance.AvatarData);

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        ApplyGravity();

        if (_movementLocked) return;

        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        if (h * h + v * v < 0.01f) return;   // dead zone

        // Project camera axes onto the horizontal plane
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight   = cameraTransform.right;
        camForward.y = 0f;
        camRight.y   = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        // Horizontal movement + accumulated vertical velocity
        Vector3 move = moveDir * moveSpeed;
        move.y = _verticalVelocity;

        _cc.Move(move * Time.deltaTime);

        // Rotate avatar to face movement direction
        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }

    // -----------------------------------------------------------------------
    // Public API — called by BuildingEntrance during transitions
    // -----------------------------------------------------------------------

    /// <summary>
    /// Prevent player movement during fade / teleport transitions.
    /// </summary>
    public void LockMovement(bool locked)
    {
        _movementLocked = locked;
    }

    // -----------------------------------------------------------------------
    // Private helpers
    // -----------------------------------------------------------------------

    private void ApplyGravity()
    {
        if (_cc.isGrounded)
        {
            _verticalVelocity = -0.5f;  // small downward force to stay grounded
        }
        else
        {
            _verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
    }
}
