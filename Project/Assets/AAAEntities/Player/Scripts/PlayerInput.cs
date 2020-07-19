using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInput : MonoBehaviour
{
    public bool Attack { get; protected set; }
    public bool Zoom { get; protected set; }
    public bool Interact { get; protected set; }
    public bool Jump { get; protected set; }
    public bool Dash { get; protected set; }
    public bool Melee { get; protected set; }

    public Vector2 MoveInput { get; protected set; }
    public Vector2 CameraInput { get; protected set; }

    private bool _WeaponChanged = false;
    public bool WeaponChanged { get { if (_WeaponChanged) { _WeaponChanged = false; return true; } else return false; } protected set { _WeaponChanged = value; } }
    public int WeaponIndex { get; protected set; }
}

