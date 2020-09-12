using Photon.Pun;
using UnityEngine;

public class OnPlayerConnectDisabler : MonoBehaviour
{
    private void Update()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1)
            gameObject.SetActive(false);
    }
}
