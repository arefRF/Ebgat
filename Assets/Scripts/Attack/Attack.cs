﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour {
    
    protected float cooldownTimer;
    protected CharacterAttributes charStats;
    protected HeroGraphics heroGraphics;
    protected PlayerControl playerControl;
	// Use this for initialization
    void Start () {
        heroGraphics = GetComponent<HeroGraphics>();
        playerControl = GetComponent<PlayerControl>();
        charStats = playerControl.charStats;
        cooldownTimer = 0;
        if (charStats.attackMode == EAttackMode.Ranged)
            if (!(this is RangedAttack))
                print("Character Attack Mode is Range but Component is not");
          
        if (charStats.attackMode == EAttackMode.Melee)
            if (!(this is MeleeAttack))
                print("Character Attack Mode is Melee but Component is not");
	}
	
	// Update is called once per frame
	void Update () {
        if(cooldownTimer>0)
            cooldownTimer -= Time.deltaTime;
        
	}

    public virtual void AttackPressed(Vector2 attackDir) {}

    public virtual void AttackServerside(Vector2 attackDir){}

    public virtual void AttackClientside(Vector2 attackDir, int attackID){}

    public virtual void AttackHitClientSide(int attackID){} 

    public virtual void AttackHitServerSide(int attackID){
        
    }
}
