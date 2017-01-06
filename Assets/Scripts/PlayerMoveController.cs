﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMoveController : MonoBehaviour {

    public GameObject CapsuleHand_L;
    private Swipes CapsuleHandSwipes_L;
    public GameObject CapsuleHand_R;
    private Swipes CapsuleHandSwipes_R;

    //public float movingTimeLimit = 0.8f;
    //public float movingTimeCurrent = 0f;

    public float changeLaneTimerLimit = 0.8f;
    public float slideTimerLimit = 1f;
    public float knockBackTimerLimit = 3f;

    public List<PlayerMoveData> moveList = new List<PlayerMoveData>();
    public PlayerMoveData moveForward;
    public PlayerMoveData moveRight;
    public PlayerMoveData moveLeft;
    public PlayerMoveData jump;
    public PlayerMoveData slide;
    public PlayerMoveData knockBack;

    public float playerStartSpeed = 2f;
    public float playerSpeed;

    private float newLanePosX;

    private Rigidbody playerRigidbody;
    private bool isGrounded;
    public float jumpPower = 5f;

    public GameObject leftCollider;
    public GameObject rightCollider;
    public GameObject topCollider;
    public GameObject bottomCollider;
    private SideCollider rightColliderScript;
    private SideCollider leftColliderScript;
    private SideCollider topColliderScript;
    //private SideCollider bottomColliderScript;
    void Awake()
    {
        CapsuleHandSwipes_L = CapsuleHand_L.GetComponent<Swipes>();
        CapsuleHandSwipes_R = CapsuleHand_R.GetComponent<Swipes>();
        playerRigidbody = GetComponent<Rigidbody>();
        rightColliderScript = rightCollider.GetComponent<SideCollider>();
        leftColliderScript = leftCollider.GetComponent<SideCollider>();
        topColliderScript = topCollider.GetComponent<SideCollider>();
        //bottomColliderScript = bottomCollider.GetComponent<SideCollider>();
    }
    void Start () {
        InitMoveList();
        moveForward.isMoving = true;
        playerSpeed = playerStartSpeed;
    }
    private void InitMoveList()
    {
        moveForward = new PlayerMoveData("MoveForward");
        moveRight = new PlayerMoveData("MoveRight");
        moveLeft = new PlayerMoveData("MoveLeft");
        jump = new PlayerMoveData("Jump");
        slide = new PlayerMoveData("Slide");
        knockBack = new PlayerMoveData("KnockBack");

        moveForward.InitOtherMoveStopList(new List<PlayerMoveData> { knockBack });
        moveRight.InitOtherMoveStopList(new List<PlayerMoveData> { moveLeft });
        moveLeft.InitOtherMoveStopList(new List<PlayerMoveData> { moveRight });
        jump.InitOtherMoveStopList(new List<PlayerMoveData> { slide });
        slide.InitOtherMoveStopList(new List<PlayerMoveData> { jump });

        moveRight.InitTimerLimit(changeLaneTimerLimit);
        moveLeft.InitTimerLimit(changeLaneTimerLimit);
        slide.InitTimerLimit(slideTimerLimit);
        knockBack.InitTimerLimit(knockBackTimerLimit);

        moveList.Add(moveForward);
        moveList.Add(moveRight);
        moveList.Add(moveLeft);
        moveList.Add(jump);
        moveList.Add(slide);
        moveList.Add(knockBack);
    }
    void Update () {
        MoveForward();
        CheckIsChangeLane();
        CheckMoveTrigger();
        WaitAllMovingTimer();
    }
    private void CheckIsChangeLane()
    {
        if (moveRight.isMoving || moveLeft.isMoving)
        {
            ChangeLane(newLanePosX, changeLaneTimerLimit / (Mathf.Abs(transform.position.x - newLanePosX)));
        }
    }
    private void WaitAllMovingTimer()
    {
        moveRight.WaitMovingTimer();
        moveLeft.WaitMovingTimer();
        slide.WaitMovingTimer();
        knockBack.WaitMovingTimer();
    }
    public void ChangeLane(float newLanePosX, float fracJourney)
    {
        Vector3 newLanePos = new Vector3(newLanePosX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newLanePos, fracJourney);
    }
    public void ChangeNewLanePosX(float addPosX)
    {
        newLanePosX += addPosX;
    }
    public void MoveForward()
    {
        if (!moveForward.OtherMoveIsMoving())
        {
            moveForward.isMoving = true;
            Vector3 newPos = transform.position;
            newPos.z += Time.deltaTime * playerSpeed;
            transform.position = newPos;
        }
    }
    private void CheckMoveTrigger()
    {
        if (CapsuleHandSwipes_L.IsSwipingRight && CapsuleHandSwipes_R.IsSwipingRight)
        {
            if (!moveRight.isMoving && !transform.position.x.Equals(1) && !moveRight.OtherMoveIsMoving() && !rightColliderScript.isCollided)
            {
                Debug.Log("MoveRight");
                ChangeNewLanePosX(1);
                moveRight.isMoving = true;
                moveRight.StartMoveTimer();
            }
            
        }
        else if (CapsuleHandSwipes_L.IsSwipingLeft && CapsuleHandSwipes_R.IsSwipingLeft)
        {
            if (!moveLeft.isMoving && !transform.position.x.Equals(-1) && !moveLeft.OtherMoveIsMoving() && !leftColliderScript.isCollided)
            {
                Debug.Log("MoveLeft");
                ChangeNewLanePosX(-1);
                moveLeft.isMoving = true;
                moveLeft.StartMoveTimer();
            }
        }
        else if (CapsuleHandSwipes_L.IsSwipingUp && CapsuleHandSwipes_R.IsSwipingUp)
        {
            if (isGrounded && !jump.OtherMoveIsMoving() && !topColliderScript.isCollided)
            {
                Debug.Log("Jump");
                playerRigidbody.velocity = new Vector3(0, jumpPower, 0);
                isGrounded = false;
                jump.isMoving = true;
            }

        }
        else if (CapsuleHandSwipes_L.IsSwipingDown && CapsuleHandSwipes_R.IsSwipingDown)
        {   /*
            if (!slide.isMoving && isGrounded && !slide.OtherMoveIsMoving())
            {
                Debug.Log("Slide");
                slide.isMoving = true;
                slide.StartMoveTimer();
            }
            */
        }
    }
    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag.Equals("Ground"))
        {
            isGrounded = true;
            jump.isMoving = false;
        }
    }

    public void KnockBack()
    {
        knockBack.isMoving = true;
        knockBack.StartMoveTimer();
        isGrounded = false;
        Vector3 newPos = new Vector3(0, 6, transform.position.z - 6);
        transform.position = newPos;
    }
}
