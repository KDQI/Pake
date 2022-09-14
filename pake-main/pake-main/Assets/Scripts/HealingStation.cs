using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HealingStation : NetworkBehaviour
{
    [SerializeField] private string healTag;
    [SerializeField] private float healInterval;
    private float nextHeal;
    private List<Player> players = new List<Player>();

    private void Update()
    {
        if(players.Count > 0)
        {
            if(Time.time > nextHeal)
            {
                foreach(Player p in players)
                {
                    p.HealPlayer(1);
                }
                nextHeal = Time.time + healInterval;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

            Debug.Log("Collided with something");
        if(collision.gameObject.CompareTag(healTag))
        {
            Debug.Log("Adding player..");
            players.Add(collision.gameObject.GetComponent<Player>());         
            collision.gameObject.GetComponent<Player>().StartPlayHealSound();            
        }
    }

    private void OnCollisionExit(Collision collision)
    {               
             
        Debug.Log("Exit collision with something..");
        if(collision.gameObject.CompareTag(healTag))
        {
            Debug.Log("Removing player..");
            players.Remove(collision.gameObject.GetComponent<Player>());
            collision.gameObject.GetComponent<Player>().StopPlayHealSound();
        }
    }
}
