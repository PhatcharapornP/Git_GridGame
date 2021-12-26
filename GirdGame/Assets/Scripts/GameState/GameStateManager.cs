using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private BaseState[] states;
    private Dictionary<Type,IGameState> stateDict = new Dictionary<Type,IGameState>();

    public void InitializeState()
    {
        InitializeStateDict();

        foreach (var state in states)
            state.Initialize();
        
        GetStateViaType(typeof(MenuState)).StartState();
    }

    private void InitializeStateDict()
    {
        stateDict.Clear();
        foreach (var state in states)
            stateDict.Add(state.GetType(), state);
    }

    public IGameState GetStateViaType(Type type)
    {
        return stateDict[type];
    }
}
