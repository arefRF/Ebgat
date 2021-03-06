using System.Collections;
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

    private List<PlayerInfo> playerInfoList;
    private List<PlayerInfo> deadPlayers;

    public int maxClientCount, currentClientCount, spawnedHeroCount;
    private int bulletIdCounter = 0;

    public float respawnTime;
    public float matchTime;
    public int maxKillCount;

    private int team1Count = 0, team1DeadCount = 0, team1KillCount = 0;
    private int team2Count = 0, team2DeadCount = 0, team2KillCount = 0;
    private float matchTimeLeft;


    private Hashtable worldStatesStash;
    int lowesId = 0;

    private float runeSpawnTimeLeft;
    public List<RuneServerside> spawnedRunes;

    public void Awake()
    {
        instance = this;
        playerControls = new List<PlayerControl>();
        networkManager = GameObject.FindWithTag("NetworkManager").GetComponent<CustomNetworkManager>();
        reservelist = new List<int>();
        playerInfoList = new List<PlayerInfo>();
        deadPlayers = new List<PlayerInfo>();
        worldStatesStash = new Hashtable();
        maxClientCount = networkManager.maxPlayerCount;
        respawnTime = networkManager.baseRespawnTime;
        currentClientCount = 0;
        runeSpawnTimeLeft = networkManager.runeSpawnTime;
        spawnedRunes = new List<RuneServerside>();
        matchTime = networkManager.maxtime;
        maxKillCount = networkManager.maxkill;
        matchTimeLeft = matchTime * 60;
        Debug.Log(matchTimeLeft);
        UpdatePlayers();
    }

    // Use this for initialization
    void Start () {
        currentWorldState = new WorldState();
        UIStats.SetTime((int)matchTime * 60);
	}

    private void FixedUpdate()
    {
        
        for (int i = 0; i < deadPlayers.Count; i++){
            deadPlayers[i].respawnTimeLeft -= Time.fixedDeltaTime;
            if(deadPlayers[i].respawnTimeLeft <= 0){
                if (deadPlayers[i].teamId == 1)
                    team1DeadCount--;
                if (deadPlayers[i].teamId == 2)
                    team2DeadCount--;
                RespawnHero(deadPlayers[i]);
                for (int j = 0; j < playerControls.Count; j++){
                    if (playerControls[j].playerId == deadPlayers[i].clientId)
                        playerControls[j].Respawn();
                        
                }
                deadPlayers.RemoveAt(i);
                i--;
            }
        }
       // Debug.Log(matchTimeLeft);
        matchTimeLeft -= Time.fixedDeltaTime;
       // Debug.Log(matchTimeLeft);
        if(matchTimeLeft <= 0){
            if (team1KillCount > team2KillCount)
                SendGameFinishedCommand(1);
            else if (team1KillCount < team2KillCount)
                SendGameFinishedCommand(2);
            else
                SendGameFinishedCommand(0);
        }
        if (team1KillCount >= maxKillCount)
            SendGameFinishedCommand(1);
        else if (team2KillCount >= maxKillCount)
            SendGameFinishedCommand(2);
    }

    private void SpawnRune(){
        runeSpawnTimeLeft = networkManager.runeSpawnTime;
        if (spawnedRunes.Count == networkManager.runeSpawnPositions.Count)
            return;
        List<Vector2> availableSpawnPositions = new List<Vector2>();
        for (int i = 0; i < networkManager.runeSpawnPositions.Count; i++){
            bool available = true;
            for (int j = 0; j < spawnedRunes.Count; j++){
                if(Vector2.Distance(networkManager.runeSpawnPositions[i].position, spawnedRunes[j].spawnPosition) < 0.1){
                    available = false;
                    break;
                }
            }
            if (available)
                availableSpawnPositions.Add(networkManager.runeSpawnPositions[i].position);
        }
        int runenum = Random.Range(0, networkManager.runesServeside.Count);
        int runeposnum = Random.Range(0, availableSpawnPositions.Count);
        RuneServerside rune = Instantiate(networkManager.runesServeside[runenum]);
        rune.transform.position = availableSpawnPositions[runeposnum];
        spawnedRunes.Add(rune);
        currentWorldState.AdditionalWorldData("R" + "&" + runenum + "&" + Toolkit.VectorSerialize(availableSpawnPositions[runeposnum]));

    }

    private void SetWorldStateOnPlayers(){
        foreach(PlayerControl p in playerControls){
            p.worldState = currentWorldState;
        }
    }

    public void UpdatePlayers()
    {
        playerControls.Clear();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("VirtualPlayer");
        for (int i = 0; i < objs.Length; i++)
            playerControls.Add(objs[i].GetComponent<PlayerControl>());
        SetWorldStateOnPlayers();
    }

    public void PlayerSimulationFinished(int ID){
        finishedPLayercounter++;
        if(finishedPLayercounter == playerControls.Count){
            string rawData = currentWorldState.GetWorldData();
            ServerNetworkSender.instance.RegisterWorldState(rawData);
            finishedPLayercounter = 0;
            worldStatesStash.Add(CurrentStateID * 3 + ServerNetworkSender.instance.currentTime, currentWorldState);
            if (worldStatesStash.Count > 600)
            {
                worldStatesStash.Remove(lowesId);
                lowesId++;
            }
            currentWorldState = new WorldState();
            UpdatePlayers();

        }
    }
    
    public void SendWorldStateToClient(int playerID, int frameId){
        string tempWorldState = "";
        //Mathf.Min(CurrentStateID * 3, frameId + 5)
        for (int i = frameId; i < CurrentStateID * 3; i++){
            tempWorldState += worldStatesStash[i] + "!";
        }
        ServerNetworkSender.instance.SendWorldFullstate(tempWorldState, playerID, frameId);
    }

    public void SpawnHero(int clientId, int heroId, int teamId, Vector2 SpawnPoint)
    {
        for (int i = 0; i < networkManager.playerConnections.Count; i++)
        {
            Debug.Log(networkManager.playerConnections[i].clientId);
            Debug.Log(clientId);
            if (networkManager.playerConnections[i].clientId == clientId)
            {
                networkManager.playerConnections[i].RpcInstansiateHero(heroId, teamId, Toolkit.VectorSerialize(SpawnPoint));
                break;
            }
        }

    }


    public void ClientConnected(int clientId)
    {
        Debug.Log("id: " + clientId);
        for (int i = 0; i < networkManager.clientsData.Count; i++){
            if(networkManager.clientsData[i].id == clientId){
                playerInfoList.Add(new PlayerInfo(networkManager.clientsData[i].id, networkManager.clientsData[i].heroId, networkManager.clientsData[i].team, true));
            }
        }
        currentClientCount++;
        if(currentClientCount == networkManager.clientsData.Count){
            StartGame();
        }
    }

    public void StartGame()
    {
        //networkManager.gameObject.GetComponent<CustomNetworkDiscovery>().StopBroadcast();
        for (int i = 0; i < playerInfoList.Count; i++)
        {
            Debug.Log(playerInfoList[i].teamId - 1);
            Debug.Log(networkManager.heroSpawnPositions.Count);
            Debug.Log(networkManager.heroSpawnPositions[playerInfoList[i].teamId - 1].position);
            SpawnHero(playerInfoList[i].clientId, playerInfoList[i].heroId, playerInfoList[i].teamId, networkManager.heroSpawnPositions[playerInfoList[i].teamId - 1].position);
            if (playerInfoList[i].teamId == 1)
                team1Count++;
            else
                team2Count++;
        }
        UpdatePlayers();
        ClientNetworkReciever.instance.RpcUpdatePlayers();
    }

    /*public void HeroSpawned(int cliendId){
        spawnedHeroCount++;
        if(spawnedHeroCount == maxClientCount){ //spawn at the begining of the match
            for (int i = 0; i < maxClientCount; i++){
                networkManager.playerConnections[i].RpcSetReady();
            }
            spawnedHeroCount++; //for respawin
        }
        else if(spawnedHeroCount > maxClientCount){ //respawn
            for (int i = 0; i < maxClientCount; i++)
            {
                if (networkManager.playerConnections[i].clientId == cliendId)
                {
                    networkManager.playerConnections[i].RpcSetReady();
                    break;
                }
            }
        }
    }*/

    public int GetBulletID(int playerId)
    {
        return ++bulletIdCounter;
    }

    public void KillHero(int playerId){
        for (int i = 0; i < playerInfoList.Count; i++)
        {
            if (playerInfoList[i].clientId == playerId)
            {
                playerInfoList[i].isAlive = false;
                deadPlayers.Add(playerInfoList[i]);
                if (playerInfoList[i].teamId == 1)
                {
                    team1DeadCount++;
                    team2KillCount++;
                    UIStats.SetKill(2, team2KillCount);
                    playerInfoList[i].respawnTimeLeft = respawnTime;
                }
                else
                {
                    team2DeadCount++;
                    team1KillCount++;
                    UIStats.SetKill(1, team1KillCount);
                    playerInfoList[i].respawnTimeLeft = respawnTime;
                }
                SendKillCommand(playerId);
                break;

            }
        }
    }

    public void SendGameFinishedCommand(int winnerTeamId){
        for (int i = 0; i < networkManager.playerConnections.Count; i++){
            networkManager.playerConnections[i].RpcGameFinished(winnerTeamId);
        }
    }

    public void SendKillCommand(int playerId){
        currentWorldState.AdditionalPlayerData(playerId, "Z");
    }

    private void RespawnHero(PlayerInfo playerInfo){
        playerInfo.isAlive = true;
        currentWorldState.AdditionalPlayerData(playerInfo.clientId, "Y");
    }
}

class PlayerInfo{
    public int clientId, heroId, teamId;
    public bool isAlive;
    public float respawnTimeLeft;
    public PlayerConnection playerConnection;

    public PlayerInfo(int clientId, int heroId,int teamId , bool isAlive){
        this.heroId = heroId;
        this.clientId = clientId;
        this.isAlive = isAlive;
        respawnTimeLeft = 0;
        this.teamId = teamId;
    }
}

public enum GameMode{
    TeamDeathMatch, LastTeamStanding
}
