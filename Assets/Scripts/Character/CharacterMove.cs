﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterMove : MonoBehaviour {
    

    private PlayerControl playerControl;
    private CharacterAttributes charStats;
    private CharacterPhysic Physic;
    private Animator animator;
    private Vector2 side;
    private int moveSide;

    void Awake()
    {

    }
	void Start ()
    {

        Physic = GetComponent<CharacterPhysic>();
        animator = GetComponentInChildren<Animator>();
        charStats = GetComponent<CharacterAttributes>();
        playerControl = GetComponent<PlayerControl>();
	}
    void FixedUpdate()
    {
        if (playerControl.IsServer())
        {
            if(charStats.BodyState == EBodyState.Moving)
                if(charStats.HeadState == EHeadState.Stunned)
                {
                    charStats.BodyState = EBodyState.Standing;
                    charStats.ResetMoveSpeed(); 
                }
                else
                {
                    MoveServerside();
                }
        }
    }
    public void MovePressed(int i)
    {
        if (charStats.HeadState != EHeadState.Stunned)
        {
            charStats.AimSide = new Vector2(i, charStats.AimSide.y);
            charStats.BodyState = EBodyState.Moving;
        }
        moveSide = i;
    }
    public void MoveServerside()
    {
        SpeedCheck(moveSide);
        Vector2 force = Vector2.right * moveSide * charStats.MoveSpeed * Time.deltaTime;
        Physic.AddForce(force);
        Physic.PhysicAction += HitFunction;
    }
    public void MoveReleasedServerside(Vector3 position){
        charStats.AimSide = new Vector2(0, charStats.AimSide.y);
        charStats.BodyState = EBodyState.Standing;
       // playerControl.serverNetworkSender.ClientMoveFinished(playerControl.clientNetworkSender.PlayerID, position);
    }
    private void SpeedCheck(int i)
    {
        side = Vector2.right * i;
        if(side  != charStats.Side)
        {
            charStats.ResetMoveSpeed();
            charStats.Side = side;
        }
        if(charStats.BodyState == EBodyState.Standing)
        {
            charStats.ResetMoveSpeed();
        }
        if(charStats.BodyState == EBodyState.Moving && charStats.FeetState == EFeetState.Onground)
        {
            charStats.MoveSpeed += charStats.MoveAcceleration;
            if(charStats.MoveSpeed > charStats.MoveSpeedMax)
            {
                charStats.MoveSpeed = charStats.MoveSpeedMax;
            }
        }
    }
    private void HitFunction(List<RaycastHit2D> vHits,List<RaycastHit2D> hHits,Vector2 direction)
    {
        if(direction.x * moveSide > 0 && hHits.Count > 0)
        {
            charStats.ResetMoveSpeed();
        }
    }

}



