using UnityEngine;

/// <summary>
/// Placed on the DoorTrigger (exterior) and ExitTrigger (interior) child GameObjects.
/// Forwards player-enter/exit events up to the parent BuildingEntrance.
/// </summary>
[RequireComponent(typeof(Collider))]
public class BuildingTrigger : MonoBehaviour
{
    [HideInInspector] public BuildingEntrance entrance;
    [HideInInspector] public bool isExitTrigger;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (entrance == null) return;

        if (isExitTrigger)
            entrance.OnPlayerNearExit(other.gameObject, true);
        else
            entrance.OnPlayerNearEntrance(other.gameObject, true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (entrance == null) return;

        if (isExitTrigger)
            entrance.OnPlayerNearExit(other.gameObject, false);
        else
            entrance.OnPlayerNearEntrance(other.gameObject, false);
    }
}
