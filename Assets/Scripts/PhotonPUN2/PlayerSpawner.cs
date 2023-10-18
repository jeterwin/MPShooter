using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance;

    [SerializeField] private float respawnTime = 3f;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject deathParticlesPrefab;

    private GameObject player;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (!PhotonNetwork.IsConnected) { return; }

        SpawnPlayer();
    }

    public void Die(string killerName)
    {
        if(player == null) { return; }

        MatchManager.Instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);

        StartCoroutine(DieCo());
    }
    IEnumerator DieCo()
    {
        PhotonNetwork.Instantiate(deathParticlesPrefab.name, player.transform.position, Quaternion.identity);

        PhotonNetwork.Destroy(player);

        yield return new WaitForSeconds(respawnTime);

        SpawnPlayer();

    }
    private void SpawnPlayer()
    {
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();

        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
}
