using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class player : MonoBehaviourPun, IPunObservable
{
    public PhotonView pv;

    public float moveSpeed = 10;
    public float jumpForce = 800;

    public Vector3 smoothMove;

    private GameObject mainCamera;
    public GameObject myCamera;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (photonView.IsMine)
        {
            mainCamera = GameObject.Find("Main Camera");
            mainCamera.SetActive(false);
            myCamera.SetActive(true);
        }

    }

    void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInputs();
        }
        else
        {
            SmoothMovement();
        }
    }

    private void SmoothMovement()
    {
        transform.position = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 10);
    }

    private void ProcessInputs()
    {
        var move = new Vector3(Input.GetAxisRaw("Horizontal"), 0);
        transform.position += move * moveSpeed * Time.deltaTime;

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            sr.flipX = false;
            pv.RPC("OnDirectionChange_RIGHT", RpcTarget.Others);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            sr.flipX = true;
            pv.RPC("OnDirectionChange_LEFT", RpcTarget.Others);
        }
    }

    [PunRPC]
    public void OnDirectionChange_LEFT()
    {
        sr.flipX = true;
    }
    [PunRPC]
    public void OnDirectionChange_RIFHT()
    {
        sr.flipX = false;
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
