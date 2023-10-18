using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;
using TMPro;

public class MatchManager : MonoBehaviour, IOnEventCallback
{
    public static MatchManager Instance;

    private List<PlayerInfo> allPlayers = new();
    private List<LeaderboardPlayer> lboardPlayers = new();

    private int index;
    public int Index
    {
        get { return index; } 
    }
    public List<PlayerInfo> AllPlayers
    {
        get { return allPlayers; }
    }

    private EventCodes theEvent;

    public enum EventCodes : byte
    {
        NewPlayer,
        ListPlayers,
        UpdateStats
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(UIController.Instance.leaderboard.activeInHierarchy)
            {
                UIController.Instance.leaderboard.SetActive(false);
            }
            else
            {
                ShowLeaderboard();
            }
        }
    }
    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code < 200)
        {
            EventCodes eventCode = (EventCodes)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;

            switch(eventCode)
            {
                case EventCodes.NewPlayer:
                    NewPlayerReceive(data);
                    break;

                case EventCodes.ListPlayers:
                    PlayerListReceive(data);
                    break;

                case EventCodes.UpdateStats:
                    UpdateStatsReceive(data);
                    break;
            }
        }
    }
    public void NewPlayerSend(string username)
    {
        object[] package = new object[4];

        package[0] = username;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[2] = 0;
        package[3] = 0;

        PhotonNetwork.RaiseEvent((byte)EventCodes.NewPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
            );
    }
    public void NewPlayerReceive(object[] data)
    {
        PlayerInfo player = new PlayerInfo((string)data[0], (int)data[1], (int)data[2], (int)data[3]);

        allPlayers.Add(player);

        PlayerListSend();
    }
    public void PlayerListSend()
    {
        object[] package = new object[allPlayers.Count];

        for(int i =  0; i < allPlayers.Count; i++) 
        {
            object[] piece = new object[4];

            piece[0] = allPlayers[i].name;
            piece[1] = allPlayers[i].actor;
            piece[2] = allPlayers[i].kills;
            piece[3] = allPlayers[i].deaths;

            package[i] = piece;
        }
        PhotonNetwork.RaiseEvent((byte)EventCodes.ListPlayers,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
            );
    }
    public void PlayerListReceive(object[] data)
    {
        allPlayers.Clear();

        for(int i = 0; i < data.Length; i++)
        {
            object[] piece = (object[])data[i];

            PlayerInfo player = new PlayerInfo(
                (string)piece[0],
                (int)piece[1],
                (int)piece[2],
                (int)piece[3]
                );
            allPlayers.Add(player);

            if(PhotonNetwork.LocalPlayer.ActorNumber == player.actor)
            {
                index = i;
            }
        }
    }
    public void UpdateStatsSend(int actorSending, int statToUpdate, int amountToChange)
    {
        object[] package = new object[]
        {
            actorSending, statToUpdate, amountToChange
        };

        PhotonNetwork.RaiseEvent((byte)EventCodes.UpdateStats,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
            );
    }
    public void UpdateStatsReceive(object[] data)
    {
        int actor = (int)data[0];
        int statType = (int)data[1];
        int amount = (int)data[2];

        for(int i = 0; i < allPlayers.Count; i++)
        {
            if(allPlayers[i].actor == actor)
            {
                switch(statType)
                {
                    case 0: //Kills
                        allPlayers[i].kills += amount;
                        break;
                    case 1: //Kills
                        allPlayers[i].deaths += amount;
                        break;
                }

                if(i == index)
                {
                    UIController.Instance.UpdateStatsDisplay(allPlayers[i]);
                }

                break;
            }
        }
    }
    void ShowLeaderboard()
    { 
        UIController.Instance.leaderboard.SetActive(true);
        
        foreach(LeaderboardPlayer lp in lboardPlayers)
        {
            Destroy(lp.gameObject);
        }

        lboardPlayers.Clear();

        UIController.Instance.leaderboardPlayerDisplay.gameObject.SetActive(false);

        foreach(PlayerInfo player in allPlayers)
        {
            LeaderboardPlayer lp = Instantiate(UIController.Instance.leaderboardPlayerDisplay,
                UIController.Instance.leaderboardPlayerDisplay.transform.parent);

            lp.SetDetails(player.name, player.kills, player.deaths);

            lp.gameObject.SetActive(true);

            lboardPlayers.Add(lp);
        }
    }

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
        else
        {
            NewPlayerSend(PhotonNetwork.NickName);
        }
    }
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}

[Serializable]
public class PlayerInfo
{
    public string name;
    public int actor;
    public int kills;
    public int deaths;
    public PlayerInfo(string _name, int _actor, int _kills, int _deaths)
    {
        name = _name;
        actor = _actor;
        kills = _kills;
        deaths = _deaths;
    }
}
