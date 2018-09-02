﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BahramGraphics : HeroGraphics{
    public GameObject IronFistInstance;
    private int attackNumber = 1;
    public AudioClip[] audio_clips;

    public override void HandState(string value)
    {
        if (value == "2")
        {
            animator.SetInteger("Attack Number", attackNumber);
            animator.SetTrigger("Attack");
        }

    }

    private void AttackEffect1()
    {

    }

    private void AttackEffect2()
    {

    }
    public override void AttackNumber(string value)
    {
        attackNumber = int.Parse(value) +1 ;
    }

    public override void AbilityState(string value)
    {
        if (value == "1")
        {
            animator.SetBool("Ability1", true);
        }
        else if (value == "2")
        {
            animator.SetBool("Ability1", false);
            GameObject ironFistEffect = Instantiate(IronFistInstance, transform.position, Quaternion.Euler(0, 0, 0));
            StartCoroutine(DestoryObjectAfterTime(1.5f, ironFistEffect));
        }
        else if (value == "3")
        {
            animator.SetBool("Boar", true);
        }
        else if (value == "4")
        {
            animator.SetBool("Boar", false);
        }
    }
    public override void BodyState(string value)
    {
        if (value == "1")
        {

            animator.SetBool("Walking", false);
        }
        else if (value == "2")
        {

            animator.SetBool("Walking", true);
        }
        else if(value == "3")
        {
            animator.SetTrigger("Roll");
        }
        else
            print("Body State Wrong Code");
    }
    public override void FeetState(string value)
    {
        animator.SetBool("OnWall", false);
        if (value == "1")
        {
            GameObject land = Instantiate(landInstance);
            StartCoroutine(DestoryObjectAfterTime(1, land));
            land.transform.position = transform.position + Vector3.down * 3 /2;
            animator.SetTrigger("OnGround");
        }
        else if (value == "2")
            animator.SetTrigger("Fall");
        else if (value == "3")
        {
            audioSource.Play();
            animator.SetTrigger("Jump");
        }
        else if(value =="6")
        {
            animator.SetBool("OnWall", true);
        }
        else if (value == "4")
        {
            //print(EFeetState.NoGravity);
        }
        else if (value == "5")
            animator.SetTrigger("DoubleJump");
        else
            print("Wrong Feet State Code");

    }
}
