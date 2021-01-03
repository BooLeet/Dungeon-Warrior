using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MobileInputElement : MonoBehaviour
{
    public abstract bool HandleTouch(Touch touch);

    public abstract bool IsInside(Vector2 position);
}
