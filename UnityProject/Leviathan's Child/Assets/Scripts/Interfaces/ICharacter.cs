using System.Collections;
using UnityEngine;

public interface ICharacter
{
    void AssignLimits();
    void TryMove();
    void CheckIfWalking(float horizontalAxis, float verticalAxis);
    void CheckIfRunning(float horizontalAxis, float verticalAxis);
    void Move(Vector3 movement);
    void SetLookingDirection(float horizontalAxis, float verticalAxis);
    void SetMovingAnimation();
    void SetSpriteLookingDirection();
    void CameraFollow();
    void CheckClick();
    void SetAttackAnimation();
    IEnumerator Attack1Timer(float secondsToWait);
    IEnumerator Attack2Timer(float secondsToWait);
    IEnumerator Attack3Timer(float secondsToWait);
}
