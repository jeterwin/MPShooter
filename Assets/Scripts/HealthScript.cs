using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    [SerializeField] private int health;
    public int Health
    {
        get { return health; }
    }
    [PunRPC]
    public void TakeDamage(int damage)
    {
        health -= damage;
    }
}
