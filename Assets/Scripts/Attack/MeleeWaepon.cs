﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWaepon : MonoBehaviour {
    public Vector2 size;

    int ID;

	// Use this for initialization
	void Start () {
		
	}

    // Attack in direction and alert objects of attack
    public void Attack(Vector2 origin,float damage,Vector2 direction,int layer, Attack attack, int attackID)
    {
        print("Attack Meele");
        ID = attackID;
        RaycastHit2D[] hitObjects = Physics2D.BoxCastAll(origin, new Vector2(0.01f,size.y), 0, direction, size.x, layer, 0, 0);
        foreach (RaycastHit2D hit in hitObjects)
        {
            print(hit.collider.name);
            if (hit.collider.tag == "Player")
            {
                print("Took Attack");
                attack.AttackHitServerSide(ID, damage, true);
            }
        }
    }
}
