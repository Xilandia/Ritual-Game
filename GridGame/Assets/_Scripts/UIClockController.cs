using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClockController : MonoBehaviour
{
    [SerializeField] private GameObject clockHand;
    [SerializeField] private bool isTicking = false;

    private float currentRotation = 0;
    private float targetRotation = 0;
    private float rotationOffset = 0;
    private int lastPhase = 0;

    void Update()
    {
        if (isTicking)
        {
            RotateClockHand();
        }
    }

    private void RotateClockHand()
    {
        float previousRotation = currentRotation;
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * 5);
        float deltaAngle = currentRotation - previousRotation;.
        clockHand.transform.RotateAround(transform.position, Vector3.forward, -deltaAngle);
        
        if (Mathf.Abs(currentRotation - targetRotation) < 0.1f)
        {
            isTicking = false;
            InitiativeManager.Instance.NextPhase();
        }
    }

    public void TickClock(int phase)
    {
        if (phase < lastPhase)
        {
            rotationOffset += 360;
        }

        lastPhase = phase;
        targetRotation = phase * 30 + rotationOffset;
        isTicking = true;
    }
}
