using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    private const string playerIdPrefix = "Player ";
    public static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public static void RegisterPlayer(string _netId, Player _player)
    {
        string _playerID = "Player " + _netId;
        players.Add(_playerID, _player);
        _player.transform.name = _playerID;
    }

    public static void UnRegisterPlayer(string _playerId)
    {
        players.Remove(_playerId);
    }

    public static Player GetPlayerWithId(string id)
    {
        return players[id];
    }

    private void Update()
    {
        //Debug.Log(players.Count);
    }

    //private void OnGUI()
    //{
    //    GUILayout.BeginArea(new Rect(200, 200, 200, 500));
    //    GUILayout.BeginVertical();
    //    foreach(string id in players.Keys)
    //    {
    //        GUILayout.Label(id + " - " + players[id].transform.name);
    //    }
    //    GUILayout.EndVertical();
    //    GUILayout.EndArea();
    //}
}
