using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmerdadGraphics : HeroGraphics {

    public override void BodyState(string value)
    {
        animator.SetBool("Dash", false);
        animator.SetBool("Walk", false);
        if (value == "1")
        {

        }
        else if (value == "2")
        {
            animator.SetBool("Walk", true);
        }
        else if (value == "3")
        {
            animator.SetBool("Dash",true);
        }
    }

    public override void FeetState(string value)
    {
        animator.SetBool("OnWall", false);
        if (value == "1")
        {
            GameObject land = Instantiate(landInstance);
            StartCoroutine(DestoryObjectAfterTime(1, land));
            land.transform.position = transform.position + Vector3.down * 3 / 2;
            if (charStats.bodyState != EBodyState.Dashing)
                animator.SetTrigger("OnGround");
        }
        else if (value == "2")
        {
            animator.SetTrigger("Fall");
        }
        else if (value == "3")
        {
            animator.SetTrigger("Jump");
        }
        else if (value == "6")
        {
            animator.SetTrigger("OnWall");
        }
        else if (value == "4")
        {
            //print(EFeetState.NoGravity);
        }
        else if (value == "8")
            animator.SetTrigger("Jump");
    }

    public override void HandState(string value)
    {
        if (value == "2")
            animator.SetTrigger("Attack");
    }

    public override void AbilityState(string value)
    {
        if (value == "1")
        {
            animator.SetTrigger("Ability 1");
        }
        else if (value == "3")
            animator.SetTrigger("Ulti");
    }

}
