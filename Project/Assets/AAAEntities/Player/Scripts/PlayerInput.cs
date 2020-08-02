using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInput : MonoBehaviour
{
    protected bool attack;
    protected bool specialAttack;
    protected bool interact;
    protected bool dash;
    protected bool forcePush;
    public bool Attack { get { return attack; } }
    public bool SpecialAttack { get { return specialAttack; } }
    public bool Interact { get { return interact; } }
    public bool Dash { get { return dash; } }
    public bool ForcePush { get { return forcePush; } }

    public Vector2 MoveInput { get; protected set; }
    public enum MovementInputDirection { Forward, Backwards, Right, Left, Null }
    public MovementInputDirection MoveInputDirection { get
        {
            if (MoveInput.magnitude == 0)
                return MovementInputDirection.Null;
            Vector2 normalizedInput = MoveInput.normalized;
            if (Mathf.Abs(normalizedInput.y) >= Mathf.Sqrt(3) / 2)
                return normalizedInput.y > 0 ? MovementInputDirection.Forward : MovementInputDirection.Backwards;
            else
                return normalizedInput.x > 0 ? MovementInputDirection.Right : MovementInputDirection.Left;
        } }

    public Vector2 CameraInput { get; protected set; }

}

