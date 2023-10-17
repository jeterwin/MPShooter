using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviourPunCallbacks
{
    [SerializeField] private int health = 100;
    private int maxHealth = 100;
    public int Health
    {
        get { return health; }
    }
    private void Start()
    {
        if(photonView.IsMine)
        {
            health = maxHealth;
            UIController.Instance.UpdateHealth(health);
        }
    }
    [PunRPC]
    public void TakeDamage(string damager, int damage)
    {
        if(photonView.IsMine)
        {
            health -= damage;
            if(health <= 0)
            {
                health = 0;
                PlayerSpawner.Instance.Die(damager);
            }
            
            UIController.Instance.UpdateHealth(health);
        }
    }
}
