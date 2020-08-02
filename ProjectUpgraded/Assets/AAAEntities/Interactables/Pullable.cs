using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pullable : Interactable
{
    public bool IsBeingPulled { get; private set; }
    public bool canInteractWith = false;

    public void Pull(Character interactingCharacter, Vector3 targetButtonPosition, float speed)
    {
        if (IsBeingPulled)
            return;
        StartCoroutine(PullRoutine(interactingCharacter, targetButtonPosition, speed));
    }

    private IEnumerator PullRoutine(Character interactingCharacter, Vector3 targetButtonPosition, float speed)
    {
        IsBeingPulled = true;
        float distance = Vector3.Distance(ButtonPosition, targetButtonPosition);
        Vector3 direction = (targetButtonPosition - ButtonPosition).normalized;

        float timeCounter = distance / speed;
        while(timeCounter > 0)
        {
            MoveObject(direction * speed * Time.deltaTime);
            timeCounter -= Time.deltaTime;
            yield return null;
        }
        IsBeingPulled = false;
        OnPullEnd(interactingCharacter);
    }

    protected abstract void OnPullEnd(Character interactingCharacter);

    protected abstract void MoveObject(Vector3 direction);
}
