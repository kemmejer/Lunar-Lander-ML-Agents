using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private ShipBehaviour _player;
    private InputActions _inputActions;

    private static PlayerInput _playerInput;

    private bool _isThrusting;
    private bool _isRotatingRight;
    private bool _isRotatingLeft;

    // Start is called before the first frame update
    void Awake()
    {
        _inputActions = new InputActions();
        _inputActions.Enable();
        _playerInput = gameObject.GetComponent<PlayerInput>();
    }

    private void FixedUpdate()
    {
        if (_isThrusting)
            Thrust();

        if (_isRotatingRight)
            RotateRight();

        if (_isRotatingLeft)
            RotateLeft();
    }

    public static PlayerInput GetInstance()
    {
        return _playerInput;
    }

    public void SetPlayer(ShipBehaviour player)
    {
        _player = player;
    }

    public void RemovePlayer()
    {
        _player = null;
    }

    public void OnThrust(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                _isThrusting = true;
                break;
            case InputActionPhase.Canceled:
                _isThrusting = false;
                StopThrust();
                break;
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
        if (_player)
            _player.Thrust();
    }

    private void StopThrust()
    {
        if (_player)
            _player.StopThrust();
    }

    private void RotateRight()
    {
        if (_player)
            _player.RotateRight();
    }

    private void RotateLeft()
    {
        if (_player)
            _player.RotateLeft();
    }
}
