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

    /// <summary>
    /// Returns the instance of the player input
    /// </summary>
    /// <returns>Instance of the player input</returns>
    public static PlayerInput GetInstance()
    {
        return _playerInput;
    }

    /// <summary>
    /// Binds the current player to the input
    /// </summary>
    /// <param name="player">Player to bind</param>
    public void SetPlayer(ShipBehaviour player)
    {
        _player = player;
    }

    /// <summary>
    /// Unbinds the current player
    /// </summary>
    public void RemovePlayer()
    {
        _player = null;
    }

    /// <summary>
    /// Handles the thrust input action
    /// </summary>
    /// <param name="context">Input context</param>
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

    /// <summary>
    /// Handles the rotate right action
    /// </summary>
    /// <param name="context">Input context</param>
    public void OnRotateRight(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started: _isRotatingRight = true; break;
            case InputActionPhase.Canceled: _isRotatingRight = false; break;
        }
    }

    /// <summary>
    /// Handles the rotate left action
    /// </summary>
    /// <param name="context">Input context</param>
    public void OnRotateLeft(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started: _isRotatingLeft = true; break;
            case InputActionPhase.Canceled: _isRotatingLeft = false; break;
        }
    }

    /// <summary>
    /// Thrusts the player ship
    /// </summary>
    private void Thrust()
    {
        if (_player)
            _player.Thrust();
    }

    /// <summary>
    /// Stops the player ship thrust
    /// </summary>
    private void StopThrust()
    {
        if (_player)
            _player.StopThrust();
    }

    /// <summary>
    /// Rotates the player ship to the right
    /// </summary>
    private void RotateRight()
    {
        if (_player)
            _player.RotateRight();
    }

    /// <summary>
    /// Rotates the player ship to the left
    /// </summary>
    private void RotateLeft()
    {
        if (_player)
            _player.RotateLeft();
    }
}
