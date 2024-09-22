using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LevelNetworkManager : MonoBehaviourPunCallbacks
{
    public static LevelNetworkManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    public void disconnectFromCurrentRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    public override void OnPlayerEnteredRoom(Player newplayer)
    {
        Debug.Log("entro al room: " + newplayer.NickName);
        UIManager.Instance.addPrivateText(newplayer.NickName + " entered the room");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            UIManager.Instance.NextState();
        }
    }

    public override  void OnPlayerLeftRoom(Player otherplayer)
    {
        Debug.Log("salio: " + otherplayer.NickName);
        UIManager.Instance.addPrivateText(otherplayer.NickName + " left the room");
    }
}
