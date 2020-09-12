using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCountDisplay : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Text text;


    void Reset()
    {
        this.RenameFromType();
        text = GetComponent<Text>();
    }


    public override void OnJoinedRoom() => DisplayPlayerCount();

    public override void OnPlayerEnteredRoom(Player newPlayer) => DisplayPlayerCount();

    public override void OnPlayerLeftRoom(Player otherPlayer) => DisplayPlayerCount();

    void DisplayPlayerCount()
    {
        // Display the number of players in the room, not counting ourselves.
        text.text = (PhotonNetwork.CurrentRoom.PlayerCount + (PhotonNetwork.IsConnected ? -1 : 0)).ToString();
    }
}
