using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiativeManager : MonoBehaviour
{

    [SerializeField] private bool isPaused = false;
    [SerializeField] private bool isClockTicking = false;
    [SerializeField] private int numPhases = 12;

    public int currentPhase { get; private set; }
    public static InitiativeManager Instance;

    private List<Action>[] preMovementActionStack;
    private List<Action>[] movementActionStack;
    private List<Action>[] postMovementActionStack;

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
            // Find a way to tell when current phase is done executing to call next phase
            // NextPhase();
            // ExecutePhase();
        }
    }

    void Pause()
    {
        isPaused = true;
    }

    void Resume()
    {
        isPaused = false;
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

        ManualNextPhase();
    }

    public void AddMovementAction(Action action, int phase)
    {
        movementActionStack[phase % numPhases].Add(action);

        ManualNextPhase();
    }

    public void AddPostMovementAction(Action action, int phase)
    {
        postMovementActionStack[phase % numPhases].Add(action);

        ManualNextPhase();
    }

    void NextPhase()
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

    public void ManualNextPhase()
    {
        NextPhase();
        ExecutePhase();
    }
}
