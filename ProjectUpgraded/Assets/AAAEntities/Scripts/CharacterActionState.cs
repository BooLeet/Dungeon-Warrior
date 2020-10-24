using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterActionState
{ 
    public abstract void Init(Character character);
    public abstract void Action(Character character);
    public abstract CharacterActionState Transition(Character character);

}
