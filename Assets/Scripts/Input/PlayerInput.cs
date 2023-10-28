using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public ShipBehaviour Player;

    private InputActions inputActions;

    private bool _isThrusting;
    private bool _isRotatingRight;
    private bool _isRotatingLeft;

    // Start is called before the first frame update
    void Awake()
    {
        inputActions = new InputActions();
        inputActions.Enable();
    }

    private void Update()
    {
        if (_isThrusting)
            Thrust();

        if (_isRotatingRight)
            RotateRight();

        if (_isRotatingLeft)
            RotateLeft();
    }

    public void OnThrust(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started: _isThrusting = true; break;
            case InputActionPhase.Canceled: _isThrusting = false; break;
        }
    }

    public void OnRotateRight(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started: _isRotatingRight = true; break;
            case InputActionPhase.Canceled: _isRotatingRight = false; break;
        }
    }

    public void OnRotateLeft(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started: _isRotatingLeft = true; break;
            case InputActionPhase.Canceled: _isRotatingLeft = false; break;
        }
    }

    private void Thrust()
    {
        Player.Thrust();
    }

    private void RotateRight()
    {
        Player.RotateRight();
    }

    private void RotateLeft()
    {
        Player.RotateLeft();
    }
}
