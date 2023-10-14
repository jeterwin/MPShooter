using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwitcher : MonoBehaviour
{
    [SerializeField] private List<WeaponScript> weaponScripts;

    private InputHandler inputHandler;
    private int currentWeapon = 0;
    private void Start()
    {
        inputHandler = GetComponentInParent<InputHandler>();
        SwitchGuns();
    }
    private void Update()
    {
        if(currentWeapon != inputHandler.NextGun)
            SwitchGuns();
    }

    private void SwitchGuns()
    {
        currentWeapon = inputHandler.NextGun;
        for(int i = 0; i < weaponScripts.Count; i++)
        {
            if(i == currentWeapon)
            {
                weaponScripts[currentWeapon].gameObject.SetActive(true);
                currentWeapon = inputHandler.NextGun;
            }
            else
            {
                weaponScripts[i].gameObject.SetActive(false);
            }
        }
    }
}
