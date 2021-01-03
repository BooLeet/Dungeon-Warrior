using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlBldrLiteDoor : Interactable
{
    public enum DoorState { Closed, Open, KeyRequired, Locked };
    public DoorState doorState;
    public Transform doorTransform;
    public LootScriptable keyLoot;

    private float closedDoorRotation;
    private float openDoorRotation;
    private float targetDoorRotation;

    private float animationTime = 0.5f;
    private bool animationPlaying = false;

    private void Start()
    {
        closedDoorRotation = doorTransform.rotation.eulerAngles.y;
        openDoorRotation = closedDoorRotation - 90;
        targetDoorRotation = closedDoorRotation;
    }


    public override string GetPrompt(Character interactingCharacter)
    {
        if (doorState == DoorState.Open)
            return GameManager.instance.languagePack.GetString("doorClose");
        else
        {
            if (doorState == DoorState.Locked)
                return GameManager.instance.languagePack.GetString("doorLocked");
            else if (doorState == DoorState.KeyRequired)
            {
                if (interactingCharacter.inventory.HasLoot(keyLoot))
                    return GameManager.instance.languagePack.GetString("doorUnlock");
                else
                    return GameManager.instance.languagePack.GetString("doorKeyRequired");
            }

            return GameManager.instance.languagePack.GetString("doorOpen");
        }
    }

    protected override void _Interact(Character interactingCharacter)
    {
        OpenClose(interactingCharacter);
    }

    private void OpenClose(Character interactingCharacter)
    {
        if (animationPlaying)
            return;

        if (doorState == DoorState.Open)
        {
            doorState = DoorState.Closed;
            targetDoorRotation = closedDoorRotation;
            StartCoroutine(ApplyAnimation());
        }
        else if (doorState == DoorState.Closed || doorState == DoorState.KeyRequired && interactingCharacter.inventory.HasResource(keyLoot))
        {
            interactingCharacter.inventory.SpendResource(keyLoot);
            doorState = DoorState.Open;
            targetDoorRotation = openDoorRotation;
            StartCoroutine(ApplyAnimation());
        }
    }

    private IEnumerator ApplyAnimation()
    {
        animationPlaying = true;

        float timePassed = 0;
        Quaternion targetRotation = Quaternion.Euler(0, targetDoorRotation, 0);
        Quaternion startingRotation = doorTransform.localRotation;
        while (timePassed < animationTime)
        {
            timePassed += Time.deltaTime;
            doorTransform.localRotation = Quaternion.Lerp(startingRotation, targetRotation, timePassed / animationTime);
            yield return new WaitForEndOfFrame();
        }
        doorTransform.localRotation = targetRotation;
        animationPlaying = false;
    }
}
