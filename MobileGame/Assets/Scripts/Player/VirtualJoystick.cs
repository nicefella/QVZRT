using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Self-contained on-screen virtual joystick.
///
/// Hierarchy expected (auto-built by SceneBuilder):
///   JoystickBackground (this script + Image)
///   └── Handle (Image)
///
/// PlayerController reads the Direction property each frame.
/// </summary>
[RequireComponent(typeof(Image))]
public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Tooltip("How far the handle can move from centre (as a fraction of background radius, 0-1)")]
    [Range(0.1f, 1f)]
    public float handleRange = 0.9f;

    [Tooltip("Input below this magnitude is treated as zero (prevents micro drift)")]
    [Range(0f, 0.3f)]
    public float deadZone = 0.05f;

    /// <summary>Normalised direction vector. Zero when joystick is released.</summary>
    public Vector2 Direction { get; private set; }

    private RectTransform backgroundRect;
    private RectTransform handleRect;

    void Start()
    {
        backgroundRect = GetComponent<RectTransform>();
        var handleGO = transform.Find("Handle");
        if (handleGO != null) handleRect = handleGO.GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData) => UpdateHandle(eventData);

    public void OnDrag(PointerEventData eventData) => UpdateHandle(eventData);

    public void OnPointerUp(PointerEventData eventData)
    {
        Direction = Vector2.zero;
        if (handleRect != null) handleRect.anchoredPosition = Vector2.zero;
    }

    void UpdateHandle(PointerEventData eventData)
    {
        if (backgroundRect == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            backgroundRect, eventData.position, eventData.pressEventCamera, out Vector2 localPos);

        // Normalise to -1..1 range
        Vector2 normalised = new Vector2(
            localPos.x / (backgroundRect.sizeDelta.x * 0.5f),
            localPos.y / (backgroundRect.sizeDelta.y * 0.5f));

        normalised = Vector2.ClampMagnitude(normalised, 1f);

        Direction = (normalised.magnitude > deadZone) ? normalised : Vector2.zero;

        if (handleRect != null)
        {
            handleRect.anchoredPosition = new Vector2(
                Direction.x * backgroundRect.sizeDelta.x * 0.5f * handleRange,
                Direction.y * backgroundRect.sizeDelta.y * 0.5f * handleRange);
        }
    }
}
