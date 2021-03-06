using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Globalization;

public class ClientNetworkReciever : NetworkBehaviour {

    public static ClientNetworkReciever instance;

    private List<PlayerControlClientside> playerControls;
    private PlayerControlClientside localPlayerControl; 

    int playernumber;

    void Awake()
    {
        instance = this;
        playerControls = new List<PlayerControlClientside>();
    }

    [ClientRpc]
    public void RpcRecieveWorldData(string[] worlddata, int packetID){
        if (playerControls.Count != playernumber)
            UpdatePlayer();
        for (int i = 0; i < worlddata.Length; i++)
        {
            if (worlddata[i].Length == 0)
            {
                
                continue;
            }
            string[] heroData = worlddata[i].Split('#');
            int frameBaseId = packetID * 3;
            for (int j = 1; j < heroData.Length; j++)
            {
                if (heroData[j].Length == 0)
                    continue;
                string[] rawData = heroData[j].Split('@');
                string[] data = rawData[1].Split('$');
                int id = Convert.ToInt32(rawData[0]);
                if (id == 0 || id > playerControls.Count)
                    continue;
                playerControls[id - 1].AddTOHashTable(frameBaseId + i, rawData[1]);
                if(!rawData[2].Equals(""))
                    playerControls[id - 1].Shoot(rawData[2]);
                if(!rawData[3].Equals(""))
                    playerControls[id - 1].DestroyBullet(rawData[3]);
                if (!rawData[4].Equals(""))
                    playerControls[id - 1].GetAdditionalData(rawData[4]);
            }
            if(!heroData[0].Equals(""))
                localPlayerControl.GetAdditionalWorldData(heroData[0]);
        }
    }

    public void RecieveWorldstate(string worldstates, int frameId){
        if (worldstates.Length == 0)
            return;
        string[] frameState = worldstates.Split('!');
        for (int i = 0; i < frameState.Length; i++)
        {
            string[] heroData = frameState[i].Split('#');
            for (int j = 1; j < heroData.Length; j++)
            {
                if (heroData[j].Length == 0)
                    continue;
                string[] rawData = heroData[j].Split('@');
                Debug.Log(frameState[i]);
                string[] data = rawData[1].Split('$');
                int id = Convert.ToInt32(rawData[0]);
                if (id == 0 || id > playerControls.Count)
                    continue;
                playerControls[id - 1].AddTOHashTable(frameId + i, rawData[1]);
                if (!rawData[2].Equals(""))
                    playerControls[id - 1].Shoot(rawData[2]);
                if (!rawData[3].Equals(""))
                    playerControls[id - 1].DestroyBullet(rawData[3]);
                if (!rawData[4].Equals(""))
                    playerControls[id - 1].GetAdditionalData(rawData[4]);
            }
            if (!heroData[0].Equals(""))
                localPlayerControl.GetAdditionalWorldData(heroData[0]);
        }
        localPlayerControl.UpdateClient();
    }

    [ClientRpc]
    public void RpcUpdatePlayers(){
        playernumber = GameObject.FindGameObjectsWithTag("Player").Length;
    }

    private void UpdatePlayer(){
        playerControls.Clear();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
        PlayerControlClientside[] playerControlArray = new PlayerControlClientside[objs.Length];
        for (int i = 0; i < objs.Length; i++)
        {
            PlayerControlClientside p = objs[i].GetComponent<PlayerControlClientside>();
            if (p.playerId == 0)
                return;
            playerControlArray[p.playerId - 1] = p;
            if (p.IsLocalPlayer())
                localPlayerControl = p;
        }
        playerControls.AddRange(playerControlArray);
    }

}
