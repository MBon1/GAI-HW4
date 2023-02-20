using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationManager : MonoBehaviour
{
    // Holds the assignment of a single character to a slot
    /*[System.Serializable]
    public struct SlotAssignment
    {
        GameObject character;
        int slotNumber;
    }*/

    //[SerializeField] int numSlots = 12;
    [SerializeField] int numSlots = 13;

    // Holds a list of slot assignments
    //[SerializeField] List<SlotAssignment> slotAssignments;
    [SerializeField] List<GameObject> slotAssignments;

    // Find the anchor point
    [SerializeField] GameObject anchor;

    // Hold position and orientation represeting
    // the drift offset for the currently filled slots.
    [SerializeField] Vector2 driftOffsetPosition;
    [SerializeField] float driftOffsetOrientation;

    // Holds the formation pattern
    [SerializeField] FormationPattern pattern;

    // Updates the assignment of characters to slots
    void UpdateSlotAssignments()
    {
        // A very simple assignment algorithm:
        // we simply go through each assignment in
        // the list and assign sequential slot numbers
        /*for (int i = 0; i < slotAssignments.Count; i++)
        {
            slotAssignments[i]
        }*/

        // Update the drift offset
        //pattern.getDriftOffset(slot assignment)
        pattern.GetDriftOffset(slotAssignments, ref driftOffsetPosition, ref driftOffsetOrientation);
    }


    // Add a new character to the first available slot. 
    // Returns false if no more slots are available.
    bool AddCharacter(GameObject character)
    {
        // Find how many slots we have occupied
        int occupiedSlots = numSlots - slotAssignments.Count;

        // Check if the pattern supports more slots
        // if pattern.SupportsSlots(occupiedSlots+1);
        if (occupiedSlots + 1 < numSlots)
        {
            // Add a new slot assignment
            slotAssignments.Add(character);

            // Update the slot assignments and return success
            UpdateSlotAssignments();
            return true;
        }
        else
        {
            return false;
        }
    }

    // Removes a character from its slot
    void RemoveCharacter(GameObject character)
    {
        // Find Character's slot
        int slot = slotAssignments.IndexOf(character);

        // Made sure we've found a valid result
        if (slot >= 0 && slot < numSlots)
        {
            slotAssignments.RemoveAt(slot);

            // Update the assignments
            UpdateSlotAssignments();
        }
    }

    // Write new slot locations to each character
    // Assumes slot 0 is the leader
    void UpdateSlots()
    {
        if (slotAssignments.Count == 0)
        {
            return;
        }

        // Find the anchor point
        //GameObject anchor = slotAssignments[0];

        // Get orientation of the anchor as a matrix
        Vector3 orientationMatrix = anchor.transform.rotation.eulerAngles;

        // Go through each character in turn
        for (int i = 0; i < slotAssignments.Count; i++)
        {
            GameObject target = slotAssignments[i];
            if (target == anchor)
            {
                continue;
            }
            // Ask for the location of the slot relative to the anchro Point.
            Vector3 distance = target.transform.position - anchor.transform.position;
            Vector3 relativePosition = Vector3.zero;
            relativePosition.x = Vector3.Dot(distance, anchor.transform.right.normalized);
            relativePosition.y = Vector3.Dot(distance, anchor.transform.up.normalized);
            relativePosition.z = Vector3.Dot(distance, anchor.transform.forward.normalized);

            // Relative rotation
            Quaternion relativeRotation = Quaternion.Inverse(anchor.transform.rotation) * target.transform.rotation;

            // Transform it by the anchor point's position and orientation
            target.transform.position = Vector3.Scale(relativePosition, orientationMatrix) + anchor.transform.position;
            target.transform.rotation = Quaternion.Euler(anchor.transform.rotation.eulerAngles + relativeRotation.eulerAngles);
            // Relative Position Equation: https://answers.unity.com/questions/346671/relative-position-1.html
            // Relative Rotation Equation: https://answers.unity.com/questions/35541/problem-finding-relative-rotation-from-one-quatern.html

            // Add drift component
            target.transform.position -= new Vector3(driftOffsetPosition.x, driftOffsetPosition.y, 0);
            Vector3 newRotation = target.transform.rotation.eulerAngles;
            newRotation.z -= driftOffsetOrientation;
            target.transform.rotation = Quaternion.Euler(newRotation);
        }
    }
}
