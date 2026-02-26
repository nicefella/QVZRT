using UnityEngine;

/// <summary>
/// Lightweight third-person follow camera.
/// No Cinemachine dependency — works out of the box.
///
/// Attach to the Main Camera in GameWorld.
/// The target is auto-found by "Player" tag if not set in the Inspector.
/// </summary>
public class SimpleFollowCamera : MonoBehaviour
{
    [Tooltip("The transform to follow — auto-found if left empty")]
    public Transform target;

    [Header("Offset from target")]
    public float distance    = 8f;
    public float height      = 5f;

    [Header("Smoothing")]
    public float positionSmoothing = 6f;
    public float lookAtHeight      = 1.2f; // look at this height above target

    void LateUpdate()
    {
        if (target == null)
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null) target = playerGO.transform;
            else return;
        }

        // Desired position: directly behind and above the player
        Vector3 desired = target.position + new Vector3(0f, height, -distance);
        transform.position = Vector3.Lerp(transform.position, desired, positionSmoothing * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * lookAtHeight);
    }
}
