using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance;
    [SerializeField] private GameObject playerPrefab;
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

    private void SpawnPlayer()
    {
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();

        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
}
