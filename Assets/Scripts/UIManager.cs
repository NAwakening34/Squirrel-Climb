using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameStates
{
    None,
    Timer,
    Playing,
    Ending
}
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameStates State;

    [SerializeField] 
    TextMeshProUGUI m_Timer, m_chat, m_winnertext; 
    float m_timer = 3;
    [SerializeField]
    int m_players, m_deathplayers;
    PhotonView m_pv;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    private void Start()
    {
        m_pv = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (State == GameStates.Timer)
        {
            Debug.Log("en Timer");
            if (m_timer > 0)
            {
                m_pv.RPC("Timer", RpcTarget.All);
            }
            else if (m_timer < 0)
            {
                m_pv.RPC("SetUp", RpcTarget.AllBuffered);
                NextState();
            }
        }
    }

    public void leaveCurrentRoomFromEditor()
    {
        State = GameStates.None;
        LevelNetworkManager.Instance.disconnectFromCurrentRoom();
    }

    public void addText(string message)
    {
        m_pv.RPC("UpdateChat", RpcTarget.All,  message);
        Debug.Log(message);
    }

    public void addPrivateText(string message)
    {
        m_chat.text = m_chat.text + message + "\n";
        Invoke("removeText", 2f);
    }

    public void PlayerDied()
    {
        m_pv.RPC("UpdateDeaths", RpcTarget.All);
    }

    public void Winner(string nickname)
    {
        Debug.Log("alguien gano");
        m_pv.RPC("ShowWinner", RpcTarget.All, nickname);
    }

    public void NextState()
    {
        m_pv.RPC("ChangeState", RpcTarget.AllBuffered);
        Debug.Log(State);
    }

    [PunRPC]
    void ChangeState()
    {
        State++;
    }

    [PunRPC]
    void SetUp()
    {
        if (m_pv.IsMine)
        {
            PhotonNetwork.Instantiate("TowerSpawner", transform.position, Quaternion.identity);
        }
        m_Timer.text = null;
        m_players = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    [PunRPC]
    void UpdateChat(string message)
    {
        m_chat.text = m_chat.text + message + "\n";
        Invoke("removeText", 2f);
    }

    private void removeText()
    {
        m_chat.text = null;
    }

    [PunRPC]
    void Timer()
    {
        m_timer -= Time.deltaTime;
        int time = (int)m_timer;
        m_Timer.text = "Game Starts in " + time.ToString();
    }

    [PunRPC]
    void UpdateDeaths()
    {
        m_deathplayers++;
        if (m_deathplayers == (m_players - 1))
        {
            NextState();
        }
    }

    [PunRPC]
    void ShowWinner(string nickname)
    {
        m_winnertext.text = nickname + "\n WON";
        State = GameStates.None;
        SpawnSecment.Instance.CancelInvoke();
    }
}
