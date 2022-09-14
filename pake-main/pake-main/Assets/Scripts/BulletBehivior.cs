using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BulletBehivior : NetworkBehaviour
{
    public int dmgAmount;
    private void Start()
    {
        Destroy(gameObject, 5);
    }

}

