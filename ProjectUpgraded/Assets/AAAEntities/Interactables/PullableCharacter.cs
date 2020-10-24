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

    protected override void _Interact(Character interactingCharacter)
    {
        
    }

    protected override void MoveObject(Vector3 direction)
    {
        character.JustMove(direction);
    }

    protected override void OnPullEnd(Character interactingCharacter) { }
}
