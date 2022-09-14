using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Mirror;

public class ScoreSystem : NetworkBehaviour
{
    [SyncVar]
    public float RedPoints;
    [SyncVar]
    public float BluePoints;
    public GameObject CarryingRedMsg;
    public GameObject RedPointMsg;
    public GameObject CarryingBlueMsg;
    public GameObject BluePointMsg;
    [SerializeField]
    private Text BlueScoreCounter;
    [SerializeField]
    private Text RedScoreCounter;


    private void Start()
    {
    }

    [Client]
    public void SyncText()
    {
        BlueScoreCounter.text = BluePoints.ToString();
        RedScoreCounter.text = RedPoints.ToString();
    }

    private void Update()
    {
        BlueScoreCounter.text = BluePoints.ToString();
        RedScoreCounter.text = RedPoints.ToString();
        //Debug.Log("Red Points: " + RedPoints + ", Blue Points: " + BluePoints);
    }

    [Command(ignoreAuthority = true)]
    public void CmdUpdatePointsText(int team)
    {
        RpcUpdateCarryingMessage(team);
    }

    [Command(ignoreAuthority = true)]
    public void CmdUpdateCarryingText(int team)
    {
        RpcUpdateCarryingMessage(team);
    }

    [Command(ignoreAuthority = true)]
    public void CmdIncreaseRedPoints()
    {
        RedPoints++;
        RpcUpdatePointMessage(1);
    }


    [Command(ignoreAuthority = true)]
    public void CmdIncreaseBluePoints()
    {
        BluePoints++;
        RpcUpdatePointMessage(0);
    }

    [Command(ignoreAuthority = true)]
    public void CmdEnableFlag(int team, GameObject target)
    {
        target.GetComponent<FlagCapture>().RpcEnableFlag(team);
    }

    [Command(ignoreAuthority = true)]
    public void CmdDisableFlag(int team, GameObject target)
    {
        target.GetComponent<FlagCapture>().RpcDisableFlag(team);
    }

    [Command(ignoreAuthority = true)]
    public void CmdDisableCarryingMsg(int team)
    {
        RpcDisableCarryingMessage(team);
    }

    [ClientRpc]
    public void RpcUpdatePointMessage(int team)
    {
        if (team == 0)
        {
            Debug.Log("PISTE WOOHOOO!");
            CarryingRedMsg.SetActive(false);
            BluePointMsg.SetActive(true);
            StartCoroutine(DisableBlueScoreText());
        }
        else
        {
            CarryingBlueMsg.SetActive(false);
            Debug.Log("PISTE WOOHOOO!");
            CarryingBlueMsg.SetActive(false);
            RedPointMsg.SetActive(true);
            StartCoroutine(DisableRedScoreText());
        }
    }

    [ClientRpc]
    public void RpcUpdateCarryingMessage(int team)
    {
        if (team == 0)
        {
            CarryingBlueMsg.SetActive(true);
        }
        else
        {
            CarryingRedMsg.SetActive(true);
        }
    }

    [ClientRpc]
    public void RpcDisableCarryingMessage(int team)
    {
        if(team == 0)
        {
            CarryingBlueMsg.SetActive(false);
        } else
        {
            CarryingRedMsg.SetActive(false);
        }
    }

    IEnumerator DisableBlueScoreText()
    {
        yield return new WaitForSeconds(5);
        BluePointMsg.SetActive(false);
    }

    IEnumerator DisableRedScoreText()
    {
        yield return new WaitForSeconds(5);
        RedPointMsg.SetActive(false);
    }
}
