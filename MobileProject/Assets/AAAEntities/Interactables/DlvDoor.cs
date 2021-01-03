using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DlvDoor : MonoBehaviour
{
    private bool isOpen = false;
    private PlayerCharacter player;
    public Transform doorTransform;
    public float targetDoorRotation = 90;
    public float animationTime = 0.5f;

    private bool playerWasClose = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (isOpen)
            return;

        if (player == null)
            player = PlayerCharacter.instance;

        if (player == null)
            return;

        bool playerIsClose = Vector3.Distance(player.Position, transform.position) <= 50;
        if (playerIsClose && playerWasClose && AIDirector.GetInstance().GetClosestAI(transform.position, 100).Count() == 0)
        {
            isOpen = true;
            StartCoroutine(ApplyAnimation());
        }

        playerWasClose = playerIsClose;
    }

    private IEnumerator ApplyAnimation()
    {
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
    }
}
