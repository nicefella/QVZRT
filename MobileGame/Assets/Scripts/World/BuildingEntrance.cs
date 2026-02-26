using UnityEngine;

/// <summary>
/// Handles entering and exiting a building.
///
/// Attach to the root of each building prefab.
/// The SceneBuilder auto-assigns all fields, but they can also be set in the Inspector.
///
/// How it works:
///   1. BuildingTrigger (on DoorTrigger child) calls OnPlayerNearEntrance().
///   2. Player presses E (keyboard) or double-taps (mobile) → Enter() is called.
///   3. UIManager fades to black, disables exterior, enables interior, moves player.
///   4. BuildingTrigger (on ExitTrigger child) calls OnPlayerNearExit().
///   5. Same interaction → Exit() reverses step 3.
/// </summary>
public class BuildingEntrance : MonoBehaviour
{
    [Header("Building Name (shown in prompt)")]
    public string buildingName = "Building";

    [Header("References (auto-filled by SceneBuilder)")]
    public GameObject exterior;
    public GameObject interior;
    public Transform  interiorSpawn;   // where player appears after entering
    public Transform  exteriorSpawn;   // where player appears after exiting

    private bool playerNearDoor;
    private bool playerNearExit;
    private bool isInside;
    private GameObject player;

    void Start()
    {
        if (interior != null) interior.SetActive(false);
    }

    void Update()
    {
        bool interact = Input.GetKeyDown(KeyCode.E) || DoubleTapDetected();

        if (!isInside && playerNearDoor && interact) Enter();
        else if (isInside && playerNearExit && interact) Exit();
    }

    // ── Called by BuildingTrigger children ──────────────────────

    public void OnPlayerNearEntrance(GameObject p, bool near)
    {
        player = p;
        playerNearDoor = near;
        if (!isInside)
            UIManager.Instance?.ShowEnterPrompt(near);
    }

    public void OnPlayerNearExit(GameObject p, bool near)
    {
        player = p;
        playerNearExit = near;
        if (isInside)
            UIManager.Instance?.ShowEnterPrompt(near);
    }

    // ── Transitions ─────────────────────────────────────────────

    public void Enter()
    {
        UIManager.Instance?.FadeAndExecute(() =>
        {
            isInside = true;
            if (exterior != null) exterior.SetActive(false);
            if (interior != null) interior.SetActive(true);
            UIManager.Instance?.ShowEnterPrompt(false);
            TeleportPlayer(interiorSpawn);
        });
    }

    public void Exit()
    {
        UIManager.Instance?.FadeAndExecute(() =>
        {
            isInside = false;
            if (interior != null) interior.SetActive(false);
            if (exterior != null) exterior.SetActive(true);
            UIManager.Instance?.ShowEnterPrompt(false);
            TeleportPlayer(exteriorSpawn);
        });
    }

    // ── Helpers ──────────────────────────────────────────────────

    void TeleportPlayer(Transform destination)
    {
        if (player == null || destination == null) return;
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.transform.position = destination.position;
        player.transform.rotation = destination.rotation;
        if (cc != null) cc.enabled = true;
    }

    // Simple double-tap detection: returns true when any touch has tapCount == 2
    static bool DoubleTapDetected()
    {
        foreach (Touch t in Input.touches)
            if (t.tapCount == 2 && t.phase == TouchPhase.Began)
                return true;
        return false;
    }
}
