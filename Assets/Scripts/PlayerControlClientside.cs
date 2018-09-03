﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerControlClientside : MonoBehaviour
{
    public Action ReadyAction;


    public CharacterAttributesClient charStatsClient { get; set; }
    public HeroGraphics heroGraphics { get; private set; }
    public ClientNetworkSender clientNetworkSender { get; private set; }
    public ClientNetworkReciever clientNetworkReciever { get; private set; }
    public PlayerConnection playerConnection;// { get; set; }
    public BulletManager bulletmanager { get; set; }
    public static string teamName { get; set; }
    public CharacterAim aim { get; set; }


    private Hashtable playerStatesHash = new Hashtable();
    private int lastStateChecked;
    private int currentStateNumber;
    private int biggestIdNumber;
    private bool start;
    private bool firstRecieved;
    private bool waitingForRequest;
    public int playerId { get; private set; }
    // Use this for initialization
    void Awake()
    {
        clientNetworkReciever = ClientNetworkReciever.instance;
        charStatsClient = GetComponent<CharacterAttributesClient>();
        heroGraphics = GetComponent<HeroGraphics>();
        bulletmanager = GetComponent<BulletManager>();
        aim = GetComponent<CharacterAim>();
    }

    void Start()
    {
        if (IsLocalPlayer())
        {
            Camera.main.GetComponent<SmoothCamera2D>().target = this.transform;
        }
    }

    private void FixedUpdate()
    {
        ReadData();
    }

    public void SetNetworkComponents(PlayerConnection playerConnection, ClientNetworkSender clientNetworkSender, ServerNetwork serverNetworkReciever, int playerId)
    {
        this.playerConnection = playerConnection;
        this.clientNetworkSender = clientNetworkSender;
        this.playerId = playerId;
    }

    public void SetTeam(string teamName, string enemyTeamName)
    {
        if (IsLocalPlayer())
            PlayerControl.teamName = teamName;

        charStatsClient.teamName = teamName;
        charStatsClient.enemyTeamName = enemyTeamName;
        gameObject.layer = LayerMask.NameToLayer(teamName);
    }

    public void SetReady()
    {
        heroGraphics.CreateHpBar();
        ReadyAction();
    }

    private void ReadData()
    {
        if (start)
        {
            if (IsLocalPlayer())
            {
                // Debug.Log(currentStateNumber + "+" + biggestIdNumber + "+" + lastStateChecked + "+ " + Time.frameCount);
            }
            if (playerStatesHash.Contains(currentStateNumber))
            {
                for (int i = lastStateChecked + 1; i <= currentStateNumber; i++)
                {
                    if (playerStatesHash.Contains(i))
                    {
                        GetData((string)playerStatesHash[i]);
                        playerStatesHash.Remove(i);
                    }
                }
                lastStateChecked = currentStateNumber;
            }
            else if (currentStateNumber > biggestIdNumber && lastStateChecked < biggestIdNumber)
            {
                for (int i = lastStateChecked + 1; i <= biggestIdNumber; i++)
                {
                    GetData((string)playerStatesHash[i]);
                    playerStatesHash.Remove(i);
                }
                lastStateChecked = biggestIdNumber;
            }
            if (currentStateNumber - lastStateChecked >= 3 && !waitingForRequest)
            {
                if (IsLocalPlayer())
                {
                    Debug.Log("request for :" + currentStateNumber + "+" + Time.frameCount);
                    waitingForRequest = true;
                    clientNetworkSender.RequestWorldState(playerId);
                }
            }
            currentStateNumber++;
        }

    }

    public bool IsLocalPlayer()
    {
        try
        {
            return playerConnection.isLocalPlayer;
        }
        catch
        {
            return false;
        }
    }

    public void deltaYAim(float deltaY)
    {
        aim.yChange(deltaY);
    }

    public void deltaXAim(float deltaX)
    {
        aim.XChange(deltaX);
    }

    public void AimPressed()
    {
        aim.AimPressed();
    }

    public void AimReleased()
    {
        aim.AimReleased();
    }

    public void AimController(Vector2 aimAxis)
    {
        aim.ControllerAim(aimAxis);
    }

    public void GetData(string data)
    {
        try
        {
            bool first = true;
            string[] dataSplit = data.Split('$');
            foreach (string dataS in dataSplit)
            {
                string[] deString = dataS.Split('&');
                if (first)
                {
                    first = false;
                    heroGraphics.ChangePosition(Toolkit.DeserializeVector(deString[0]));

                }
                else
                {
                    if (deString.Length > 1)
                    {
                        Deserilize(deString[0].ToCharArray()[0], deString[1]);
                    }
                }
            }
        }
        catch (MissingReferenceException e)
        {
            Debug.Log(data);
        }
        /*catch (UnassignedReferenceException e)
        {
            Debug.Log(data);
        }*/
    }

    // gets the code and value of datas
    private void Deserilize(char code, string value)
    {

        charStatsClient.SetAttribute(code, value);
        switch (code)
        {
            case 'A': heroGraphics.AbilityState(value); break;
            case 'a': heroGraphics.HeadState(value); break;
            case 'b': heroGraphics.BodyState(value); break;
            case 'c': heroGraphics.HandState(value); break;
            case 'd': heroGraphics.FeetState(value); break;
            case 'e': heroGraphics.SetSide(value); break;
            case 'g': heroGraphics.HpChange(value); break;
            case 'x': if (playerConnection.isLocalPlayer) heroGraphics.EnergyChange(value); break;
            case 'y': heroGraphics.AttackNumber(value); break;
            case 'C': aim.AimClinet(); break;
        }
    }

    public void AddTOHashTable(int id, string state)
    {
        if (IsLocalPlayer())
        {
            //  Debug.Log(id + "+" + Time.frameCount);
        }
        if (!start && (!firstRecieved || currentStateNumber <= id))
        {
            currentStateNumber = id;
            lastStateChecked = id - 1;
            start = true;
            firstRecieved = true;
        }
        playerStatesHash.Add(id, state);
        if (id > biggestIdNumber)
        {
            biggestIdNumber = id;
        }
    }

    public void UpdateClient(int id, string state)
    {
        Debug.Log("awnser for :" + id + "+" + Time.frameCount);
        waitingForRequest = false;
        start = false;
        if (id > currentStateNumber)
        {
            currentStateNumber = id + 1;
        }
        GetData(state);
        for (int i = lastStateChecked + 1; i <= id; i++)
        {
            if (playerStatesHash.Contains(i))
            {
                playerStatesHash.Remove(i);
            }
        }
        lastStateChecked = id;
    }

    public void Die()
    {
        Debug.Log("hey teacher don't leave these codes alone ");

        //input.start = false;
        GetComponent<SpriteRenderer>().enabled = false;
        if (IsLocalPlayer())
        {
            Camera.main.GetComponent<SmoothCamera2D>().UnfollowTarget();
        }
    }

    public void Respawn()
    {
        Debug.Log("hey teacher don't leave these codes alone ");
        //input.start = true;
        GetComponent<SpriteRenderer>().enabled = true;
        if (IsLocalPlayer())
        {
            Camera.main.GetComponent<SmoothCamera2D>().FollowTarget();
        }
    }

    public void Shoot(string data)
    {
        string[] dataSplit = data.Split('$');
        foreach (string dataS in dataSplit)
        {
            if (dataS.Equals(""))
                continue;
            string[] deString = dataS.Split('&');
            int id = Convert.ToInt32(deString[0]);
            Vector2 attackSide = Toolkit.DeserializeVector(deString[1]);
            float gravityAcc = float.Parse(deString[2]);
            float range = float.Parse(deString[3]);
            int number = int.Parse(deString[4]);
            Vector2 startPos = Toolkit.DeserializeVector(deString[5]);
            bulletmanager.Shoot(attackSide, gravityAcc, id, range, number, startPos);
        }
    }
    public void DestroyBullet(string data)
    {
        string[] dataSplit = data.Split('$');
        foreach (string dataS in dataSplit)
        {
            if (dataS.Equals(""))
                continue;
            int id = Convert.ToInt32(dataS);
            bulletmanager.DestroyBullet(id);
        }
    }
}