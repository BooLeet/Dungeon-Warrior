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

    public class FreeWalkState : PlayerState
    {
        public override void Action(PlayerCharacter player)
        {
            player.ManualMovement(player.input.MoveInput, player.playerCamera.CameraForward, player.playerCamera.CameraRight, 0.5f);
            player.ManualRotation(player.input.CameraInput);
            player.Gravity();
            player.ZoomAbilityUpdate(player.input.Zoom);
            player.ApplyMovement();//(zoomAbilityInUse && ZoomAbilityMeter > 0) ? zoomAbilityMotionMultiplier : 1);

            if (player.input.MoveInput.magnitude > 0 && player.CanAttack && player.IsNearGround() && !player.ZoomAbilityInUse)
                player.FaceWalkingDirection(player.input.MoveInput);
            else if (player.ZoomAbilityInUse)
                player.FaceCameraDirection();

            if (player.CanAttack)
                player.animator.WalkingAnimation(player.CanAttack && player.input.MoveInput.magnitude > 0 && player.IsNearGround());

            if (player.input.Jump)
                player.Jump();

            if (player.input.Attack)
                player.RangeAttack();
        }

        public override void Init(PlayerCharacter player)
        {
            
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (player.input.Dash && player.DashMeter >= 1)
                return ReturnState(player, new DashState());
            if (player.input.Melee)
            {
                if (player.CanAttack && !player.IsNearGround() && player.playerCamera.angleV >= player.leapCameraAngleThreshold)
                    return ReturnState(player, new LeapState());
                else
                    player.MeleeAttack();
            }

            return null;
        }

        private PlayerState ReturnState(PlayerCharacter player,PlayerState state)
        {
            player.ZoomAbilityInUse = false;
            player.playerCamera.SetZoom(false);
            Time.timeScale = 1;
            return state;
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
            player.FaceWalkingDirectionImediate(moveInput);

            moveInput.Normalize();
            player.RedirectInertia(moveInput, player.playerCamera.CameraForward, player.playerCamera.CameraRight);
            Vector3 forward = player.playerCamera.transform.forward;
            if (player.playerCamera.angleV > player.dashInversionMinAngle && moveInput.y > 0)
                forward *= -1;
            dashDirection = forward * moveInput.y + player.playerCamera.transform.right * moveInput.x;

            PlayerCamera.Recoil(20);
            PlayerCamera.ScreenShake(player.dashDuration);

            player.animator.PlayDashSound();
            player.animator.PlayDashMovementEffect(dashDirection, player.dashDuration);
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (timeCounter < player.dashDuration)
                return null;
            PlayerCamera.Recoil();
            PlayerCamera.ScreenShake(0.2f);
            player.animator.PlayDashEndEffect(player.Position, dashDirection);
            return new FreeWalkState();
        }
    }

    public class LeapState : PlayerState
    {
        private float currentLeapSpeed = 0;
        private Vector3 movementVector;

        public override void Action(PlayerCharacter player)
        {
            player.ManualRotation(player.input.CameraInput);
            currentLeapSpeed = Mathf.Lerp(currentLeapSpeed, player.leapSpeed, Time.deltaTime);
            player.NonInertialMovement(movementVector * currentLeapSpeed);
            player.ApplyMovement();
        }

        public override void Init(PlayerCharacter player)
        {
            player.ResetInertia();
            player.animator.LeapStart();
            player.FaceCameraDirection();
            float angle = -player.playerCamera.angleV;
            movementVector = player.transform.forward * Mathf.Cos(angle * Mathf.Deg2Rad) + player.transform.up * Mathf.Sin(angle * Mathf.Deg2Rad);
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (!player.IsNearGround())
                return null;
            return new LeapEndState();
        }
    }

    public class LeapEndState : PlayerState
    {
        private float timeCounter = 0;

        public override void Action(PlayerCharacter player)
        {
            player.ManualRotation(player.input.CameraInput);
            timeCounter += Time.deltaTime;
        }

        public override void Init(PlayerCharacter player)
        {
            PlayerCamera.ScreenShake(0.5f, player.Position);
            PlayerCamera.Recoil();
            player.HeadKnockBack(-5);

            // Do damage
            Damage.ExplosiveDamage(player.transform.position, player.meleeAttackDamage * player.leapMeleeDamageMultiplier, player.leapRange, player);
            player.animator.LeapEnd();
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (timeCounter < player.animator.leapLandingDuration)
                return null;
            return new FreeWalkState();
        }
    }


}



