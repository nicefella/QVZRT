using UnityEngine;

/// <summary>
/// Third-person player movement for mobile.
///
/// - Reads from VirtualJoystick (auto-found by type at Start).
/// - Falls back to WASD / Arrow keys for in-editor testing.
/// - Moves relative to the camera's forward direction.
/// - No jumping needed for this prototype.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed   = 4.5f;
    public float rotateSpeed = 12f;
    public float gravity     = -20f;

    private CharacterController cc;
    private VirtualJoystick joystick;
    private Transform cameraTransform;
    private float verticalVelocity;

    void Start()
    {
        cc               = GetComponent<CharacterController>();
        joystick         = FindObjectOfType<VirtualJoystick>();
        cameraTransform  = Camera.main?.transform;
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        // ── Input ──────────────────────────────────────────────────
        Vector2 raw = joystick != null ? joystick.Direction : Vector2.zero;

        // Keyboard fallback for editor testing
        if (raw == Vector2.zero)
            raw = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // ── Gravity ────────────────────────────────────────────────
        if (cc.isGrounded)
            verticalVelocity = -1f;          // small downward force keeps isGrounded true
        else
            verticalVelocity += gravity * Time.deltaTime;

        // ── Horizontal movement ────────────────────────────────────
        if (raw.sqrMagnitude < 0.01f)
        {
            cc.Move(Vector3.up * verticalVelocity * Time.deltaTime);
            return;
        }

        Vector3 camForward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
        Vector3 camRight   = cameraTransform != null ? cameraTransform.right   : Vector3.right;
        camForward.y = 0f; camForward.Normalize();
        camRight.y   = 0f; camRight.Normalize();

        Vector3 moveDir = (camForward * raw.y + camRight * raw.x).normalized;
        Vector3 velocity = moveDir * moveSpeed + Vector3.up * verticalVelocity;

        cc.Move(velocity * Time.deltaTime);

        // ── Rotation ───────────────────────────────────────────────
        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
    }
}
