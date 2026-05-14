using Fusion;
using UnityEngine;

public enum InputAction
{ 
    MoveForward  = 1 << 0,
    MoveBackward = 1 << 1,
    MoveLeft     = 1 << 2,
    MoveRight    = 1 << 3,
}

public struct NetworkInputData : INetworkInput
{
    private Vector2 lookRotation;
    private char actions;

    public NetworkInputData(Vector2 lookRotation, char actions)
    {
        this.lookRotation = lookRotation;
        this.actions = actions;
    }

    public void ClearActions()
    {
        actions = (char)0;
    }

    public void AddAction(InputAction action)
    {
        actions |= (char)action;
    }

    public bool IsAction(InputAction action)
    {
        return (actions & (char)action) != 0;
    }

    public Vector2 GetLookRotation()
    {
        return lookRotation;
    }
    public void SetLookRotation(Vector2 lookRotation) 
    {
        this.lookRotation = lookRotation;
    }
}