using UnityEngine;

/// <summary>
/// Base implementation of IPlayerState with default empty implementations
/// </summary>
public abstract class BasePlayerState : IPlayerState
{
    public virtual void OnEnter(PlayerController controller)
    {
        // Default: do nothing
    }

    public virtual void OnUpdate(PlayerController controller)
    {
        // Default: do nothing
    }

    public virtual void OnFixedUpdate(PlayerController controller)
    {
        // Default: do nothing
    }

    public virtual void OnExit(PlayerController controller)
    {
        // Default: do nothing
    }

    public virtual bool CanTransitionTo(PlayerController.PlayerState newState, PlayerController controller)
    {
        // Default: allow all transitions (preserving current behavior)
        return true;
    }
}
