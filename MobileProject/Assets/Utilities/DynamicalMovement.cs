using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicalMovement : MonoBehaviour
{
    public Vector3 force;
    public CharacterController characterController;
    public float mass = 50;
    public float jumpAcceleration = 100;
    public float freeFallAcceleration = 10;
    [Space]
    public float movementAcceleration = 20;
    public float frictionCoefficient = 1;

    public Vector3 gravityForceVector = Vector3.zero;

    private Vector2 movementInput = Vector2.zero;
    private bool jump = false;

    void Update()
    {
        movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (movementInput.magnitude > 1)
            movementInput.Normalize();
        jump = Input.GetKeyDown("space");
    }

    private void FixedUpdate()
    {
        Movement(movementInput, jump,Time.fixedDeltaTime);
    }

    public void Movement(Vector2 movementInput, bool jump,float deltaTime)
    {
        // Vertical forces
        gravityForceVector = Vector3.down * freeFallAcceleration * mass;
        if (IsGrounded() && force.y < 0)
            gravityForceVector = Vector3.down * force.y;

        Vector3 jumpForceVector = Vector3.zero;
        if (jump && CanJump())
            jumpForceVector = Vector3.up * jumpAcceleration * mass;

        // Horizontal forces
        Vector3 direction = transform.right * movementInput.x + transform.forward * movementInput.y;
        Vector3 horizontalForce = direction * mass * movementAcceleration;
        Vector3 frictionForce = -direction * frictionCoefficient * mass * freeFallAcceleration;

        force += gravityForceVector + jumpForceVector;

        // Applying Force
        characterController.Move(((force + horizontalForce + frictionForce) / mass) * deltaTime * deltaTime);
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.1f))
            return true;

        return characterController.isGrounded;
    }

    private bool CanJump()
    {
        return IsGrounded();
    }
}
