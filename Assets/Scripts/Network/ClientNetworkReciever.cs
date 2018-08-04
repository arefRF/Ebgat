﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Globalization;

public class ClientNetworkReciever : NetworkBehaviour {

    public static ClientNetworkReciever instance;

    public List<PlayerControl> playerControls;

    int playernumber;

    void Awake()
    {
        instance = this;
        playerControls = new List<PlayerControl>();
    }

    [ClientRpc]
    public void RpcRecieveCommands(string data){
        if (playerControls.Count != playernumber)
            ccss();
        string[] lines = data.Split('\n');
        for (int i = 0; i < lines.Length - 1; i++)
        {
            string[] parts = lines[i].Split(',');
            int playerID = Convert.ToInt32(parts[0]);
            switch (parts[1])
            {
                case "1": playerControls[playerID - 1].characterMove.MoveClientside(new Vector3(float.Parse(parts[2], CultureInfo.InvariantCulture.NumberFormat), float.Parse(parts[3], CultureInfo.InvariantCulture.NumberFormat), float.Parse(parts[4], CultureInfo.InvariantCulture.NumberFormat))); break;
                case "2": playerControls[playerID - 1].characterMove.MoveReleasedClientside(new Vector3(float.Parse(parts[2], CultureInfo.InvariantCulture.NumberFormat), float.Parse(parts[3], CultureInfo.InvariantCulture.NumberFormat), float.Parse(parts[4], CultureInfo.InvariantCulture.NumberFormat))); break;
                case "6": playerControls[playerID - 1].SetVerticalDirection(Convert.ToInt32(parts[2])); break;
            }
        }
    }

    [ClientRpc]
    public void RpcUpdatePlayers(){
        playernumber = GameObject.FindGameObjectsWithTag("Player").Length;
    }

    private void ccss(){
        playerControls.Clear();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
        PlayerControl[] playerControlArray = new PlayerControl[objs.Length];
        for (int i = 0; i < objs.Length; i++)
        {
            PlayerControl p = objs[i].GetComponent<PlayerControl>();
            playerControlArray[p.clientNetworkSender.PlayerID - 1] = p;
        }
        playerControls.AddRange(playerControlArray);
    }

    /*public void RpcMove(int num)
    {
        if (isLocalPlayer)
        {
            return;
        }
        characterMove.MovePressed(num);
    }

    public void RpcMoveFinished(Vector3 position)
    {
        if (isLocalPlayer)
        {
            return;
        }
        Debug.Log("RPC MoveFINISHED");
        transform.position = position;
    }

    public void RpcJumpPressed(Vector3 position)
    {
        if (isLocalPlayer)
            return;
        transform.position = position;
        jump.JumpPressed();
    }

    public void RpcJumpLong(Vector3 position)
    {
        if (isLocalPlayer)
            return;
        transform.position = position;
        jump.IncreaseJumpSpeed();
    }

    public void RpcJumpReleased(Vector3 position)
    {
        if (isLocalPlayer)
            return;
        transform.position = position;
        jump.JumpReleased();
    }

    public void RpcMeleeAttack(Vector3 position){
        playerControl.attack.AttackPressed(position);
    }
    */

    public void RpcShootBullet(Vector3 targetdirection, Vector3 origin, float bulletDamage)
    {
    }

    /*[ClientRpc]
    public void RpcTakeAttack(float damage)
    {
        if (isServer)
            return;
        playerControl.TakeAttack(damage, null);
    }*/
}
