using System.Collections;
using System.Collections.Generic;
using System.IO;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


//classe auxiliar para controle das ações
public class Action
{
    public bool active = false;
    public bool finished = false;

    public void Reset()
    {
        active = false;
        finished = false;
    }
}

public class Warrior : MonoBehaviourPunCallbacks, ICharacter, IPunObservable
{
    enum directions
    {
        TOP = 1,
        DOWN = 2,
        RIGHT = 3,
        LEFT = 4
    }
    //objetos associados
    public PhotonView pv;

    [SerializeField]
    private Animator objectAnimator;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Transform objectTransform;
    [SerializeField]
    private Rigidbody2D objectRB;

    [SerializeField]
    private CharacterUIController characterUIController;

    private Transform attack1AreaTransform;
    private Transform attack2AreaTransform;
    private Transform attack3AreaTransform;

    //childrens prefabs
    [SerializeField]
    private GameObject attack1AreaPrefab;
    [SerializeField]
    private GameObject attack2AreaPrefab;
    [SerializeField]
    private GameObject attack3AreaPrefab;

    //controle de câmera
    public GameObject myCamera;
    private float rightCameraLimit = 0;
    private float leftCameraLimit = 0;
    private float topCameraLimit = 0;
    private float downCameraLimit = 0;

    //controle de jogabilidade
    [HideInInspector]
    public float damage;

    [HideInInspector]
    public float maxHp;
    [HideInInspector]
    public float hp;

    [HideInInspector]
    public float maxStamina;
    [HideInInspector]
    public float stamina;
    [HideInInspector]
    public bool exausted = false;

    [HideInInspector]
    public float speed;
    [HideInInspector]
    public float runSpeedMultiplier = 2f;

    [HideInInspector]
    private bool running = false;
    [HideInInspector]
    private bool walking = false;

    private Action attack1 = new Action();
    private Action attack2 = new Action();
    private Action attack3 = new Action();
    private Action special = new Action();
    private bool attackInCooldown = false;
    public bool takingDamage = false;
    public bool dead = false;

    private int xLookingTo = 0;
    private int yLookingTo = (int)directions.DOWN;

    //variáveis de ajuste e controle
    [HideInInspector]
    public int playerNumber;

    private Vector3 smoothMove;

    private bool configured = false;

    private float StrengthMultiplierToStamina = 0.3f;
    private float AgilityMultiplierToStamina = 0.1f;
    private float StrengthMultiplierToDamage = 0.5f;
    private float AgilityMultiplierToDamage = 0.2f;
    private float StrengthMultiplierToSpeed = 0.1f;
    private float AgilityMultiplierToSpeed = 0.3f;

    public void Start()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(Setup());
        }
    }

    IEnumerator Setup()
    {
        yield return new WaitForSeconds(1);
        myCamera.SetActive(true);

        myCamera.transform.position = new Vector3(objectTransform.position.x, objectTransform.position.y, -10f);
        AssignLimits();
        pv.RPC("RPCSetPlayerNumber", RpcTarget.AllBuffered, PhotonRoom.instance.myNumberInRoom);
        yield return new WaitForSeconds(1);
        pv.RPC("RPCConfigurePlayer", RpcTarget.AllBuffered, playerNumber);

        Character selectedCharacterInfos = GameController.instance.selectedCharacter;
        pv.RPC("RPCConfigurePlayerGameplayProperties", RpcTarget.AllBuffered, selectedCharacterInfos.hp, selectedCharacterInfos.strength, selectedCharacterInfos.agility, selectedCharacterInfos.intelligence);

        StartCoroutine(SendMoveToAvoidTransformBug());

        yield return new WaitForSeconds(1);
        configured = true;
    }

    public void AssignLimits()
    {
        rightCameraLimit = GameObject.FindGameObjectWithTag("RightLimit").transform.position.x;
        leftCameraLimit = GameObject.FindGameObjectWithTag("LeftLimit").transform.position.x;
        topCameraLimit = GameObject.FindGameObjectWithTag("TopLimit").transform.position.y;
        downCameraLimit = GameObject.FindGameObjectWithTag("DownLimit").transform.position.y;
    }

    [PunRPC]
    public void RPCSetPlayerNumber(int number) => playerNumber = number;

    [PunRPC]
    private void RPCConfigurePlayer(int number)
    {
        gameObject.tag = "Player" + number;
        gameObject.name = "Player" + number;
        characterUIController.nameText.text = GameController.instance.selectedCharacter.name;

        attack1AreaTransform = Instantiate(attack1AreaPrefab, transform).GetComponent<Transform>();
        attack1AreaTransform.gameObject.GetComponent<Attack1Area>().parentScript = gameObject.GetComponent<Warrior>();
        attack2AreaTransform = Instantiate(attack2AreaPrefab, transform).GetComponent<Transform>();
        attack2AreaTransform.gameObject.GetComponent<Attack2Area>().parentScript = gameObject.GetComponent<Warrior>();
        attack3AreaTransform = Instantiate(attack3AreaPrefab, transform).GetComponent<Transform>();
        attack3AreaTransform.gameObject.GetComponent<Attack3Area>().parentScript = gameObject.GetComponent<Warrior>();

        if (number == 1)
        {
            MatchController.instance.player1 = gameObject;
            MatchController.instance.player1Job = "Guerreiro";
            xLookingTo = (int)directions.RIGHT;
        }
        else
        {
            MatchController.instance.player2 = gameObject;
            MatchController.instance.player2Job = "Guerreiro";
            xLookingTo = (int)directions.LEFT;
        }
        SetSpriteLookingDirection();
    }

    [PunRPC]
    private void RPCConfigurePlayerGameplayProperties(float hp, float strength, float agility, float intelligence)
    {
        maxHp = hp;
        maxStamina = float.Parse((100 + (StrengthMultiplierToStamina * strength) + (AgilityMultiplierToStamina * agility)).ToString());
        damage = float.Parse((((StrengthMultiplierToDamage * strength) + (AgilityMultiplierToDamage * agility) / 2) / 3).ToString());
        speed = float.Parse((((StrengthMultiplierToSpeed * strength) + (AgilityMultiplierToSpeed * agility)) / 5).ToString());

        stamina = maxStamina;
        this.hp = maxHp;
    }

    IEnumerator SendMoveToAvoidTransformBug()
    {
        yield return new WaitForSeconds(5);
        Move(new Vector3(Mathf.Epsilon, Mathf.Epsilon, 0f));
        StartCoroutine(SendMoveToAvoidTransformBug());
        yield break;
    }

    public void Update()
    {
        if (!configured) return;

        if (photonView.IsMine)
        {
            TryMove();
            SetSpriteLookingDirection();
            CameraFollow();
            CheckIfUsingSpecial();
            CheckIfAttack();
            Regein();
            UpdateCharacterUI();
            ConfigureAnimations();
            CheckIfIsDead();
        }
        else
        {
            SmoothMovement();
        }
    }

    public void TryMove()
    {
        if (IsAttacking() || special.active || takingDamage || dead || MatchController.instance.matchFinished)
            return;

        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        IsRunning(horizontalAxis, verticalAxis);
        IsWalking(horizontalAxis, verticalAxis);

        if ((horizontalAxis != 0 || verticalAxis != 0))
        {
            Vector3 movement = new Vector3(
                horizontalAxis * (speed * (running ? runSpeedMultiplier : 1)),
                verticalAxis * (speed * (running ? runSpeedMultiplier : 1)), 0f
            );
            Move(movement);
            SetLookingDirection(horizontalAxis, verticalAxis);
        }
    }

    public bool IsAttacking()
    {
        if (attack1.active && !attack1.finished || attack2.active && !attack2.finished || attack3.active && !attack3.finished)
            return true;
        return false;
    }

    public void IsRunning(float horizontalAxis, float verticalAxis)
    {
        if (running && stamina < 1f)
            exausted = true;

        running = Input.GetKey(KeyCode.LeftShift) && !exausted && (horizontalAxis != 0 || verticalAxis != 0) ? true : false;
    }

    public void IsWalking(float horizontalAxis, float verticalAxis) =>
       walking = !running && (horizontalAxis != 0 || verticalAxis != 0) ? true : false;

    public void Move(Vector3 movement)
    {
        if (running)
            stamina -= 0.1f;
        objectRB.MovePosition(objectTransform.position + (movement * Time.deltaTime));
    }

    public void SetLookingDirection(float horizontalAxis, float verticalAxis)
    {
        xLookingTo = horizontalAxis != 0
                        ? horizontalAxis > 0
                            ? (int)directions.RIGHT
                            : (int)directions.LEFT
                            : 0;

        yLookingTo = verticalAxis != 0
                            ? verticalAxis > 0
                                ? (int)directions.TOP
                                : (int)directions.DOWN
                            : 0;
    }

    public void SetSpriteLookingDirection()
    {
        if (spriteRenderer.flipX != false && xLookingTo == (int)directions.RIGHT)
        {
            pv.RPC("Rpc_SetSpriteLookingDirection", RpcTarget.AllBuffered, false);
            pv.RPC("Rpc_SetAttackAreaDirection", RpcTarget.AllBuffered, false);
        }
        else if (spriteRenderer.flipX == false && xLookingTo == (int)directions.LEFT)
        {
            pv.RPC("Rpc_SetSpriteLookingDirection", RpcTarget.AllBuffered, true);
            pv.RPC("Rpc_SetAttackAreaDirection", RpcTarget.AllBuffered, true);
        }
    }

    [PunRPC]
    public void Rpc_SetSpriteLookingDirection(bool right) =>
        spriteRenderer.flipX = right ? true : false;

    [PunRPC]
    public void Rpc_SetAttackAreaDirection(bool left)
    {
        attack1AreaTransform.localScale = new Vector3(left ? -1 : 1, 1, 1);
        attack2AreaTransform.localScale = new Vector3(left ? -1 : 1, 1, 1);
        attack3AreaTransform.localScale = new Vector3(left ? -1 : 1, 1, 1);
    }

    public void CameraFollow()
    {
        if (myCamera.transform == null)
            return;

        float cameraPositionX = myCamera.transform.position.x;
        float cameraPositionY = myCamera.transform.position.y;
        float playerPositionX = objectTransform.position.x;
        float playerPositionY = objectTransform.position.y;
        float newCameraPositionX = playerPositionX <= rightCameraLimit && playerPositionX >= leftCameraLimit ? playerPositionX : cameraPositionX;
        float newCameraPostionY = playerPositionY <= topCameraLimit && playerPositionY >= downCameraLimit ? playerPositionY : cameraPositionY;

        myCamera.transform.position = new Vector3(newCameraPositionX, newCameraPostionY, -10);
    }

    public void CheckIfUsingSpecial() => special.active = !exausted && !dead && !MatchController.instance.matchFinished && Input.GetMouseButton(1) ? true : false;

    public void CheckIfAttack()
    {
        if (Input.GetMouseButtonDown(0) && !(dead || MatchController.instance.matchFinished) && !takingDamage)
            Attack();
    }

    public void Attack()
    {
        if (attackInCooldown || special.active)
            return;

        if (attack1.active && !attack2.active && !exausted)
        {
            if (stamina < 2f)
                exausted = true;
            stamina -= 2f;
            StartCoroutine(Attack2Timer(0.75f));
            StartCoroutine(AttackCooldown(0.5f));

        }
        else if (attack2.active && !attack3.active && !exausted)
        {
            if (stamina < 10f)
                exausted = true;
            stamina -= 10f;
            StartCoroutine(Attack3Timer(0.75f));
            StartCoroutine(AttackCooldown(0.75f));
        }
        else if (!attack1.active)
        {
            StartCoroutine(Attack1Timer(0.75f));
            StartCoroutine(AttackCooldown(0.5f));
        }
    }

    public IEnumerator Attack1Timer(float secondsToWait)
    {
        attack1.active = true;
        attack1.finished = false;
        yield return new WaitForSeconds(secondsToWait / 2);
        pv.RPC("RPCActiveOrInactiveAttackArea", RpcTarget.All, 1, true);
        yield return new WaitForSeconds(secondsToWait / 2);
        pv.RPC("RPCActiveOrInactiveAttackArea", RpcTarget.All, 1, false);
        attack1.active = false;
    }
    public IEnumerator Attack2Timer(float secondsToWait)
    {
        attack2.active = true;
        attack2.finished = false;
        yield return new WaitForSeconds(secondsToWait / 2);
        pv.RPC("RPCActiveOrInactiveAttackArea", RpcTarget.All, 2, true);
        yield return new WaitForSeconds(secondsToWait / 2);
        pv.RPC("RPCActiveOrInactiveAttackArea", RpcTarget.All, 2, false);
        attack2.active = false;
    }
    public IEnumerator Attack3Timer(float secondsToWait)
    {
        attack3.active = true;
        attack3.finished = false;
        yield return new WaitForSeconds(secondsToWait / 2);
        pv.RPC("RPCActiveOrInactiveAttackArea", RpcTarget.All, 3, true);
        yield return new WaitForSeconds(secondsToWait / 2);
        pv.RPC("RPCActiveOrInactiveAttackArea", RpcTarget.All, 3, false);
        attack3.active = false;
    }

    [PunRPC]
    public void RPCActiveOrInactiveAttackArea(int attackNumber, bool value)
    {
        switch (attackNumber)
        {
            case 1:
                attack1AreaTransform.gameObject.SetActive(value);
                break;
            case 2:
                attack2AreaTransform.gameObject.SetActive(value);
                break;
            case 3:
                attack3AreaTransform.gameObject.SetActive(value);
                break;
        }
    }

    public IEnumerator AttackCooldown(float secondsToWait)
    {
        attackInCooldown = true;
        yield return new WaitForSeconds(secondsToWait);
        attackInCooldown = false;
    }

    public void UpdateCharacterUI()
    {
        characterUIController.staminaBarImage.fillAmount = stamina / maxStamina;
        characterUIController.lifeBarImage.fillAmount = hp / maxHp;
    }

    public void ConfigureAnimations()
    {
        SetAnimationToIdleIfNeed();
        SetMovingAnimation();
        SetAttackAnimation();
        SetSpecialAnimation();
        SetTakingDamageAnimation();
    }

    public void SetAnimationToIdleIfNeed()
    {
        if ((!walking && !running && !attack1.active && !attack2.active && !attack3.active && !special.active && !takingDamage) || (MatchController.instance.matchFinished && !dead))
            SetAnimationToIdle();
    }

    public void SetAnimationToIdle()
    {
        objectAnimator.SetBool("idle", true);
        objectAnimator.SetBool("running", false);
        objectAnimator.SetBool("walking", false);
        objectAnimator.SetBool("attack1", false);
        objectAnimator.SetBool("attack2", false);
        objectAnimator.SetBool("attack3", false);
        objectAnimator.SetBool("special", false);
        objectAnimator.SetBool("takingDamage", false);
    }

    public void SetMovingAnimation()
    {
        objectAnimator.SetBool("running", running);
        objectAnimator.SetBool("walking", walking);
    }

    public void SetAttackAnimation()
    {
        objectAnimator.SetBool("attack1", attack1.active && !attack1.finished);
        objectAnimator.SetBool("attack2", attack2.active && !attack2.finished);
        objectAnimator.SetBool("attack3", attack3.active && !attack3.finished);
    }

    public void SetSpecialAnimation() => objectAnimator.SetBool("special", special.active);

    public void SetTakingDamageAnimation() => objectAnimator.SetBool("takingDamage", takingDamage);

    public void CheckIfIsDead()
    {
        if (dead || MatchController.instance.matchFinished)
            return;

        if (hp <= 0)
        {
            dead = true;
            pv.RPC("RPCFinishMatch", RpcTarget.All, playerNumber == 1 ? 2 : 1);
            objectAnimator.SetBool("dead", dead);
        }
    }

    [PunRPC]
    public void RPCFinishMatch(int winnerNumber) => MatchController.instance.FinishMatch(winnerNumber);

    public void Regein()
    {
        if (ValidToRegein())
        {
            stamina += maxStamina / 1000;
            if (exausted && stamina > 10f) exausted = false;
        }
    }

    public bool ValidToRegein() => stamina < maxStamina && !running && !IsAttacking() ? true : false;

    public void GiveDamageForAttack1()
    {
        GameObject enemy = playerNumber == 1 ? MatchController.instance.player2 : MatchController.instance.player1;
        ICharacter enemyScript = enemy.GetComponent<ICharacter>();

        pv.RPC("RPCGiveDamageToEnemy", RpcTarget.AllBuffered, playerNumber == 1 ? 2 : 1, Random.Range((damage / 2), (damage * 1.2f)));
    }

    public void GiveDamageForAttack2()
    {
        GameObject enemy = playerNumber == 1 ? MatchController.instance.player2 : MatchController.instance.player1;
        ICharacter enemyScript = enemy.GetComponent<ICharacter>();

        pv.RPC("RPCGiveDamageToEnemy", RpcTarget.AllBuffered, playerNumber == 1 ? 2 : 1, Random.Range((damage * 2), (damage * 1.4f)));
    }

    public void GiveDamageForAttack3()
    {
        GameObject enemy = playerNumber == 1 ? MatchController.instance.player2 : MatchController.instance.player1;
        ICharacter enemyScript = enemy.GetComponent<ICharacter>();

        pv.RPC("RPCGiveDamageToEnemy", RpcTarget.AllBuffered, playerNumber == 1 ? 2 : 1, Random.Range((damage * 2), (damage * 1.6f)));
    }

    [PunRPC]
    public void RPCGiveDamageToEnemy(int enemyNumber, float damage)
    {
        GameObject enemy = enemyNumber == 1 ? MatchController.instance.player1 : MatchController.instance.player2;
        enemy.GetComponent<ICharacter>().TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        if (special.active && !exausted)
        {
            stamina -= damage * 4;
            if (stamina < 10f) exausted = true;
            return;
        }

        StartCoroutine(TakingDamageTimer(0.45f));

        hp = hp - damage;
    }

    public IEnumerator TakingDamageTimer(float secondsToWait)
    {
        takingDamage = true;
        yield return new WaitForSeconds(secondsToWait);
        takingDamage = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(transform.position);
        else if (stream.IsReading)
            smoothMove = (Vector3)stream.ReceiveNext();
    }

    private void SmoothMovement() => transform.position = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 10);

}
