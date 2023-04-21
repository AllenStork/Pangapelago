using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState, IRootState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    :base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubState();
    }

    public override void EnterState()
    {
        HandleJump();
    }

    public override void UpdateState()
    {
        HandleGravity();
        Debug.Log("UPDATE FROM JUMP STATE");
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Debug.Log("EXITING JUMP STATE");
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
        if (Ctx.IsJumpPressed)
        {
            Ctx.RequireNewJumpPress = true;
        }

    }

    public override void InitializeSubState() 
    {
        if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        }
        else
        {
            SetSubState(Factory.Run());
        }
    }

    public override void CheckSwitchStates() 
    {
    if (Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    void HandleJump()
    {
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, true);
        //ctx.IsJumpAnimating = true;
        Ctx.IsJumping = true;
        Ctx.CurrentMovementY = Ctx.InitialJumpVelocity * .5f;
        Ctx.CurrentRunMovementY = Ctx.InitialJumpVelocity * .5f;
    }

    public void HandleGravity()
    {
        bool isFalling = Ctx.CurrentMovementY <= 0.0f || !Ctx.IsJumpPressed;
        float fallMultiplier = 2.0f;
        // apply proper gravity depending on if the character is grounded or not
        if (isFalling)
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            float newYVelocity = Ctx.CurrentMovementY + (Ctx.Gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
            Ctx.CurrentMovementY = nextYVelocity;
            Ctx.CurrentRunMovementY = nextYVelocity;
        }
        else
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            float newYVelocity = Ctx.CurrentMovementY + (Ctx.Gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
            Ctx.CurrentMovementY = nextYVelocity;
            Ctx.CurrentRunMovementY = nextYVelocity;
        }

    }

}
