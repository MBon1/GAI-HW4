using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleFormationPattern : FormationPattern
{
    public void GetDriftOffset(
        List<GameObject> slotAssignments,
        ref Vector2 driftOffsetPosition,
        ref float driftOffsetOrientation)
    {
        Vector2 centerPosition = Vector2.zero;
        float centerOrientation = 0.0f;

        for (int i = 0; i < slotAssignments.Count; i++) {
            Vector3 location = GetSlotLocation(i);

            centerPosition.x += location.x;
            centerPosition.y += location.y;

            centerOrientation += location.z;
        }

        // This could probably be replaced by this.numberOfSlots, right?
        int numberOfAssignments = slotAssignments.Count;

        centerPosition *= (1 / numberOfAssignments);
        centerOrientation *= (1 / numberOfAssignments);

        driftOffsetPosition = centerPosition;
        driftOffsetOrientation = centerOrientation;
    }

    public Vector3 GetSlotLocation(int slotNumber)
    {
        // Placeholder for the time being based on the character
        float characterRadius = 0.5f;

        int n = this.numberOfSlots;
        
        float angleAroundCircle = slotNumber / n * 360;
        float radius = Mathf.Sin(Mathf.PI / n);

        Vector3 location = Vector3.zero;

        // Use x and y as position
        location.x = radius * Mathf.Cos(angleAroundCircle);
        location.y = radius * Mathf.Sin(angleAroundCircle);

        // Use z as orientation
        location.z = angleAroundCircle;

        return location;
    }

    bool SupportsSlots(int slotCount) {
        // A circle formation allows for any number of slots
        return true;
    }
}
