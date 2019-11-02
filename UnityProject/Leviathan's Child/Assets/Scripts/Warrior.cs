﻿using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

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

    public PhotonView pv;

    public string characterName { get; private set; }
    public string job { get; private set; }
    public string user { get; private set; }
    public string element { get; private set; }
    public string amulet { get; private set; }
    public int amuletLevel { get; private set; }
    public int amuletExperience { get; private set; }
    public float strength { get; private set; }
    public float agility { get; private set; }
    public float intelligence { get; private set; }
    public int battlesNumber { get; private set; }
    public int victorysNumber { get; private set; }
    public int losesNumber { get; private set; }
    public int battleTimeInSeconds { get; private set; }
    public int xp { get; private set; }
    public int xpToUp { get; private set; }
    public int level { get; private set; }

    private Animator objectAnimator;

    private Transform objectTransform;
    private Rigidbody2D objectRB;

    public GameObject mainCamera;
    public GameObject myCamera;
    private float rightCameraLimit = 0;
    private float leftCameraLimit = 0;
    private float topCameraLimit = 0;
    private float downCameraLimit = 0;

    public float maxHp = 100f;
    public float hp;

    public float maxStamina = 100f;
    public float stamina;
    public bool exausted = false;

    public float speed = 1f;
    public float runSpeedMultiplier = 2f;
    public float rollSpeedMultiplier = 3f;

    private bool rolling = false;
    private bool running = false;
    private bool walking = false;

    private Action attack1 = new Action();
    private Action attack2 = new Action();
    private Action attack3 = new Action();
    private Action special = new Action();

    private int xLookingTo = 0;
    private int yLookingTo = (int)directions.DOWN;

    private Vector3 smoothMove;

    public void Start()
    {
        if (photonView.IsMine)
        {
            mainCamera = GameObject.Find("Main Camera");
            mainCamera.SetActive(false);
            myCamera.SetActive(true);

            stamina = maxStamina;
            hp = maxHp;

            objectAnimator = GetComponent<Animator>();
            objectTransform = GetComponent<Transform>();
            objectRB = GetComponent<Rigidbody2D>();
            myCamera.transform.position = new Vector3(objectTransform.position.x, objectTransform.position.y, -10f);
            AssignLimits();
        }
    }

    public void AssignLimits()
    {
        rightCameraLimit = GameObject.FindGameObjectWithTag("RightLimit").transform.position.x;
        leftCameraLimit = GameObject.FindGameObjectWithTag("LeftLimit").transform.position.x;
        topCameraLimit = GameObject.FindGameObjectWithTag("TopLimit").transform.position.y;
        downCameraLimit = GameObject.FindGameObjectWithTag("DownLimit").transform.position.y;
    }

    public void Update()
    {
        if (photonView.IsMine)
        {
            TryMove();
            SetMovingAnimation();
            SetSpriteLookingDirection();
            CameraFollow();
            CheckClick();
            SetAttackAnimation();
            SetSpecialAnimation();
            Regein();
        }
        else
        {
            SmoothMovement();
        }
    }

    public void TryMove()
    {
        if (CheckIfAttacking() || special.active)
            return;
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");
        CheckIfRoll();
        if ((horizontalAxis != 0 || verticalAxis != 0) && !rolling)
        {
            walking = true;
            CheckIfRunning();
            Vector3 movement = new Vector3(
            horizontalAxis *
            (speed *
                (rolling
                ? rollSpeedMultiplier
                    : running
                ? runSpeedMultiplier
                    : 1)
            ),
            verticalAxis *
            (speed *
                (rolling
                ? rollSpeedMultiplier
                    : running
                ? runSpeedMultiplier
                    : 1)
            ), 0f);
            Move(movement);
            SetLookingDirection(horizontalAxis, verticalAxis);
        }
        else if (rolling)
        {
            walking = true;
            float movementX = (xLookingTo != 0 ? xLookingTo == (int)directions.RIGHT ? 1 : -1 : 0) * (speed * rollSpeedMultiplier);
            float movementY = (yLookingTo != 0 ? yLookingTo == (int)directions.TOP ? 1 : -1 : 0) * (speed * rollSpeedMultiplier);
            Vector3 movement = new Vector3(movementX, movementY, 0f);
            Move(movement);
        }
        else
            walking = false;
    }

    public bool CheckIfAttacking()
    {
        if (attack1.active && !attack1.finished || attack2.active && !attack2.finished || attack3.active && !attack3.finished)
            return true;
        return false;
    }

    public void CheckIfRoll()
    {
        if (exausted)
            return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (stamina < 10f)
                exausted = true;
            stamina -= 10f;
            StartCoroutine(RollTimer(0.5f));
        }
    }

    public void CheckIfRunning()
    {
        if (stamina < 1f)
            exausted = true;
        if (Input.GetKey(KeyCode.LeftShift) && !exausted)
        {
            running = true;
            walking = false;
        }
        else
        {
            running = false;
            walking = true;
        }
    }

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

    public void SetMovingAnimation()
    {
        if (running)
        {
            objectAnimator.SetBool("running", true);
            objectAnimator.SetBool("walking", false);
        }
        else if (walking)
        {
            objectAnimator.SetBool("walking", true);
            objectAnimator.SetBool("running", false);
        }
        else
        {
            objectAnimator.SetBool("walking", false);
            objectAnimator.SetBool("running", false);
        }
    }

    public void SetSpriteLookingDirection()
    {
        if (xLookingTo == (int)directions.RIGHT)
            transform.localScale = new Vector3(1, 1, 1);
        else if (xLookingTo == (int)directions.LEFT)
            transform.localScale = new Vector3(-1, 1, 1);
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

    public void CheckClick()
    {
        if (Input.GetMouseButton(1))
            Special();
        else if (Input.GetMouseButtonDown(0))
            Attack();

        if (!Input.GetMouseButton(1))
        {
            special.active = false;
        }
    }

    public void Special()
    {
        if (!exausted)
        {
            special.active = true;
        }
    }

    public void Attack()
    {
        if (attack1.active && !attack2.active && !exausted)
        {
            if (stamina < 2f)
                exausted = true;
            stamina -= 2f;
            StartCoroutine(Attack2Timer(0.75f));
        }
        else if (attack2.active && !attack3.active && !exausted)
        {
            if (stamina < 10f)
                exausted = true;
            stamina -= 10f;

            StartCoroutine(Attack3Timer(0.75f));
        }
        else if (!attack1.active)
        {
            StartCoroutine(Attack1Timer(0.75f));
        }
    }

    public void SetAttackAnimation()
    {
        objectAnimator.SetBool("attack1", attack1.active && !attack1.finished);
        objectAnimator.SetBool("attack2", attack2.active && !attack2.finished);
        objectAnimator.SetBool("attack3", attack3.active && !attack3.finished);
    }

    public void SetSpecialAnimation()
    {
        objectAnimator.SetBool("special", special.active);
    }

    public void Regein()
    {
        if (ValidToRegein())
        {
            stamina += 0.1f;
            if (exausted && stamina > 10f) exausted = false;
        }
        Debug.Log(stamina);
    }

    public bool ValidToRegein()
    {
        if (stamina < maxStamina && !running && !rolling && !CheckIfAttacking())
            return true;
        return false;
    }

    private void SmoothMovement()
    {
        transform.position = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 10);
    }

    public IEnumerator RollTimer(float secondsToWait)
    {
        rolling = true;
        yield return new WaitForSeconds(secondsToWait);
        rolling = false;
    }
    public IEnumerator Attack1Timer(float secondsToWait)
    {
        attack1.active = true;
        attack1.finished = false;
        yield return new WaitForSeconds(secondsToWait);
        attack1.active = false;
    }
    public IEnumerator Attack2Timer(float secondsToWait)
    {
        attack2.active = true;
        attack2.finished = false;
        yield return new WaitForSeconds(secondsToWait);
        attack2.active = false;
    }
    public IEnumerator Attack3Timer(float secondsToWait)
    {
        attack3.active = true;
        attack3.finished = false;
        yield return new WaitForSeconds(secondsToWait);
        attack3.active = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else if (stream.IsReading)
        {
            smoothMove = (Vector3)stream.ReceiveNext();
        }
    }
}
