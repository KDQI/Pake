using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    Camera mainCam;
    private PlayerManager playerManager;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
            foreach(Behaviour b in componentsToDisable)
            {
                b.enabled = false;
            }
        } else
        {
            gameObject.layer = LayerMask.NameToLayer("LocalPlayer");
            mainCam = Camera.main;
            if (mainCam != null)
                mainCam.gameObject.SetActive(false);
        }

        GetComponent<Player>().Setup();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();
        GameManager.RegisterPlayer(_netID, _player);
        CmdAnnounceName(this.gameObject, _netID);
    }

    [Command]
    public  void CmdAnnounceName(GameObject player, string netId)
    {
        player.transform.name = "Player " + netId;
    }

    private void OnDisable()
    {
        if(mainCam != null)
        {
            mainCam.gameObject.SetActive(true);
        }

        GameManager.UnRegisterPlayer(transform.name);
    }
}
