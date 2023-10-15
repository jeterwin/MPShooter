using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    public string LevelToPlay;
    [SerializeField] private GameObject startButton;

    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI errorText;

    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField inputField;

    [SerializeField] private TMP_Text playerNameLabel;

    [Header("Screen Gameobjects")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject createRoomScreen;
    [SerializeField] private GameObject roomScreen;
    [SerializeField] private GameObject errorScreen;
    [SerializeField] private GameObject roomBrowserScreen;
    [SerializeField] private GameObject nameInputScreen;
    [SerializeField] private GameObject menuButtons;

    [Header("Display All Rooms")]
    [SerializeField] private RoomButton theRoomButton;
    [SerializeField] private List<RoomButton> allRoomButtons = new();

    private List<TMP_Text> allPlayerNames = new();

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        closeMenus();
        loadingScreen.SetActive(true);
        loadingText.text = "Connecting to server...";

        PhotonNetwork.ConnectUsingSettings();
    }

    #region Overrides

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

        PhotonNetwork.AutomaticallySyncScene = true;

        loadingText.text = "Joining Lobby...";
    }

    public override void OnJoinedLobby()
    {
        closeMenus();
        menuButtons.SetActive(PlayerPrefs.HasKey("playerName"));
        nameInputScreen.SetActive(!PlayerPrefs.HasKey("playerName"));

        PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");
    }

    public override void OnJoinedRoom()
    {
        closeMenus();

        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        roomScreen.SetActive(true);

        ListAllPlayers();

        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        closeMenus();
        errorText.text = "Failed to create room: " + message;
        errorScreen.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
        newPlayerLabel.text = newPlayer.NickName;
        newPlayerLabel.gameObject.SetActive(true);

        allPlayerNames.Add(newPlayerLabel);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ListAllPlayers();
    }

    public override void OnLeftRoom()
    {
        closeMenus();
        menuButtons.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomButton rb in allRoomButtons)
        {
            Destroy(rb.gameObject);
        }
        allRoomButtons.Clear();

        theRoomButton.gameObject.SetActive(false);

        for(int i = 0; i < roomList.Count; i++)
        {
            if(roomList[i].PlayerCount >= roomList[i].MaxPlayers || roomList[i].RemovedFromList) { return; }
            {
                RoomButton roomButton = Instantiate(theRoomButton, theRoomButton.transform.parent);
                roomButton.SetButtonDetails(roomList[i]);
                roomButton.gameObject.SetActive(true);

                allRoomButtons.Add(roomButton);
            }
        }
    }
    #endregion

    #region Methods
    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(inputField.text)) { return; }

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;

        PhotonNetwork.CreateRoom(inputField.text, options);

        closeMenus();

        loadingScreen.SetActive(true);
        loadingText.text = "Creating Room...";
    }

    public void SetNickname()
    {
        if(string.IsNullOrEmpty(nameInputField.text)) { return; }

        PhotonNetwork.NickName = nameInputField.text;

        PlayerPrefs.SetString("playerName", nameInputField.text);

        closeMenus();
        menuButtons.SetActive(true);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(LevelToPlay);
    }

    public void JoinRoom(RoomInfo roomInfo)
    {
        PhotonNetwork.JoinRoom(roomInfo.Name);

        closeMenus();
        loadingText.text = "Joining Room: " + roomInfo.Name;
        loadingScreen.SetActive(true);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    private void ListAllPlayers()
    {
        foreach(TMP_Text player in allPlayerNames)
        {
            Destroy(player.gameObject);
        }
        allPlayerNames.Clear();

        Player[] players = PhotonNetwork.PlayerList;

        for(int i = 0; i < players.Length; i++)
        {
            TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
            newPlayerLabel.text = players[i].NickName;
            newPlayerLabel.gameObject.SetActive(true);

            allPlayerNames.Add(newPlayerLabel);
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        closeMenus();
        loadingText.text = "Leaving Room...";
        loadingScreen.SetActive(true);
    }

    void closeMenus()
    {
        loadingScreen.SetActive(false);
        menuButtons.SetActive(false);
        createRoomScreen.SetActive(false);
        roomScreen.SetActive(false);
        errorScreen.SetActive(false);
        roomBrowserScreen.SetActive(false);
        nameInputScreen.SetActive(false);
    }

    #endregion
}
