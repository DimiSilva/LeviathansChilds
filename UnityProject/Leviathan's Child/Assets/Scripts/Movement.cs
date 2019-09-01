using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    enum directions
    {
        TOP = 1,
        DOWN = 2,
        RIGHT = 3,
        LEFT = 4
    }

    private Transform objectTransform;
    private Rigidbody2D objectRB;

    private Transform CameraTransform;
    private float rightCameraLimit = 0;
    private float leftCameraLimit = 0;
    private float topCameraLimit = 0;
    private float downCameraLimit = 0;

    public float speed = 5f;
    public float runSpeedMultiplier = 1.4f;
    public float rollSpeedMultiplier = 1.7f;

    private bool rolling = false;
    private bool running = false;

    private int xLookingTo = 0;
    private int yLookingTo = (int)directions.DOWN;


    void Start()
    {
        objectTransform = GetComponent<Transform>();
        objectRB = GetComponent<Rigidbody2D>();
        CameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        CameraTransform.position = new Vector3(objectTransform.position.x, objectTransform.position.y, -10f);
        AssignLimits();
    }

    void Update()
    {
        TryMove();
        CameraFollow();
    }

    private void AssignLimits()
    {
        rightCameraLimit = GameObject.FindGameObjectWithTag("RightLimit").transform.position.x;
        leftCameraLimit = GameObject.FindGameObjectWithTag("LeftLimit").transform.position.x;
        topCameraLimit = GameObject.FindGameObjectWithTag("TopLimit").transform.position.y;
        downCameraLimit = GameObject.FindGameObjectWithTag("DownLimit").transform.position.y;
    }

    private void CameraFollow()
    {
        if (CameraTransform == null)
            return;

        float cameraPositionX = CameraTransform.position.x;
        float cameraPositionY = CameraTransform.position.y;
        float playerPositionX = objectTransform.position.x;
        float playerPositionY = objectTransform.position.y;
        float newCameraPositionX = playerPositionX <= rightCameraLimit && playerPositionX >= leftCameraLimit ? playerPositionX : cameraPositionX;
        float newCameraPostionY = playerPositionY <= topCameraLimit && playerPositionY >= downCameraLimit ? playerPositionY : cameraPositionY;

        CameraTransform.position = new Vector3(newCameraPositionX, newCameraPostionY, -10);
    }

    private void Move(Vector3 movement)
    {
        objectRB.MovePosition(objectTransform.position + (movement * Time.deltaTime));
    }

    private void TryMove()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");
        CheckIfRoll();
        if ((horizontalAxis != 0 || verticalAxis != 0) && !rolling)
        {
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
        else if (rolling)
        {
            float movementX = (xLookingTo != 0 ? xLookingTo == (int)directions.RIGHT ? 1 : -1 : 0) * (speed * rollSpeedMultiplier);
            float movementY = (yLookingTo != 0 ? yLookingTo == (int)directions.TOP ? 1 : -1 : 0) * (speed * rollSpeedMultiplier);
            Vector3 movement = new Vector3(movementX, movementY, 0f);
            Move(movement);
        }
    }

    private void CheckIfRoll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(RollTimer(0.5f));
    }

    private void CheckIfRunning()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            running = true;
        else
            running = false;
    }

    private IEnumerator RollTimer(float secondsToWait)
    {
        rolling = true;
        yield return new WaitForSeconds(secondsToWait);
        rolling = false;
    }
}
