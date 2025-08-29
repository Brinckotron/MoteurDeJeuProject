using UnityEngine;

/// <summary>
/// Interface for all player states in the state machine
/// </summary>
public interface IPlayerState
{
    /// <summary>
    /// Called when entering this state
    /// </summary>
    /// <param name="controller">The player controller</param>
    void OnEnter(PlayerController controller);
    
    /// <summary>
    /// Called every frame while in this state (Update)
    /// </summary>
    /// <param name="controller">The player controller</param>
    void OnUpdate(PlayerController controller);
    
    /// <summary>
    /// Called every fixed timestep while in this state (FixedUpdate)
    /// </summary>
    /// <param name="controller">The player controller</param>
    void OnFixedUpdate(PlayerController controller);
    
    /// <summary>
    /// Called when exiting this state
    /// </summary>
    /// <param name="controller">The player controller</param>
    void OnExit(PlayerController controller);
    
    /// <summary>
    /// Determines if this state can transition to another state
    /// </summary>
    /// <param name="newState">The target state</param>
    /// <param name="controller">The player controller</param>
    /// <returns>True if transition is allowed</returns>
    bool CanTransitionTo(PlayerController.PlayerState newState, PlayerController controller);
}
