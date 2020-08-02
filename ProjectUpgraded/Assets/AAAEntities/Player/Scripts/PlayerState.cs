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
            player.ManualMovement(player.input.MoveInput, player.transform.forward, player.transform.right, 0.5f);
            player.Gravity();
            player.ApplyMovement();

            if (player.AttackAnimationFinished && player.CanAttack)
                player.animator.WalkingAnimation(player.CanAttack && player.input.MoveInput.magnitude > 0 && player.IsNearGround());

            //if (player.input.Attack)
            //    player.MeleeAttack();
        }

        public override void Init(PlayerCharacter player)
        {

        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (player.input.Dash && player.DashMeter >= 1)
                return new DashState();
            return null;
        }
    }

    public class DashState : PlayerState
    {
        private float timeCounter = 0;
        private Vector3 dashDirection;

        public override void Action(PlayerCharacter player)
        {
            player.NonInertialMovement(dashDirection * player.dashSpeed);
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

            PlayerCamera.Recoil(20);
            PlayerCamera.ScreenShake(player.dashDuration);

            player.animator.PlayDashSound();
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (timeCounter < player.dashDuration)
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
                else if (player.input.MoveInputDirection == PlayerInput.MovementInputDirection.Backwards)
                    return new RevolverAttack();
                else if (player.input.MoveInputDirection == PlayerInput.MovementInputDirection.Right || player.input.MoveInputDirection == PlayerInput.MovementInputDirection.Left)
                    return new SpinAttackStartState();
            }
            return null;
        }
    }

    // Revolver Attack
    public class RevolverAttack : PlayerState
    {
        public float timeCounter;

        public override void Action(PlayerCharacter player)
        {
            player.ManualRotation(player.input.CameraInput);
            timeCounter -= Time.deltaTime;
            if(player.CanAttack && player.input.SpecialAttack)
            {
                player.RevolverAttack();
                timeCounter = player.animator.revolverAttack.attackAnimationDuration;
            }
        }

        public override void Init(PlayerCharacter player)
        {
            timeCounter = player.animator.revolverAttack.attackAnimationDuration;
            player.RevolverAttack();
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (timeCounter <= 0)
                return new BasicAttackState();
            if (player.CanAttack && player.input.Attack)
            {
                player.MeleeAttack();
                return new BasicAttackState();
            }
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
            chargeTime = player.stingAttackChargeTime;
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
            chargeTime = player.spinAttackChargeTime;
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



