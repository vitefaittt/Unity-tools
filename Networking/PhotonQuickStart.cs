using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonQuickStart : MonoBehaviourPunCallbacks
{
    [SerializeField]
    bool connectOnStart = true;
    bool isConnecting;
    public byte maxPlayers = 5;


    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        if (connectOnStart)
            Connect();
    }


    public void Connect()
    {
        isConnecting = true;

        if (PhotonNetwork.IsConnected)
        {
            print("Joining Room...");
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            print("Connecting...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            print("Connected to master. Joining room...");
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("Join random room failed. Creating a room...");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayers });
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected, " + cause);
        isConnecting = false;
    }

    public override void OnJoinedRoom()
    {
        print("Joined room \""+PhotonNetwork.CurrentRoom.Name+ "\" with " + PhotonNetwork.CurrentRoom.PlayerCount + " player(s)");
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }
}
