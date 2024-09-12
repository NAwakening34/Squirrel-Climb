using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class PhotinConnection : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField m_newInputField;
    [SerializeField] TextMeshProUGUI m_textMeshProUGUI;
    [SerializeField] TMP_InputField m_newInputFieldNickname;
    private bool saved;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("se ha conectado al master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Se ha entrado al lobby Abstracto");
        //PhotonNetwork.JoinOrCreateRoom("TestRoom", NewRoomInfo(), null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("se entro al room");
        PhotonNetwork.LoadLevel("SampleScene");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.LogWarning("Hubo un erro al crear un room: " + message);
        m_textMeshProUGUI.text = "Hubo un error al crear el room " + m_newInputField.text;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.LogWarning("Hubo un erro al entrar: " + message);
        m_textMeshProUGUI.text = "Hubo un erro al entrar al room " + m_newInputField.text;
    }

    RoomOptions NewRoomInfo()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        return roomOptions;
    }

    public void JoinRoom()
    {
        if (m_newInputField.text == "")
        {
            Debug.LogWarning("Tiene que dar un nombre al cuarto primero");
            m_textMeshProUGUI.text = "Tiene que dar un nombre al cuarto primero";
            return;
        }
        if (!saved)
        {
            m_textMeshProUGUI.text = "Tienes que guardar tu nickname primero";
            return;
        }
        PhotonNetwork.NickName = m_newInputFieldNickname.text;
        PhotonNetwork.JoinRoom(m_newInputField.text);
    }

    public void CreateRoom()
    {
        if (m_newInputField.text == "")
        {
            Debug.LogWarning("Tiene que dar un nombre al cuarto primero");
            m_textMeshProUGUI.text = "Tiene que dar un nombre al cuarto primero";
            return;
        }
        if (!saved)
        {
            m_textMeshProUGUI.text = "Tienes que guardar tu nickname primero";
            return;
        }
        PhotonNetwork.CreateRoom(m_newInputField.text, NewRoomInfo(), null);
    }

    public void SaveNickName()
    {
        if (m_newInputFieldNickname.text == "")
        {
            m_textMeshProUGUI.text = "Tienes que poner tu nickname primero";
            return;
        }
        PhotonNetwork.NickName = m_newInputFieldNickname.text;
        m_textMeshProUGUI.text = "Nickname Guardado";
        saved = true;
    }
}
