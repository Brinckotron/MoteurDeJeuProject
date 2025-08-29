using UnityEngine;

/// <summary>
/// A temporary wrapper state that delegates all state logic back to the original StateControl method.
/// This allows us to migrate to the state machine pattern while preserving exact behavior.
/// In Phase 2, we'll gradually replace these with proper state implementations.
/// </summary>
public class LegacyStateWrapper : BasePlayerState
{
    public override void OnFixedUpdate(PlayerController controller)
    {
        // Delegate to the original state control logic
        controller.CallOriginalStateControl();
    }
}

// For Phase 1, all states will use this wrapper to preserve existing behavior
public class IdleState : LegacyStateWrapper { }
public class RunState : LegacyStateWrapper { }
public class AttackState : LegacyStateWrapper { }
public class AimingState : LegacyStateWrapper { }
public class Attack2State : LegacyStateWrapper { }
public class BlockState : LegacyStateWrapper { }
public class JumpState : LegacyStateWrapper { }
public class JumpAttackState : LegacyStateWrapper { }
public class FallingState : LegacyStateWrapper { }
public class CrouchState : LegacyStateWrapper { }
public class CrouchedState : LegacyStateWrapper { }
public class CrouchAttackState : LegacyStateWrapper { }
public class CrouchBlockState : LegacyStateWrapper { }
public class CrouchHurtState : LegacyStateWrapper { }
public class RollState : LegacyStateWrapper { }
public class FaceWallState : LegacyStateWrapper { }
public class LadderState : LegacyStateWrapper { }
public class ClimbState : LegacyStateWrapper { }
public class HurtState : LegacyStateWrapper { }
public class DeadState : LegacyStateWrapper { }
