using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerManager : NetworkBehaviour
{
    [SyncVar] public int playerAmount;
    [SyncVar] public int blueTeamCount;
    [SyncVar] public int redTeamCount;
    public Transform redSpawnPoint;
    public Transform blueSpawnPoint;


    public void IncreasePlayerCount()
    {
        playerAmount++;
    }

    public void IncreaseBluePlayerCount()
    {
        blueTeamCount++;
    }

    public void IncreaseRedPlayerCount()
    {
        redTeamCount++;
    }
}
