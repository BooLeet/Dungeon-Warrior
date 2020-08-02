using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullableCharacter : Pullable
{
    public Character character;

    public override string GetPrompt(Character interactingCharacter)
    {
        return "";
    }

    public override void Interact(Character interactingCharacter)
    {
        
    }

    protected override void MoveObject(Vector3 direction)
    {
        character.controller.Move(direction);
    }

    protected override void OnPullEnd(Character interactingCharacter) { }
}
