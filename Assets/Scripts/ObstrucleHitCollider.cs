﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstrucleHitCollider : MonoBehaviour {

    public int damage = 1;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            GameObject player = other.gameObject;
            PlayerSeedController playerSeedControllerScript = player.GetComponent<PlayerSeedController>();
            playerSeedControllerScript.DecreaseSeed(damage);
        }
    }
}
