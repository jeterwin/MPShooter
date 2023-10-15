using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if(!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
        }
    }
}
