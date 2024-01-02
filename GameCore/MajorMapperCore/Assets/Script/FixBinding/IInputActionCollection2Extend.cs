using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class IInputActionCollection2Extend : IInputActionCollection2
{
    public IEnumerable<InputBinding> bindings => throw new NotImplementedException();

    public InputBinding? bindingMask { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public ReadOnlyArray<InputDevice>? devices { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ReadOnlyArray<InputControlScheme> controlSchemes => throw new NotImplementedException();

    public bool Contains(InputAction action)
    {
        throw new NotImplementedException();
    }

    public void Disable()
    {
        throw new NotImplementedException();
    }

    public void Enable()
    {
        throw new NotImplementedException();
    }

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        throw new NotImplementedException();
    }

    public int FindBinding(InputBinding mask, out InputAction action)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
