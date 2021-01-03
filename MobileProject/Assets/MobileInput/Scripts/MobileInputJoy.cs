using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInputJoy : MobileInputElement
{
    public float diameter;
    public float handleMaxDiameter;
    public RectTransform handle;
    int fingerId = -1;

    private Vector2 previousTouchPosition;

    public Vector2 Input { get; private set; }
    public Vector2 DeltaInput { get; private set; }


    public bool IsPressed { get; private set; }

    public override bool HandleTouch(Touch touch)
    {
        bool result = false;
        switch(touch.phase)
        {
            case TouchPhase.Began:
                if (fingerId == -1 && IsInside(touch.position))
                {
                    previousTouchPosition = touch.position;
                    fingerId = touch.fingerId;
                    result = true;
                }
                break;
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if(fingerId == touch.fingerId)
                    result = true;
                break;
            case TouchPhase.Ended:
                if(touch.fingerId == fingerId)
                    fingerId = -1;
                break;
        }
        if (fingerId == -1)
        {
            DeltaInput = Input = Vector2.zero;
        }
        else if(fingerId == touch.fingerId)
        {
            DeltaInput = touch.position - previousTouchPosition;
            previousTouchPosition = touch.position;
            Input = (touch.position - GetCenterPosition()) / GetRadius();
            if (Input.magnitude > 1)
                Input = Input.normalized;
        }

        IsPressed = fingerId != -1;
        Vector2 handlePosition = Input * GetRadius();
        if (handlePosition.magnitude > handleMaxDiameter)
            handlePosition = handlePosition.normalized * handleMaxDiameter;
        handle.anchoredPosition = handlePosition;
        return result;
    }


    private float GetRadius()
    {
        return diameter / 2;
    }

    private Vector2 GetCenterPosition()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    public override bool IsInside(Vector2 position)
    {
        return Vector2.Distance(position, GetCenterPosition()) <= GetRadius();
    }
}
