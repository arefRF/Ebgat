﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class Physic : MonoBehaviour {
    public Action<List<RaycastHit2D>, List<RaycastHit2D>, Vector2> PhysicAction;

    public float wight;

    protected int layerMask;
    protected int gravityLayerMask;

    protected Vector2 size;
    protected Vector2 distance;
    protected Vector2 virtualPosition;



    protected List<Vector2> forces = new List<Vector2>();
    protected List<Vector2> destenitions = new List<Vector2>();
    protected List<RaycastHit2D> verticalPoints = new List<RaycastHit2D>();
    protected List<RaycastHit2D> horizontalPoints = new List<RaycastHit2D>();

    protected PlayerControl playerControl;

	// Use this for initialization
	private void Start ()
    {
        size = transform.localScale * GetComponent<BoxCollider2D>().size;
        virtualPosition = transform.position;
	}

    private void LateUpdate()
    {
        
        if (playerControl.IsServer())
        {
            Calculate();
        }
    } 
    private void Calculate()
    {
        bool vHit = false ;
        bool hHit = false ;
        verticalPoints.Clear();
        horizontalPoints.Clear() ;
        
        distance = Vector2.zero;

        Vector2 originalDistance = distance;
        while (forces.Count > 0)
        {
            var currentforce = forces[0];
            distance += currentforce;
            forces.Remove(currentforce);
        }
        if (distance.x > 0)
        {
            hHit = Toolkit.CheckMoveFloat(virtualPosition, size, Vector2.right, distance.x, layerMask, out horizontalPoints);
            if (hHit)
            {
                distance.x = horizontalPoints[0].distance;
            }
        }
        else if (distance.x < 0)
        {
            hHit = Toolkit.CheckMoveFloat(virtualPosition, size, Vector2.left, -distance.x, layerMask, out horizontalPoints);
            if (hHit)
            {
                distance.x = -horizontalPoints[0].distance;
            }
        }
        if (distance.y > 0)
        {
            vHit = Toolkit.CheckMoveFloat(virtualPosition, size, Vector2.up, distance.y, layerMask, out verticalPoints);
            if (vHit)
            {
                distance.y = verticalPoints[0].distance;
            }
        }
        else if (distance.y < 0)
        {
            vHit = Toolkit.CheckMoveFloat(virtualPosition, size, Vector2.down, -distance.y, gravityLayerMask, out verticalPoints);
            if (vHit)
            {
                distance.y = -verticalPoints[0].distance;
            }
        }
        virtualPosition += distance;
        destenitions.Add(virtualPosition);
        if(PhysicAction != null)
        {
            PhysicAction(verticalPoints, horizontalPoints, distance);
        }
        HitFunction(verticalPoints, horizontalPoints, originalDistance);
        PhysicAction = null;
        ServerManager.instance.PlayerSimulationFinished(playerControl.clientNetworkSender.PlayerID);
    }

    public void AddForce(Vector2 force)
    {
        forces.Add(force);
    }

    abstract protected void HitFunction(List<RaycastHit2D> vHits, List<RaycastHit2D> hHits, Vector2 direction);

    




}
