using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FormationPattern : MonoBehaviour
{
    // Holds the number of slots currently in the pattern.
    // This is updated in the getDriftOffset method.
    // It may be a fixed value.
    public int numberOfSlots;

    // Calculates the drift offset when characters are in
    // given set of slots
    public void GetDriftOffset(List<GameObject> slotAssignments, ref Vector2 driftOffsetPosition, ref float driftOffsetOrientation) { return; }

    // Gets the location of the given slot index.
    public Vector3 GetSlotLocation(int slotNumber) { return Vector3.zero; }

    // Returns true if the pattern can support the given number of slots
    bool SupportsSlots(int slotCount) { return false; }
}
