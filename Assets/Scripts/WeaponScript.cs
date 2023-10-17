using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviourPunCallbacks
{
    private InputHandler inputHandler;
    private CameraScript cameraScript;

    [Header("Gun Stats")]
    [SerializeField] private int damage = 10;
    [SerializeField] private int magSize = 7;
    [SerializeField] private int magAmount = 3;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float reloadTime = 2f;

    [Header("Object References")]
    [SerializeField] private Transform muzzleFlashLocation;

    [Header("Prefab References")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private GameObject playerHitImpactPrefab;
    [SerializeField] private GameObject bulletImpactPrefab;

    [Header("Settings")]
    [SerializeField] private float deleteBulletHoleTime = 2f;
    [SerializeField] private float deleteMuzzleFlashTime = 1f;

    [Range(0f, 10f)]
    private int remainingBullets;
    private int remainingMags;
    private float remainingShootTime = 0;
    private float remainingReloadTime = 0;
    private bool isReloading = false;
    private bool canShoot = true;
    private bool canReload = true;
    private new void OnEnable()
    {
        isReloading = false;
        canReload = true;
    }
    void Start()
    {
        remainingBullets = magSize;
        remainingMags = magAmount;
        inputHandler = GetComponentInParent<InputHandler>();
        cameraScript = GetComponentInParent<CameraScript>();
    }
    void Update()
    {
        if(photonView.IsMine)
        {
            if(remainingShootTime > 0f)
                UpdateTimers();
            else
            {
                canShoot = true;
            }

            if(!canShoot || (remainingBullets == 0 && remainingMags == 0) || isReloading) { return; }

            if((inputHandler.PressedReload || remainingBullets == 0) && remainingMags > 0 && canReload && canShoot)
            {
                StartCoroutine(Reload());
            }
            if (inputHandler.PressedShoot && !isReloading)
            {
                Fire();
            }
        }
    }

    private IEnumerator Reload()
    {        
        isReloading = true;
        canReload = false;

        yield return new WaitForSeconds(reloadTime);

        canReload = true;
        isReloading = false;
        remainingMags--;
        remainingBullets = magSize;
    }

    private void UpdateTimers()
    {
        remainingShootTime -= Time.deltaTime;
    }

    private void Fire()
    {
        canShoot = false;
        remainingBullets -= 1;
        remainingShootTime = fireRate;

        CreateMuzzleFlash();

        Ray ray = cameraScript.Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        ray.origin = cameraScript.Camera.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if(hit.collider.CompareTag("Player"))
            {
                PhotonNetwork.Instantiate(playerHitImpactPrefab.name, hit.point, Quaternion.identity);
                
                hit.collider.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, photonView.Owner.NickName, damage);
            }
            else
            {
                PhotonNetwork.Instantiate(bulletImpactPrefab.name, hit.point + (0.002f * hit.normal), Quaternion.LookRotation(hit.normal));
            }

        }
    }

    private void CreateMuzzleFlash()
    {
        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzleFlashLocation);

        Destroy(muzzleFlash, deleteMuzzleFlashTime);
    }
}
