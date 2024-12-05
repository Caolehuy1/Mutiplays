using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using System.Linq;
using Unity.VisualScripting;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] InputField roomNameInputField;
    [SerializeField] Text roomNameText;
    [SerializeField] Text errorText;

    [SerializeField] Transform roomlistContent;
    [SerializeField] GameObject roomlistItemPrefab;

    [SerializeField] Transform playerlistContent;
    [SerializeField] GameObject playerlistItemPrefab;

    private int nextTeamNumber = 1;
    public GameObject startButton;
    private void Awake()
    {
        Instance = this;    
    }
    private void Start()
    {
        Debug.Log("Connecting to Master ...");

         PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster()
    {

        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.instance.OpenMenu("UserNameMenu");
        Debug.Log("Joined Lobby");
       

    }
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.instance.OpenMenu("loadingMenu");

    }

    public override void OnJoinedRoom()
    {
        MenuManager.instance.OpenMenu("RoomMenu");

        roomNameText.text = PhotonNetwork.CurrentRoom.Name;


        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in playerlistContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++) 
        {
            int teamnumber = GetNextTeamNumber();
             Instantiate(playerlistItemPrefab, playerlistContent).GetComponent<PlayerListItem>().Setup(players[i],teamnumber);
        }

        startButton.SetActive(PhotonNetwork.IsMasterClient);

    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    public override void OnCreateRoomFailed(short returnCode, string errorMessage)
    {
        errorText.text = "Room Generation Unsuccesfull" + errorMessage;

        MenuManager.instance.OpenMenu("ErrorMenu");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("LoadingMenu");
    }
    public void StarGame()
    {
        PhotonNetwork.LoadLevel(1);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("LoadingMenu");
    }
    public override void OnLeftRoom()
    {
        MenuManager.instance.OpenMenu("TitleMenu");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in roomlistContent)
        {
            Destroy(trans.gameObject);
        }
        for(int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomlistItemPrefab, roomlistContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        int teamNumber = GetNextTeamNumber();

        GameObject playerItem = Instantiate(playerlistItemPrefab, playerlistContent);
        playerItem.GetComponent<PlayerListItem>().Setup(newPlayer, teamNumber);
    }
    private int GetNextTeamNumber()
    {
        int teamNumber = nextTeamNumber;
        nextTeamNumber = 3 - nextTeamNumber;
        return teamNumber;
    }
}
