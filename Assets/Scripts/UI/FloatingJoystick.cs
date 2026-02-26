using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A floating virtual joystick that appears where the player first touches
/// the screen and follows their drag within a clamped radius.
///
/// Scene setup:
///   JoystickCanvas (Canvas, Screen Space Overlay)
///     JoystickArea  (full-screen transparent Image, has this script)
///       Background  (RectTransform — the outer circle graphic)
///         Handle    (RectTransform — the inner circle graphic)
///
/// Wire Background → background field, Handle → handle field.
/// Read Horizontal and Vertical in PlayerController.
/// </summary>
public class FloatingJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [Tooltip("Handle travel as a fraction of the background radius (1 = edge).")]
    [SerializeField] private float handleRange = 1f;

    /// <summary>Normalised horizontal axis (-1 … 1).</summary>
    public float Horizontal { get; private set; }

    /// <summary>Normalised vertical axis (-1 … 1).</summary>
    public float Vertical { get; private set; }

    private Canvas _canvas;
    private Camera _cam;

    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        _cam    = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
        background.gameObject.SetActive(false);
    }

    // -----------------------------------------------------------------------
    // IPointer / IDrag handlers
    // -----------------------------------------------------------------------

    public void OnPointerDown(PointerEventData data)
    {
        background.anchoredPosition = ScreenToAnchoredPosition(data.position);
        background.gameObject.SetActive(true);
        handle.anchoredPosition = Vector2.zero;
        OnDrag(data);
    }

    public void OnDrag(PointerEventData data)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, data.position, _cam, out Vector2 localPos);

        float radius = background.sizeDelta.x * 0.5f * handleRange;
        Vector2 clamped = Vector2.ClampMagnitude(localPos, radius);

        handle.anchoredPosition = clamped;

        Horizontal = clamped.x / radius;
        Vertical   = clamped.y / radius;
    }

    public void OnPointerUp(PointerEventData data)
    {
        background.gameObject.SetActive(false);
        handle.anchoredPosition = Vector2.zero;
        Horizontal = 0f;
        Vertical   = 0f;
    }

    // -----------------------------------------------------------------------
    // Helper
    // -----------------------------------------------------------------------

    private Vector2 ScreenToAnchoredPosition(Vector2 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)background.parent, screenPos, _cam, out Vector2 localPos);
        return localPos;
    }
}
