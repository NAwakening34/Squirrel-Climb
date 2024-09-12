using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] TextMeshProUGUI m_TextMeshProUGUI;
    [SerializeField] TextMeshProUGUI m_Score;
    [SerializeField] TextMeshProUGUI m_chat;
    [SerializeField] GameObject m_button;
    int m_currentScore;
    float m_timeleft = 180, m_timer = 3;
    bool start, canMove;
    PhotonView m_pv;

    public bool CanMove { get => canMove; set => canMove = value; }

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
        if (start)
        {
            if (m_timer > 0)
            {
                m_pv.RPC("FirstTimer", RpcTarget.AllBuffered);
            }
            else
            {
                m_pv.RPC("Timer", RpcTarget.All);
            }
        }
    }

    public void leaveCurrentRoomFromEditor()
    {
        CanMove = false;
        m_button.SetActive(true);
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
    void UpdateChat(string message)
    {
        m_chat.text = m_chat.text + message + "\n";
        Invoke("removeText", 2f);
    }

    private void removeText()
    {
        m_chat.text = null;
    }

    public void StartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            start = true;
        }
        else
        {
            addText("Se necesita un minimo de 2 jugadores para empezar");
        }
    }


    [PunRPC]
    void FirstTimer()
    {
        m_button.SetActive(false);
        m_timer -= Time.deltaTime;
        int time = (int)m_timer;
        m_TextMeshProUGUI.text = "Game Starts in " + time.ToString();
    }

    [PunRPC]
    void Timer()
    {
        CanMove = true;
        if (m_timeleft < 0)
        {
            leaveCurrentRoomFromEditor();
        }
        m_timeleft -= Time.deltaTime;
        int time = (int)m_timeleft;
        m_TextMeshProUGUI.text = "Time Left: " + time.ToString();
    }

    public void updateText(int p_newScore)
    {
        m_currentScore += p_newScore;
        m_Score.text = "Score: " + m_currentScore.ToString();
    }

    public void addPoints()
    {
        m_pv.RPC("AddPointsInUI", RpcTarget.AllBuffered, 5);
    }

    [PunRPC]
    void AddPointsInUI(int p_newPoints)
    {
        updateText(p_newPoints);
    }
}
