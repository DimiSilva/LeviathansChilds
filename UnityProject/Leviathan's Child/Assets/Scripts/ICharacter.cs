using System.Collections;
using UnityEngine;

public interface ICharacter
{
    void AssignLimits();
    void TryMove();
    void CheckIfRoll();
    void CheckIfRunning();
    void Move(Vector3 movement);
    void SetLookingDirection(float horizontalAxis, float verticalAxis);
    void SetMovingAnimation();
    void SetSpriteLookingDirection();
    void CameraFollow();
    void CheckClick();
    void SetAttackAnimation();
    IEnumerator RollTimer(float secondsToWait);
    IEnumerator Attack1Timer(float secondsToWait);
    IEnumerator Attack2Timer(float secondsToWait);
    IEnumerator Attack3Timer(float secondsToWait);
}
