using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;

public class FlagCapture : NetworkBehaviour
{
    public GameObject CarryingRed;
    public GameObject CarryingBlue;
    public bool CarryingRedFlag = false;
    public bool CarryingBlueFlag = false;
    private ScoreSystem scoreSystem;

    private void Start()
    {
        scoreSystem = GameObject.Find("ScoreManager").GetComponent<ScoreSystem>();
        if(isLocalPlayer)
        {
            CarryingBlue.layer = LayerMask.NameToLayer("Default");
            CarryingRed.layer = LayerMask.NameToLayer("Default");
        }
    }

    private void Update()
    {
        if (isLocalPlayer)
        {

            if (scoreSystem.RedPoints >= 5 || scoreSystem.BluePoints >= 5)
            {
                NetworkManager.singleton.StopClient();
            }
        }

    }
    [Command]
    void CmdPlayEffects(string soundName)
    {
        RpcPlayEffects(soundName);
    }

    [ClientRpc]
    void RpcPlayEffects(string soundName)
    {
        AudioManager.instance.Play(soundName);
    }

    [Client]
    private void OnCollisionEnter(Collision col)
    {
        if (isLocalPlayer)
        {
            if (col.gameObject.CompareTag("RedFlag"))
            {
                Debug.Log("PUNAINEN LIPPU!");
                if (gameObject.tag == "Blue")
                {
                    CarryingRedFlag = true;
                    scoreSystem.CmdEnableFlag(1, this.gameObject);
                    scoreSystem.CmdUpdateCarryingText(1);
                    CmdPlayEffects("RedTaken");
                }
                else if (CarryingBlueFlag == true)
                {
                    CarryingBlueFlag = false;
                    scoreSystem.CmdDisableFlag(0, this.gameObject);
                    scoreSystem.CmdIncreaseRedPoints();
                    CmdPlayEffects("RedPoint");
                }
            }
            if (col.gameObject.CompareTag("BlueFlag"))
            {
                Debug.Log("SININEN LIPPU!");
                if (gameObject.tag == "Red")
                {
                    CarryingBlueFlag = true;
                    scoreSystem.CmdEnableFlag(0, this.gameObject);
                    scoreSystem.CmdUpdateCarryingText(0);
                    CmdPlayEffects("BlueTaken");
                }
                else if (CarryingRedFlag == true)
                {
                    CarryingRedFlag = false;
                    scoreSystem.CmdDisableFlag(1, this.gameObject);
                    scoreSystem.CmdIncreaseBluePoints();
                    CmdPlayEffects("BluePoint");
                }
            }
        }
    }

    [Client]
    public void CheckDead()
    {
        if (GetComponent<Player>().isDead)
        {
            if (isLocalPlayer)
            {
                if (CarryingRedFlag)
                {
                    scoreSystem.CmdDisableCarryingMsg(1);
                    scoreSystem.CmdDisableFlag(1, this.gameObject);
                }
                else if (CarryingBlue)
                {
                    scoreSystem.CmdDisableCarryingMsg(0);
                    scoreSystem.CmdDisableFlag(0, this.gameObject);
                }
                CarryingRedFlag = false;
                CarryingBlueFlag = false;
            }
        }
    }

    //private void OnCollisionExit(Collision col)
    //{
    //    if (col.gameObject.CompareTag("BlueFlag"))
    //    {
    //    }

    //    if (col.gameObject.CompareTag("RedFlag"))
    //    {
    //    }
    //}

    [ClientRpc]
    public void RpcEnableFlag(int team)
    {
        if (team == 0)
        {
            CarryingBlue.SetActive(true);
        }
        else
        {
            CarryingRed.SetActive(true);
        }
    }

    [ClientRpc]
    public void RpcDisableFlag(int team)
    {
        if (team == 0)
        {
            CarryingBlue.SetActive(false);
        }
        else
        {
            CarryingRed.SetActive(false);
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if(isLocalPlayer)
        {
            if (scoreSystem.RedPoints == 5 && scoreSystem.BluePoints == 5)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                AudioManager.instance.StopAll();
                SceneManager.LoadScene("Draw");
                

            }
            else if (scoreSystem.RedPoints >= 5)
            {
                if (gameObject.tag == "Red")
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    AudioManager.instance.StopAll();
                    SceneManager.LoadScene("Win");
                }
                else if (gameObject.tag == "Blue")
                {                    
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    AudioManager.instance.StopAll();
                    SceneManager.LoadScene("Lost");
                }
            }
            else if (scoreSystem.BluePoints >= 5)
            {
                if (gameObject.tag == "Blue")
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    AudioManager.instance.StopAll();
                    SceneManager.LoadScene("Win");
                }
                else if (gameObject.tag == "Red")
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    AudioManager.instance.StopAll();
                    SceneManager.LoadScene("Lost");
                }
            }
        }
    }

}
