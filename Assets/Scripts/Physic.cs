﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class Physic : MonoBehaviour
{
    public float weight;
    protected int layerMask;
    protected Vector2 size;
    protected Vector2 distance;
    public Vector2 virtualPosition { get; protected set;}
   // private bool start;
   // private bool first;
   // private float startTimer;
    protected PlayerControl playerControl;

    // Use this for initialization

    //private void LateUpdate()
    //{
    //   // Debug.Log(Time.deltaTime);
    //    if(start)
    //    {
    //        if (playerControl != null)
    //        {
    //            if (playerControl.IsServer())
    //            {
    //                if (!first)
    //                {
    //                    StartCoroutine(PhysicUpdate());
    //                    first = true;
    //                }
    //            }
    //        }
    //    }
    //    else{
    //        startTimer+= Time.deltaTime;
    //        if(startTimer>1)
    //            start = true;
    //    }
    //}
    private const double interval = 0.01666;
    private double currentTime;
    public IEnumerator PhysicUpdate()
    {
        while (true)
        {
            currentTime += Time.deltaTime;
            if (currentTime < interval)
            {
                yield return null;
            }
            currentTime -= interval;
           // Calculate();
            yield return null;
        }
    }
    public void AddForce(Vector2 force)
    {
        distance += force;
    }
    protected abstract void Calculate();
}

