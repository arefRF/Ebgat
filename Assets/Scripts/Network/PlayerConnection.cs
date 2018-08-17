﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnection : NetworkBehaviour {

    [SyncVar] public int clientId;

    private CustomNetworkManager networkManager;
    private ServerManager serverManager;
    private ClientNetworkSender clientNetworkSender;
    private ServerNetwork serverNetworkReciever;
    private PlayerControl playerControl;

    public Vector2 spawnPoint { get; set; }

    [SerializeField]
    private GameObject player;

	// Use this for initialization
	void Start () {
        networkManager = GameObject.FindWithTag("NetworkManager").GetComponent<CustomNetworkManager>();
        serverManager = ServerManager.instance;
        serverNetworkReciever = GetComponent<ServerNetwork>();
        if(isLocalPlayer){
            serverNetworkReciever.CmdClientConnected(clientId, networkManager.playerNumber);
        }
	}

    private void Update()
    {
        //Debug.Log(isLocalPlayer);
    }

    public void SetClientNetworkSender(ClientNetworkSender clientNetworkSender){
        this.clientNetworkSender = clientNetworkSender;
    }

    [ClientRpc]
    public void RpcInstansiateHero(int heroId, int teamId, string spawnPoint){
        this.spawnPoint = Toolkit.DeserializeVector(spawnPoint);
        if(isServer)
            player = Instantiate(networkManager.serverSidePlayers[heroId], this.spawnPoint, Quaternion.Euler(0,0,0));
        else
            player = Instantiate(networkManager.clientSidePlayers[heroId]);
        playerControl = player.GetComponent<PlayerControl>();
        serverNetworkReciever.SetPlayerControl(playerControl);
        playerControl.SetNetworkComponents(this, clientNetworkSender, serverNetworkReciever, clientId);
        SetTeam(teamId);
        StartCoroutine(SetReadyWait(1));
        //serverNetworkReciever.CmdHeroSpawned(clientId);
    }

    private void SetTeam(int teamId){
        string teamName = "", enemyTeamName = "";
        if(teamId == 1){
            teamName = "Team " + 1;
            enemyTeamName = "Team " + 2;
        }
        else if(teamId == 2){
            teamName = "Team " + 2;
            enemyTeamName = "Team " + 1;
        }
        else{
            Debug.Log("wrong team id");
        }
        playerControl.SetTeam(teamName, enemyTeamName);
    }

    [ClientRpc]
    public void RpcSetReady(){
        playerControl.SetReady();
    }

    private IEnumerator SetReadyWait(int time){
        yield return new WaitForSeconds(time);
        playerControl.SetReady();
    }

    [ClientRpc]
    public void RpcKillHero(){
        playerControl.Die();
    }

    [ClientRpc]
    public void RpcRespawnHero(){
        playerControl.Respawn();
    }
}
