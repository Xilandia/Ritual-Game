using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiativeManager : MonoBehaviour
{

    [SerializeField] private bool isPaused = false;
    [SerializeField] private bool isClockTicking = false;
    [SerializeField] private bool isPhaseDone = false;
    [SerializeField] private int numPhases = 12;
    [SerializeField] private UIClockController clockController;

    public int currentPhase { get; private set; }
    public static InitiativeManager Instance;

    private List<Action>[] preMovementActionStack;
    private List<Action>[] movementActionStack;
    private List<Action>[] postMovementActionStack;
    
    private float waitTime = 1f;
    private float currentWait = 0f;
    private bool queuedPause = false;
    private bool queuedResume = false;

    void Start()
    {
        Instance = this;
        currentPhase = 0;
        Pause();
        InitActionStack();
    }

    void Update()
    {
        if (!isPaused)
        {
            if (isPhaseDone)
            {
                currentWait = 0f;
                isPhaseDone = false;
            }
            // Find a way to tell when current phase is done executing to call next phase
            // NextPhase();
            // ExecutePhase();

            if (currentWait < waitTime)
            {
                currentWait += Time.deltaTime;

                if (currentWait >= waitTime)
                {
                    clockController.TickClock(currentPhase);
                    isClockTicking = true;
                }
            }
        }
    }

    void Pause()
    {
        if (isClockTicking)
        {
            queuedPause = true;
        }
        else
        {
            isPaused = true;
        }
    }

    void Resume()
    {
        isPaused = false;

        if (queuedResume)
        {
            queuedResume = false;
            NextPhase();
        }
    }

    void InitActionStack()
    {
        preMovementActionStack = new List<Action>[numPhases];
        movementActionStack = new List<Action>[numPhases];
        postMovementActionStack = new List<Action>[numPhases];
        for (int i = 0; i < numPhases; i++)
        {
            preMovementActionStack[i] = new List<Action>();
            movementActionStack[i] = new List<Action>();
            postMovementActionStack[i] = new List<Action>();
        }
    }

    public void AddPreMovementAction(Action action, int phase)
    {
        preMovementActionStack[phase % numPhases].Add(action);
    }

    public void AddMovementAction(Action action, int phase)
    {
        movementActionStack[phase % numPhases].Add(action);
    }

    public void AddPostMovementAction(Action action, int phase)
    {
        postMovementActionStack[phase % numPhases].Add(action);
    }

    void ProgressPhaseCounter()
    {
        currentPhase = (currentPhase + 1) % numPhases;
    }

    void ExecutePhase()
    {
        foreach (Action action in preMovementActionStack[currentPhase])
        {
            action.Execute();
        }

        foreach (Action action in movementActionStack[currentPhase])
        {
            action.Execute();
        }

        foreach (Action action in postMovementActionStack[currentPhase])
        {
            action.Execute();
        }

        preMovementActionStack[currentPhase].Clear();
        movementActionStack[currentPhase].Clear();
        postMovementActionStack[currentPhase].Clear();
    }

    public void NextPhase()
    {
        isClockTicking = false;

        if (queuedPause)
        {
            queuedPause = false;
            queuedResume = true;
            Pause();
            return;
        }

        ProgressPhaseCounter();
        ExecutePhase();

        isPhaseDone = true;
    }
}
