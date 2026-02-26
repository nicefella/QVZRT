using System.Collections;
using UnityEngine;

/// <summary>
/// Handles entering and exiting a building via trigger colliders.
///
/// Each building prefab has TWO BuildingEntrance components:
///
///   1. Door trigger (outside)  — isInsideTrigger = false
///        Refs: exteriorRoot, interiorRoot, interiorSpawnPoint, exteriorSpawnPoint
///
///   2. Exit trigger (inside)   — isInsideTrigger = true
///        Same refs as above (can share references by pointing to the same objects)
///
/// Both triggers share the same exteriorRoot / interiorRoot so toggling
/// works regardless of which side the player walks through.
///
/// Tag the Player GameObject with the "Player" tag.
///
/// Prefab hierarchy example:
///
///   School (empty root)
///     Exterior  (exteriorRoot)
///       BuildingMesh
///       DoorTrigger  ← BoxCollider isTrigger, BuildingEntrance (isInsideTrigger=false)
///     Interior  (interiorRoot, starts disabled)
///       RoomMesh
///       Props...
///       ExitTrigger  ← BoxCollider isTrigger, BuildingEntrance (isInsideTrigger=true)
///     InteriorSpawn  (empty Transform — where player appears after entering)
///     ExteriorSpawn  (empty Transform — where player appears after exiting)
/// </summary>
public class BuildingEntrance : MonoBehaviour
{
    [Header("Building Roots")]
    [SerializeField] private GameObject exteriorRoot;
    [SerializeField] private GameObject interiorRoot;

    [Header("Spawn Points")]
    [SerializeField] private Transform interiorSpawnPoint;
    [SerializeField] private Transform exteriorSpawnPoint;

    [Header("References")]
    [SerializeField] private FadeScreen  fadeScreen;
    [SerializeField] private UIManager   uiManager;

    [Header("Settings")]
    [Tooltip("Set true on the exit trigger inside the building; false on the entrance door outside.")]
    [SerializeField] private bool isInsideTrigger = false;

    // -----------------------------------------------------------------------
    // Private state
    // -----------------------------------------------------------------------

    private PlayerController _player;
    private bool             _playerNearby  = false;
    private bool             _transitioning = false;

    // -----------------------------------------------------------------------
    // Trigger events
    // -----------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _player       = other.GetComponent<PlayerController>();
        _playerNearby = true;

        string prompt = isInsideTrigger ? "Tap to Exit" : "Tap to Enter";
        uiManager.ShowEnterPrompt(prompt, OnInteract);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _playerNearby = false;
        _player       = null;
        uiManager.HideEnterPrompt();
    }

    // -----------------------------------------------------------------------
    // Interaction callback
    // -----------------------------------------------------------------------

    private void OnInteract()
    {
        if (!_playerNearby || _player == null || _transitioning) return;
        StartCoroutine(Transition());
    }

    // -----------------------------------------------------------------------
    // Transition coroutine
    // -----------------------------------------------------------------------

    private IEnumerator Transition()
    {
        _transitioning = true;
        _player.LockMovement(true);
        uiManager.HideEnterPrompt();

        // Fade to black
        yield return StartCoroutine(fadeScreen.FadeOut());

        if (isInsideTrigger)
        {
            // Exiting the building
            interiorRoot.SetActive(false);
            exteriorRoot.SetActive(true);
            TeleportPlayer(exteriorSpawnPoint);
        }
        else
        {
            // Entering the building
            exteriorRoot.SetActive(false);
            interiorRoot.SetActive(true);
            TeleportPlayer(interiorSpawnPoint);
        }

        // Fade back in
        yield return StartCoroutine(fadeScreen.FadeIn());

        _player.LockMovement(false);
        _transitioning = false;

        // Player is now in a new area — reset nearby flag so the
        // trigger re-fires correctly if the player steps back
        _playerNearby = false;
        _player       = null;
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private void TeleportPlayer(Transform spawnPoint)
    {
        if (spawnPoint == null || _player == null) return;

        // CharacterController must be disabled briefly to teleport
        var cc = _player.GetComponent<CharacterController>();
        cc.enabled = false;
        _player.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        cc.enabled = true;
    }
}
