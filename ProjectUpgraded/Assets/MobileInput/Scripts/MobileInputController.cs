using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInputController : PlayerInput
{
    private float cameraSensitivity = 100;

    public MobileInputJoy moveJoystick;
    public MobileInputJoy primaryJoystick;
    public MobileInputJoy secondaryJoystick;
    public MobileInputButton dashButton;
    public MobileInputButton interactButton;
    public MobileInputButton pushButton;
    public MobileInputButton pauseButton;

    public MobileInputCamera cameraInput;

    public RawImage secondaryAttackImage;
    public Texture secondarySting;
    public Texture secondarySpin;
    public Texture secondaryPistol;
    public Texture secondaryNone;

    private List<MobileInputElement> elements;

    private void Start()
    {
        GameManager.instance.mobileInputController = this;

        secondaryAttackImage.texture = secondaryNone;

        elements = new List<MobileInputElement>();
        elements.Add(moveJoystick);
        elements.Add(primaryJoystick);
        elements.Add(secondaryJoystick);
        elements.Add(dashButton);
        elements.Add(interactButton);
        elements.Add(pushButton);
        elements.Add(pauseButton);

        elements.Add(cameraInput);
    }

    public override void UpdateValues()
    {
        foreach(Touch touch in Input.touches)
        {
            foreach (MobileInputElement element in elements)
            {
                if (element.HandleTouch(touch))
                    break;
            }
        }

        CameraInput = cameraSensitivity * (cameraInput.DeltaInput + primaryJoystick.DeltaInput + secondaryJoystick.DeltaInput) / Screen.height;
        MoveInput = moveJoystick.Input.normalized * Mathf.Pow(moveJoystick.Input.magnitude, 1 / 4f); 

        attack = primaryJoystick.IsPressed;
        specialAttack = secondaryJoystick.IsPressed;
        interact = interactButton.KeyDown;
        pause = pauseButton.KeyDown;
        forcePush = pushButton.KeyDown;
        dash = dashButton.KeyDown;

        if(!specialAttack || secondaryAttackImage.texture == secondaryNone)
            switch (MoveInputDirection)
            {
                case MovementInputDirection.Forward:
                    secondaryAttackImage.texture = secondarySting;
                    break;
                case MovementInputDirection.Right:
                case MovementInputDirection.Left:
                    secondaryAttackImage.texture = secondarySpin;
                    break;
                case MovementInputDirection.Backwards:
                    secondaryAttackImage.texture = secondaryPistol;
                    break;
                case MovementInputDirection.Null:
                    secondaryAttackImage.texture = secondaryNone;
                    break;
            }
    }

    public void ShowHide(bool val)
    {
        gameObject.SetActive(val);
    }
}
