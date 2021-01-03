using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInputButton : MobileInputElement
{
    public float diameter;
    public RectTransform pressEffectTransform;
    private int fingerId = -1;
    public bool KeyPressed { get; private set; }
    public bool KeyDown { get; private set; }
    public bool KeyUp { get; private set; }

    public override bool HandleTouch(Touch touch)
    {
        bool result = false;
        KeyDown = false;
        KeyUp = false;
        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (fingerId == -1 && IsInside(touch.position))
                {
                    fingerId = touch.fingerId;
                    KeyDown = true;
                    KeyPressed = true;
                    result = true;
                }
                break;
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (fingerId == touch.fingerId)
                    result = true;
                break;
            case TouchPhase.Ended: 
            case TouchPhase.Canceled:
                if(fingerId == touch.fingerId)
                {
                    fingerId = -1;
                    KeyUp = true;
                    KeyPressed = false;
                }
                break;
        }
        if (pressEffectTransform) 
            pressEffectTransform.localScale = Vector3.one * (KeyPressed ? 0.9f : 1);
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
