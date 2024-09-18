using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] TextMeshProUGUI m_Timer;
    [SerializeField] TextMeshProUGUI m_chat;
    float m_timer = 3;
    [SerializeField]
    bool startTimer, canMove;
    PhotonView m_pv;

    public bool CanMove { get => canMove; set => canMove = value; }
    public bool StartTimer { get => startTimer; set => startTimer = value; }

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
        if (StartTimer)
        {
            if (m_timer > 0)
            {
                m_pv.RPC("Timer", RpcTarget.All);
            }
            else if (m_timer < 0)
            {
                m_pv.RPC("SetUp", RpcTarget.AllBuffered);
            }
        }
    }

    public void leaveCurrentRoomFromEditor()
    {
        CanMove = false;
        LevelNetworkManager.Instance.disconnectFromCurrentRoom();
    }

    public void addText(string message)
    {
        m_pv.RPC("UpdateChat", RpcTarget.All,  message);
    }

    public void addPrivateText(string message)
    {
        m_chat.text = m_chat.text + message + "\n";
        Invoke("removeText", 2f);
    }

    [PunRPC]
    void SetUp()
    {
        if (m_pv.IsMine)
        {
            PhotonNetwork.Instantiate("TowerSpawner", transform.position, Quaternion.identity);
        }
        CanMove = true;
        StartTimer = false;
        m_Timer.text = null;
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
}
