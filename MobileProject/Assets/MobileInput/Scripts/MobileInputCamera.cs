using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInputCamera : MobileInputElement
{
    private int fingerId = -1;
    private Vector2 previousTouchPosition;
    public Vector2 DeltaInput { get; private set; }

    public override bool HandleTouch(Touch touch)
    {
        bool result = false;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (fingerId == -1)
                {
                    previousTouchPosition = touch.position;
                    fingerId = touch.fingerId;
                    result = true;
                }
                break;
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (fingerId == touch.fingerId)
                    result = true;
                break;
            case TouchPhase.Ended:
                if (touch.fingerId == fingerId)
                    fingerId = -1;
                break;
        }

        if (fingerId == -1)
        {
            DeltaInput = Vector2.zero;
        }
        else if (fingerId == touch.fingerId)
        {
            DeltaInput = touch.position - previousTouchPosition;
            previousTouchPosition = touch.position;
        }

        return result;
    }

    public override bool IsInside(Vector2 position)
    {
        return true;
    }
}
