using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pullable : Interactable
{
    public bool IsBeingPulled { get; private set; }

    public override string GetPrompt(Character interactingCharacter)
    {
        return "_PULL_";
    }

    public void Pull(Vector3 targetButtonPosition, float speed)
    {
        if (IsBeingPulled)
            return;
        StartCoroutine(PullRoutine(targetButtonPosition, speed));
    }

    private IEnumerator PullRoutine(Vector3 targetButtonPosition, float speed)
    {
        IsBeingPulled = true;
        float distance = Vector3.Distance(ButtonPosition, targetButtonPosition);
        Vector3 direction = (targetButtonPosition - ButtonPosition).normalized;

        float timeCounter = distance / speed;
        while(timeCounter > 0)
        {
            transform.Translate(direction * speed * Time.deltaTime, null);
            timeCounter -= Time.deltaTime;
            yield return null;
        }
        IsBeingPulled = false;
    }

    public override void Interact(Character interactingCharacter)
    {
        
    }
}
