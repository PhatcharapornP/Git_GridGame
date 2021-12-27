using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameState
{
    void Initialize();
    void StartState();
    void EndState();
}
