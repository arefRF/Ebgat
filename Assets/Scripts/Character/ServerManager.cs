﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : NetworkBehaviour {
    
    public static ServerManager instance;
    public WorldState currentWorldState;

    private CustomNetworkManager networkManager;
    private List<PlayerControl> playerControls;
    private int finishedPLayercounter;

    public int CurrentStateID = 0;

    private List<int> reservelist;

    private List<PlayerId> playerIdList;

    public int maxClientCount, currentClientCount, spawnedHeroCount;
    private int bulletIdCounter = 0;

    private void Awake()
    {
        instance = this;
        currentWorldState = new WorldState();
        playerControls = new List<PlayerControl>();
        networkManager = GameObject.FindWithTag("NetworkManager").GetComponent<CustomNetworkManager>();
        reservelist = new List<int>();
        playerIdList = new List<PlayerId>();
        maxClientCount = networkManager.maxPlayerCount;
        currentClientCount = 0;
        UpdatePlayers();
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	/*void Update () {
        if (playerControls.Count == 0)
            UpdatePlayers();
	}*/

    private void SetWorldStateOnPlayers(){
        foreach(PlayerControl p in playerControls){
            p.worldState = currentWorldState;
        }
    }

    public void UpdatePlayers()
    {
        playerControls.Clear();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < objs.Length; i++)
            playerControls.Add(objs[i].GetComponent<PlayerControl>());
        SetWorldStateOnPlayers();
    }

    public void PlayerSimulationFinished(int ID){
        finishedPLayercounter++;
        if(finishedPLayercounter == playerControls.Count){
            ServerNetworkSender.instance.RegisterWorldState(currentWorldState);
            finishedPLayercounter = 0;
            currentWorldState = new WorldState();
            UpdatePlayers();

        }
    }

    public void RequestworldFullState(int playerID){
        reservelist.Add(playerID);

    }
    
    public void SendWorldStateToClient(int playerID){
        WorldState tempWorldState = new WorldState();
        foreach (PlayerControl p in playerControls)
        {
            tempWorldState.RegisterHeroPhysics(p.playerId, p.physic.virtualPosition, Vector2.zero);
            p.charStats.RegisterAllStates();
        }
        ServerNetworkSender.instance.SendWorldFullstate(tempWorldState, playerID);
    }

    public void SendFullWorldStates(){
        foreach(int id in reservelist){
            SendWorldStateToClient(id);
        }
        reservelist.Clear();
    }




    public void SpawnHero(int clientId, int heroId, int teamId)
    {
        for (int i = 0; i < networkManager.playerConnections.Count; i++)
        {
            if (networkManager.playerConnections[i].clientId == clientId)
            {
                networkManager.playerConnections[i].RpcInstansiateHero(heroId, teamId);
            }
        }

    }


    public void ClientConnected(int clientId, int heroId)
    {
        Debug.Log("id: " + clientId + " , " + heroId);
        playerIdList.Add(new PlayerId(clientId, heroId));
        currentClientCount++;
        int teamId = 1;
        if (currentClientCount == maxClientCount)
        {
            for (int i = 0; i < maxClientCount; i++)
            {
                SpawnHero(playerIdList[i].clientId, playerIdList[i].heroId, teamId);
                if (teamId == 1)
                    teamId = 2;
                else
                    teamId = 1;
                break;
            }
            UpdatePlayers();
            ClientNetworkReciever.instance.RpcUpdatePlayers();
        }
    }

    public void HeroSpawned(int cliendid){
        spawnedHeroCount++;
        if(spawnedHeroCount == maxClientCount){
            for (int i = 0; i < maxClientCount; i++){
                networkManager.playerConnections[i].RpcSetReady();
            }
        }
    }

    public int GetBulletID(int PlayerID)
    {
        return ++bulletIdCounter;
    }
}

struct PlayerId{
    public int clientId, heroId;

    public PlayerId(int clientId, int heroId){
        this.heroId = heroId;
        this.clientId = clientId;
    }
}