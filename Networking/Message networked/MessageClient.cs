using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class MessageClient : MonoBehaviour
{
    PhotonView view;
    public bool dontDestroyOnLoad = true;
    public event System.Action<string> MessageReceived;

    public static MessageClient Instance { get; private set; }


    private void Reset()
    {
        this.RenameFromType();
    }

    private void Awake()
    {
        Instance = this;
        view = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }


    public void Send(string text)
    {
        view.RPC("Receive", RpcTarget.All, text);
    }

    [PunRPC]
    void Receive(string text)
    {
        MessageReceived?.Invoke(text);
    }
}
