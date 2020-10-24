using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerFSM
{
    public abstract class PlayerState
    {
        public abstract void Init(PlayerCharacter player);

        public abstract void Action(PlayerCharacter player);

        public abstract PlayerState Transition(PlayerCharacter player);
    }

    #region Movement States
    public class DirectMovementState : PlayerState
    {
        public override void Action(PlayerCharacter player)
        {
            player.ManualMovement(player.input.MoveInput, player.transform.forward, player.transform.right);
            if (player.input.Jump)
                player.Jump();
            player.Gravity();
            player.ApplyMovement();

            if (player.AttackAnimationFinished && player.CanAttack)
                player.animator.WalkingAnimation(player.CanAttack && player.input.MoveInput.magnitude > 0 && player.IsNearGround());
        }

        public override void Init(PlayerCharacter player)
        {

        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (player.input.Dash && player.DashMeter >= 0)
                return new DashState();
            return null;
        }
    }

    public class DashState : PlayerState
    {
        private float timeCounter = 0;
        private Vector3 dashDirection;
        private float currentDashDuration;

        public override void Action(PlayerCharacter player)
        {
            player.NonInertialMovement(dashDirection * player.playerStats.dashSpeed);
            player.ManualRotation(player.input.CameraInput);
            player.ApplyMovement();

            timeCounter += Time.deltaTime;
        }

        public override void Init(PlayerCharacter player)
        {
            player.DeduceDashMeter();
            player.SetVerticalVelocity(0);
            Vector2 moveInput = player.input.MoveInput;
            if (moveInput.magnitude == 0)
                moveInput = new Vector2(0, 1);
            else
                moveInput.Normalize();

            moveInput.Normalize();
            player.RedirectInertia(moveInput, player.transform.forward, player.transform.right);
            Vector3 forward = player.transform.forward;
            dashDirection = forward * moveInput.y + player.transform.right * moveInput.x;

            currentDashDuration = player.playerStats.dashDuration * player.DashMeter;

            PlayerCamera.Recoil(20);
            PlayerCamera.ScreenShake(currentDashDuration);

            player.animator.PlayDashSound();
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (timeCounter < currentDashDuration)
                return null;
            PlayerCamera.Recoil();
            PlayerCamera.ScreenShake(0.2f);
            return new DirectMovementState();
        }
    }

    #endregion

    #region Combat States

    public class BasicAttackState : PlayerState
    {
        public override void Action(PlayerCharacter player)
        {
            player.ManualRotation(player.input.CameraInput);

            if (player.input.Interact && player.CanAttack)
                player.Interact();

            if (player.input.Attack)
                player.MeleeAttack();

            if (player.input.ForcePush)
                player.ForcePush();

            if (player.input.InspectWeapon)
                player.InspectWeapon();
        }

        public override void Init(PlayerCharacter player)
        {
            
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if(player.CanAttack && player.input.SpecialAttack)
            {
                if (player.input.MoveInputDirection == PlayerInput.MovementInputDirection.Forward)
                    return new StingAttackStartState();
                else if (player.input.MoveInputDirection == PlayerInput.MovementInputDirection.Backwards && player.CanUseRevolver())
                    return new RevolverAttackStartState();
                else if (player.input.MoveInputDirection == PlayerInput.MovementInputDirection.Right || player.input.MoveInputDirection == PlayerInput.MovementInputDirection.Left)
                    return new SpinAttackStartState();
            }
            return null;
        }
    }

    // Revolver Attack
    public class RevolverAttackStartState : PlayerState
    {
        private float timeCounter;
        private float chargeTime, chargeTimeCounter;
        public override void Action(PlayerCharacter player)
        {
            player.ManualRotation(player.input.CameraInput);
            timeCounter -= Time.deltaTime;
            chargeTimeCounter += Time.deltaTime;
            player.SpecialAttackMeter = Mathf.Clamp(chargeTimeCounter / chargeTime, 0, 1);
        }

        public override void Init(PlayerCharacter player)
        {
            player.RevolverAttackStart();
            timeCounter = player.animator.revolverStartAnimationDuration;
            chargeTime = player.playerStats.revolverAttackChargeTime;
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (timeCounter <= 0 && !player.input.SpecialAttack)
                return new RevolverAttackEndState();
            return null;
        }
    }

    public class RevolverAttackEndState : PlayerState
    {
        public override void Action(PlayerCharacter player)
        {
            player.ManualRotation(player.input.CameraInput);
        }

        public override void Init(PlayerCharacter player)
        {
            player.RevolverAttackEnd();
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (player.CanAttack)
                return new BasicAttackState();
            return null;
        }
    }


    // Sting Attack
    public class StingAttackStartState : PlayerState
    {
        private float timeCounter;
        private float chargeTime, chargeTimeCounter;
        public override void Action(PlayerCharacter player)
        {
            player.ManualRotation(player.input.CameraInput);
            timeCounter -= Time.deltaTime;
            chargeTimeCounter += Time.deltaTime;
            player.SpecialAttackMeter = Mathf.Clamp(chargeTimeCounter / chargeTime, 0, 1);
        }

        public override void Init(PlayerCharacter player)
        {
            player.StingAttackStart();
            timeCounter = player.animator.stingStartAnimationDuration;
            chargeTime = player.playerStats.stingAttackChargeTime;
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (timeCounter <= 0 && !player.input.SpecialAttack)
                return new StingAttackEndState();
            return null;
        }
    }

    public class StingAttackEndState : PlayerState
    {
        public override void Action(PlayerCharacter player)
        {
            player.ManualRotation(player.input.CameraInput);
        }

        public override void Init(PlayerCharacter player)
        {
            player.StingAttackEnd();
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (player.CanAttack)
                return new BasicAttackState();
            return null;
        }
    }


    // Spin Attack
    public class SpinAttackStartState : PlayerState
    {
        public float timeCounter;
        private float chargeTime, chargeTimeCounter;
        public override void Action(PlayerCharacter player)
        {
            player.ManualRotation(player.input.CameraInput);
            timeCounter -= Time.deltaTime;
            chargeTimeCounter += Time.deltaTime;
            player.SpecialAttackMeter = Mathf.Clamp(chargeTimeCounter / chargeTime, 0, 1);
        }

        public override void Init(PlayerCharacter player)
        {
            player.SpinAttackStart();
            timeCounter = player.animator.spinStartAnimationDuration;
            chargeTime = player.playerStats.spinAttackChargeTime;
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (timeCounter <= 0 && !player.input.SpecialAttack)
                return new SpinAttackEndState();
            return null;
        }
    }

    public class SpinAttackEndState : PlayerState
    {
        public override void Action(PlayerCharacter player)
        {

        }

        public override void Init(PlayerCharacter player)
        {
            player.SpinAttackEnd();
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (player.CanAttack)
                return new BasicAttackState();
            return null;
        }
    }

    #endregion
}



