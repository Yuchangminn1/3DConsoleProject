using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    PlayerState currentState;
    public void ChangState(PlayerState _newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = _newState;
        _newState.Enter();
    }
    public void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }
    public void FixedUpdate()
    {
        if (currentState != null)
        {
            currentState.FixedUpdate();
        }
    }
    public void LateUpdate()
    {
        if (currentState != null)
        {
            currentState.LateUpdate();
        }
    }
}
