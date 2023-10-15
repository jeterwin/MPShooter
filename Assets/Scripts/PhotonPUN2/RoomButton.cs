using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Realtime;

public class RoomButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomName;

    private RoomInfo roomInfo;

    public void SetButtonDetails(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;

        roomName.text = roomInfo.Name;
    }

    public void OpenRoom()
    {
        Launcher.Instance.JoinRoom(roomInfo);
    }
}
