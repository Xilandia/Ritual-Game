using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClockController : MonoBehaviour
{
    [SerializeField] private GameObject clockHand;
    [SerializeField] private bool isTicking = false;

    private float currentRotation = 0;
    private float targetRotation = 0;

    // Tracks additional rotation when phases wrap from 11 to 0.
    private float rotationOffset = 0;
    // Stores the last phase so we can detect when it loops.
    private int lastPhase = 0;

    // Update is called once per frame
    void Update()
    {
        if (isTicking)
        {
            RotateClockHand();
        }
    }

    private void RotateClockHand()
    {
        // Store the previous rotation value.
        float previousRotation = currentRotation;
        // Smoothly interpolate currentRotation towards targetRotation.
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * 5);
        // Calculate how much we have rotated since the last update.
        float deltaAngle = currentRotation - previousRotation;
        // Rotate the clockHand around the pivot (this object's position) using the delta angle.
        clockHand.transform.RotateAround(transform.position, Vector3.forward, -deltaAngle);

        // Once close enough to the target, stop ticking and trigger the next phase.
        if (Mathf.Abs(currentRotation - targetRotation) < 0.1f)
        {
            isTicking = false;
            InitiativeManager.Instance.NextPhase();
        }
    }

    public void TickClock(int phase)
    {
        // Detect phase wrap-around (e.g. from 11 to 0) and add 360 degrees.
        if (phase < lastPhase)
        {
            rotationOffset += 360;
        }
        lastPhase = phase;
        // Use the rotation offset to maintain a continuous rotation.
        targetRotation = phase * 30 + rotationOffset;
        isTicking = true;
    }
}
