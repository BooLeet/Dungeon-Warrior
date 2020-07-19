using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUD_DamageIndicator : MonoBehaviour {
    public Transform transformToRotate;
    private Vector3 damageSource;
    public float lifeTime = 1;
    public RawImage directionIndicatorImage,damageIndicatorImage;
    private PlayerCharacter player;

    public void StartEffect(Vector3 damageSource, PlayerCharacter player)
    {
        this.damageSource = damageSource;
        this.player = player;
        StartCoroutine(EffectRoutine());
        Destroy(gameObject, lifeTime);
    }

    private IEnumerator EffectRoutine()
    {
        float currentTime = 0;
        while(true)
        {
            Color newColor = directionIndicatorImage.color;
            newColor.a = 1 - currentTime / lifeTime;
            directionIndicatorImage.color = damageIndicatorImage.color = newColor;

            damageSource.y = player.transform.position.y;
            Vector3 playerForward = player.playerCamera.CameraForward;
            Vector3 direction = (damageSource - player.transform.position).normalized;
            float angle = Utility.AngleBetweenTwoVectors(playerForward, direction);
            if ((direction - player.playerCamera.CameraRight).magnitude < (direction + player.playerCamera.CameraRight).magnitude)
                angle *= -1;
            transformToRotate.localRotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);

            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

}
