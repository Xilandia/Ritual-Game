using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiativeManager : MonoBehaviour
{

    [SerializeField] private bool isPaused = false;
    [SerializeField] private int currentPhase = 0;
    public InitiativeManager instance;

    private List<Action>[] actionStack;

    void Start()
    {
        instance = this;
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
        actionStack = new List<Action>[12];
        for (int i = 0; i < actionStack.Length; i++)
        {
            actionStack[i] = new List<Action>();
        }
    }

    public void AddAction(Action action, int phase)
    {
        actionStack[phase].Add(action);
    }

    void NextPhase()
    {
        currentPhase++;
        if (currentPhase >= actionStack.Length)
        {
            currentPhase = 0;
        }
    }

    void ExecutePhase()
    {
        foreach (Action action in actionStack[currentPhase])
        {
            action.Execute();
        }

        actionStack[currentPhase].Clear();
    }
}
