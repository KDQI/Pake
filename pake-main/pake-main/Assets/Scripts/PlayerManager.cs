using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject playerCanvas;
    [SerializeField]
    private GameObject teamSelectionCanvas;
    [SerializeField]
    private GameObject waitingCanvas;
    [SerializeField]
    private Material blueMat;
    [SerializeField]
    private Material redMat;
    [SerializeField]
    private GameObject body;

    private float playerCount;
    public Text playerCountText;

    [SyncVar(hook = nameof(ChangePlayerTag))]
    private string playerTag = "";

    private ServerManager serverManager;

    private CharacterController cc;

    public GameObject youRed;
    public GameObject youBlue;

    public Transform spawnPoint;

    private ScoreSystem scoreSystem;

    private void Start()
    {
        if (isLocalPlayer)
        {
            scoreSystem = GameObject.Find("ScoreManager").GetComponent<ScoreSystem>();
            teamSelectionCanvas.SetActive(true);
            //waitingCanvas.SetActive(true);
            CmdPlayerCount();
            cc = this.GetComponent<CharacterController>();
            serverManager = GameObject.Find("ServerManager").GetComponent<ServerManager>();
        }
    }

    [Client]
    private void Update()
    {
        if(isLocalPlayer)
        {
            if(gameObject.tag == "Blue")
            {
                CmdChangeBlueMat();
            } else if(gameObject.tag == "Red")
            {
                CmdChangeRedMat();
            }
        }
        playerCountText.text = playerCount.ToString("Player Count:  ") + GameManager.players.Count;
    }

    [Command]
    private void CmdPlayerCount()
    {
        RpcPlayerCount();
    }
    [ClientRpc]
    private void RpcPlayerCount()
    {
        playerCount++;
    }

    [Client]
    public void StartGame()
    {
        if(isLocalPlayer)
        {
            CmdStartGame();
        }
    }

    [Command]
    private void CmdStartGame()
    {
        RpcStartGame();
    }

    [ClientRpc]
    private void RpcStartGame()
    {
        waitingCanvas.SetActive(false);
        if(isLocalPlayer)
            teamSelectionCanvas.SetActive(true);
    }

    [Client]
    public void AssignToBlueTeam()
    {
        if(isLocalPlayer)
        {
            CmdChangeBlueMat();                        
            this.gameObject.tag = "Blue";
            youBlue.SetActive(true);
            cc.enabled = false;
            serverManager.IncreasePlayerCount();
            serverManager.IncreaseBluePlayerCount();
            playerCanvas.SetActive(true);
            teamSelectionCanvas.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            spawnPoint = serverManager.blueSpawnPoint;
            transform.position = spawnPoint.position;
            CmdChangePlayerTag("Blue");
            this.gameObject.tag = playerTag;
            cc.enabled = true;
            scoreSystem.SyncText();
        }
        //Vanha paska if-lause jos tarvitaa joskus
        //if (isLocalPlayer && serverManager.blueTeamCount < serverManager.redTeamCount || serverManager.blueTeamCount == serverManager.redTeamCount)
        //{
        //}
    }

    [Client]
    public void AssignToRedTeam()
    {
        if(isLocalPlayer)
        {
        CmdChangeRedMat();
        gameObject.tag = "Red";
        youRed.SetActive(true);
        cc.enabled = false;
        serverManager.IncreasePlayerCount();
        serverManager.IncreaseRedPlayerCount();
        playerCanvas.SetActive(true);
        teamSelectionCanvas.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        spawnPoint = serverManager.redSpawnPoint;
        transform.position = spawnPoint.position;
        CmdChangePlayerTag("Red");
        this.gameObject.tag = playerTag;
        cc.enabled = true;
        scoreSystem.SyncText();
        }
        //Vanha paska if-lause jos tarvitaa joskus
        //if (isLocalPlayer && serverManager.redTeamCount < serverManager.blueTeamCount || serverManager.blueTeamCount == serverManager.redTeamCount)
        //{
        //}
    }

    [Command]
    private void CmdChangePlayerTag(string tag)
    {
        playerTag = tag;
    }

    private void ChangePlayerTag(string oldTag, string newTag) {
        this.gameObject.tag = newTag;
    }

    [Command]
    private void CmdChangeBlueMat()
    {
        RpcChangeBlueMat();
    }

    [Command]
    private void CmdChangeRedMat()
    {
        RpcChangeRedMat();
    }

    [ClientRpc]
    private void RpcChangeBlueMat()
    {
        body.GetComponent<MeshRenderer>().material = blueMat;
    }

    [ClientRpc]
    private void RpcChangeRedMat()
    {
        body.GetComponent<MeshRenderer>().material = redMat;
    }

}
