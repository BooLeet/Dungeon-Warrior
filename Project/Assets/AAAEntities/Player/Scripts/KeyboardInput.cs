using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : PlayerInput {
    public string attackKey = "mouse 0";
    public string zoomKey = "mouse 1";
    public string interactKey = "e";
    public string dashKey = "left shift";
    public string jumpKey = "space";
    public string meleeKey = "e";
    [Space]
    public string moveHorizontalAxis = "Horizontal";
    public string moveVerticalAxis = "Vertical";
    [Space]
    public string cameraHorizontalAxis = "Mouse X";
    public string cameraVerticalAxis = "Mouse Y";
    private float keyPressValidTime = 0.1f;
    private float jumpTimeCounter, interactTimeCounter, meleeTimeCounter;

    void Update()
    {
        Attack = Input.GetKey(attackKey);
        Zoom = Input.GetKey(zoomKey);

        if (Input.GetKeyUp(interactKey))
            interactTimeCounter = keyPressValidTime;
        if (Input.GetKey(jumpKey))
            jumpTimeCounter = keyPressValidTime;
        if (Input.GetKey(meleeKey))
            meleeTimeCounter = keyPressValidTime;

        Interact = interactTimeCounter > 0;
        Jump = jumpTimeCounter > 0;
        Melee = meleeTimeCounter > 0;
        Dash = Input.GetKeyDown(dashKey);
        

        MoveInput = new Vector2(Input.GetAxisRaw(moveHorizontalAxis), Input.GetAxisRaw(moveVerticalAxis));
        if (MoveInput.magnitude > 1)
            MoveInput = MoveInput.normalized;
        CameraInput = new Vector2(Input.GetAxisRaw(cameraHorizontalAxis), Input.GetAxisRaw(cameraVerticalAxis));
        
        // Time counting
        interactTimeCounter -= Time.deltaTime;
        jumpTimeCounter -= Time.deltaTime;
        meleeTimeCounter -= Time.deltaTime;

        if (interactTimeCounter < 0)
            interactTimeCounter = 0;
        if (jumpTimeCounter < 0)
            jumpTimeCounter = 0;
    }

}
