using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : PlayerInput {
    public bool attackHold = true;
    public string attackKey = "mouse 0";
    public string specialAttackKey = "mouse 1";
    public string interactKey = "e";
    public string jumpKey = "space";
    public string dashKey = "left shift";
    public string forcePushKey = "c";
    public string pauseKey = "escape";
    public string inspectKey = "f";
    [Space]
    public string moveHorizontalAxis = "Horizontal";
    public string moveVerticalAxis = "Vertical";
    [Space]
    public string cameraHorizontalAxis = "Mouse X";
    public string cameraVerticalAxis = "Mouse Y";
    public float keyPressValidTime = 0.1f;
    private float attackTimeCounter, jumpTimeCounter, forcePushTimeCounter, interactTimeCounter;

    public override void UpdateValues()
    {
        pause = Input.GetKeyDown(pauseKey);

        StickyInput(ref attackTimeCounter, ref attack, attackHold? Input.GetKey(attackKey) : Input.GetKeyDown(attackKey));
        StickyInput(ref forcePushTimeCounter, ref forcePush, Input.GetKeyDown(forcePushKey));
        StickyInput(ref jumpTimeCounter, ref jump, Input.GetKeyDown(jumpKey));
        StickyInput(ref interactTimeCounter, ref interact, Input.GetKeyDown(interactKey));
        dash = Input.GetKeyDown(dashKey);
        specialAttack = Input.GetKey(specialAttackKey);
        inspectWeapon = Input.GetKeyDown(inspectKey);

        MoveInput = new Vector2(Input.GetAxisRaw(moveHorizontalAxis), Input.GetAxisRaw(moveVerticalAxis));
        if (MoveInput.magnitude > 1)
            MoveInput = MoveInput.normalized;
        CameraInput = new Vector2(Input.GetAxisRaw(cameraHorizontalAxis), Input.GetAxisRaw(cameraVerticalAxis));
    }

    private void StickyInput(ref float timeCounter, ref bool boolToStick, bool input)
    {
        if (input)
            timeCounter = keyPressValidTime;

        boolToStick = timeCounter > 0;

        timeCounter -= Time.deltaTime;
        if (timeCounter < 0)
            timeCounter = 0;
    }

}
