using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClockController : MonoBehaviour
{
    [SerializeField] private GameObject clockHand;
    [SerializeField] private bool isTicking = false;

    private float currentRotation = 0;
    private float targetRotation = 0;

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
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * 5);
        clockHand.transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        if (Mathf.Abs(currentRotation - targetRotation) < 0.1f)
        {
            isTicking = false;
            InitiativeManager.Instance.NextPhase();
        }
    }

    public void TickClock(int phase)
    {
        isTicking = true;
        targetRotation = phase * 30;
    }
}
