﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {
    
    public GameObject serverNetwork;
    public GameObject clientNetworkReciever;
    public List<GameObject> players;
    public GameObject playerConnection;

    private bool flag = true;
    bool start = true;

    public int playerNumber { get; set; }

    private int playerID = 0;
    private Hashtable connectionTable;

    private void Start()
    {
        connectionTable = new Hashtable();
    }

    private void Update()
    {
        if(NetworkServer.active && flag){
            flag = false;
            GameObject server = Instantiate(serverNetwork);
            GameObject clientNetwork = Instantiate(clientNetworkReciever);
            NetworkServer.Spawn(clientNetwork);
            NetworkServer.Spawn(server);
        } 
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        connectionTable.Add(++playerID, conn);
        GameObject playercon = Instantiate(playerConnection);
        playercon.GetComponent<PlayerConnection>().clientId = playerID;
        NetworkServer.AddPlayerForConnection(conn, playercon, playerControllerId);

    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

    }

    public void SpawnHero(int clientId, int heroId){
        GameObject player = Instantiate(players[heroId]);
        NetworkConnection connnnnn = connectionTable[clientId] as NetworkConnection;
        NetworkServer.SpawnWithClientAuthority(player, connnnnn);
        ServerManager.instance.UpdatePlayers();
        ClientNetworkReciever.instance.RpcUpdatePlayers();
    }

}