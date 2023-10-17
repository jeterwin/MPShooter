using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwitcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<WeaponScript> weaponScripts;

    private InputHandler inputHandler;
    private int currentWeapon = 0;
    private void Awake()
    {
        inputHandler = GetComponentInParent<InputHandler>();
    }
    private void Start()
    {
        photonView.RPC("ChangeGunOnNetwork", RpcTarget.All);
    }
    private void Update()
    {
        if(photonView.IsMine)
        {
            if(currentWeapon != inputHandler.NextGun)
            {
                photonView.RPC("ChangeGunOnNetwork", RpcTarget.All);
            }
        }
    }
    [PunRPC]
    void ChangeGunOnNetwork()
    {
        currentWeapon = inputHandler.NextGun;
        SwitchGuns();
    }
    private void SwitchGuns()
    {
        for(int i = 0; i < weaponScripts.Count; i++)
        {
            if(i == currentWeapon)
            {
                weaponScripts[currentWeapon].gameObject.SetActive(true);
            }
            else
            {
                weaponScripts[i].gameObject.SetActive(false);
            }
        }
    }
}
