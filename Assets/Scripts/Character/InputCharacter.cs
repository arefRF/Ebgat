﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCharacter : MonoBehaviour
{
    public float speed;
    private PlayerControl playerControl;

    private CharacterAttributes charStats;
    private Attack attack;
    private CharacterMove characterMove;
    private PlayerJump jump;
    private ClientNetworkSender clientNetworkSender;
	// Use this for initialization
	void Start ()
    {
        playerControl = GetComponent<PlayerControl>();
        charStats = playerControl.charStats;
        clientNetworkSender = playerControl.clientNetworkSender;
        attack = playerControl.attack;
        characterMove = playerControl.characterMove;

        jump = playerControl.jump;
	}
	// Update is called once per frame
	void Update ()
    {
        if (!playerControl.IsLocalPlayer())
            return;

        // Move left and Right
        if (Input.GetKeyDown(KeyCode.D))
            clientNetworkSender.Move(1);
        else if (Input.GetKeyDown(KeyCode.A))
            clientNetworkSender.Move(-1);
        


        //move button released
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            clientNetworkSender.MoveFinished(transform.position);
            characterMove.MoveReleased();
        }

        //Attack
        if (Input.GetButtonDown("Fire"))
        {
            attack.AttackPressed(Input.mousePosition);
        }
        else if(Input.GetAxis("Fire") > 0.1)
        {
            print("RT");
        }

        // Jump

        if (Input.GetButtonDown("Jump"))
        {
            clientNetworkSender.JumpPressed(transform.position);
            jump.JumpPressed();
        }
        else if (Input.GetButton("Jump"))
        {
            jump.JumpHold();
        }

        if (Input.GetButtonUp("Jump"))
        {
            clientNetworkSender.JumpReleased(transform.position);
            jump.JumpReleased();
        }
	}


}
