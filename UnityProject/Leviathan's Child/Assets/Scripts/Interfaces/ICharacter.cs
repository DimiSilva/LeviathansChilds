using System.Collections;
using UnityEngine;

public interface ICharacter
{
    void AssignLimits();
    void TryMove();
    void IsWalking(float horizontalAxis, float verticalAxis);
    void IsRunning(float horizontalAxis, float verticalAxis);
    void Move(Vector3 movement);
    void SetLookingDirection(float horizontalAxis, float verticalAxis);
    void SetMovingAnimation();
    void SetSpriteLookingDirection();
    void CameraFollow();
    void SetAttackAnimation();
    void GiveDamageForAttack1();
    void TakeDamage(float damage);
    // float GiveDamageForAttack2();
    // float GiveDamageForAttack3();
    IEnumerator Attack1Timer(float secondsToWait);
    IEnumerator Attack2Timer(float secondsToWait);
    IEnumerator Attack3Timer(float secondsToWait);
}
