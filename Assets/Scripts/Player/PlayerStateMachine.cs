using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages player state transitions and execution
/// </summary>
public class PlayerStateMachine
{
    private Dictionary<PlayerController.PlayerState, IPlayerState> _states;
    private IPlayerState _currentStateHandler;
    private PlayerController.PlayerState _currentState;
    private PlayerController _controller;

    public PlayerController.PlayerState CurrentState => _currentState;

    public PlayerStateMachine(PlayerController controller)
    {
        _controller = controller;
        _states = new Dictionary<PlayerController.PlayerState, IPlayerState>();
        InitializeStates();
    }

    /// <summary>
    /// Initialize all state handlers
    /// </summary>
    private void InitializeStates()
    {
        // Create all state instances - initially they'll wrap the existing logic
        _states[PlayerController.PlayerState.Idle] = new IdleState();
        _states[PlayerController.PlayerState.Run] = new RunState();
        _states[PlayerController.PlayerState.Attack] = new AttackState();
        _states[PlayerController.PlayerState.Aiming] = new AimingState();
        _states[PlayerController.PlayerState.Attack2] = new Attack2State();
        _states[PlayerController.PlayerState.Block] = new BlockState();
        _states[PlayerController.PlayerState.Jump] = new JumpState();
        _states[PlayerController.PlayerState.JumpAttack] = new JumpAttackState();
        _states[PlayerController.PlayerState.Falling] = new FallingState();
        _states[PlayerController.PlayerState.Crouch] = new CrouchState();
        _states[PlayerController.PlayerState.Crouched] = new CrouchedState();
        _states[PlayerController.PlayerState.CrouchAttack] = new CrouchAttackState();
        _states[PlayerController.PlayerState.CrouchBlock] = new CrouchBlockState();
        _states[PlayerController.PlayerState.CrouchHurt] = new CrouchHurtState();
        _states[PlayerController.PlayerState.Roll] = new RollState();
        _states[PlayerController.PlayerState.FaceWall] = new FaceWallState();
        _states[PlayerController.PlayerState.Ladder] = new LadderState();
        _states[PlayerController.PlayerState.Climb] = new ClimbState();
        _states[PlayerController.PlayerState.Hurt] = new HurtState();
        _states[PlayerController.PlayerState.Dead] = new DeadState();
    }

    /// <summary>
    /// Change to a new state
    /// </summary>
    /// <param name="newState">The state to transition to</param>
    public void ChangeState(PlayerController.PlayerState newState)
    {
        // Only change if it's actually different
        if (_currentState == newState)
            return;

        // Check if current state allows this transition
        if (_currentStateHandler != null && !_currentStateHandler.CanTransitionTo(newState, _controller))
            return;

        // Exit current state
        _currentStateHandler?.OnExit(_controller);

        // Update state
        var previousState = _currentState;
        _currentState = newState;
        _currentStateHandler = _states[newState];

        // Enter new state
        _currentStateHandler.OnEnter(_controller);

        // Call the original ChangePlayerState for debugging and other systems
        _controller.CallOriginalChangePlayerState(newState);
    }

    /// <summary>
    /// Update the current state (called from Update)
    /// </summary>
    public void Update()
    {
        _currentStateHandler?.OnUpdate(_controller);
    }

    /// <summary>
    /// Fixed update the current state (called from FixedUpdate)
    /// </summary>
    public void FixedUpdate()
    {
        _currentStateHandler?.OnFixedUpdate(_controller);
    }

    /// <summary>
    /// Initialize the state machine with a starting state
    /// </summary>
    /// <param name="initialState">The initial state</param>
    public void Initialize(PlayerController.PlayerState initialState)
    {
        _currentState = initialState;
        _currentStateHandler = _states[initialState];
        _currentStateHandler?.OnEnter(_controller);
    }
}
