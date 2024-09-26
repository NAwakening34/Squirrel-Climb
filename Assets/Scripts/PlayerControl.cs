using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerControl: MonoBehaviour
{
    [SerializeField] TextMeshPro m_NicknameUI;
    [SerializeField] int m_speed, m_force;
    float m_hor;
    [SerializeField] GameObject m_punch;
    [SerializeField]
    bool m_suelo = true, m_death, m_punching;
    Rigidbody2D m_rb2D;
    BoxCollider2D m_boxCollider;
    SpriteRenderer m_spriteRenderer;
    Animator m_myanim;
    PhotonView m_pv;

    void Start()
    {
        m_rb2D = GetComponent<Rigidbody2D>();
        m_boxCollider = GetComponent<BoxCollider2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_myanim= GetComponent<Animator>();
        m_pv = GetComponent<PhotonView>();
        m_NicknameUI.text = m_pv.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_pv.IsMine && UIManager.Instance.State == GameStates.Playing && !m_death && !m_punching)
        {
            if (Input.GetKeyDown(KeyCode.Space) && m_suelo)
            {
                m_rb2D.AddForce(Vector3.up * m_force, ForceMode2D.Impulse);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("golpeo");
                m_pv.RPC("Punch", RpcTarget.All);
            }
        }
    }

    private void FixedUpdate()
    {
        if (m_pv.IsMine)
        {
            m_myanim.SetInteger("Vel", (int)m_hor);
            m_myanim.SetBool("Floor", m_suelo);
            m_myanim.SetBool("Death", m_death);
            if (!m_death)
            {
                if (UIManager.Instance.State == GameStates.Playing && !m_punching)
                {
                    m_hor = Input.GetAxisRaw("Horizontal");
                    m_rb2D.velocity = new Vector2(m_hor * m_speed, m_rb2D.velocity.y);
                    if (m_hor < 0)
                    {
                        m_spriteRenderer.flipX = true;
                        m_punch.transform.localScale = new Vector3(-1, 1, 1);
                    }
                    else if (m_hor > 0)
                    {
                        m_spriteRenderer.flipX = false;
                        m_punch.transform.localScale = new Vector3(1, 1, 1);
                    }
                }
                if (UIManager.Instance.State == GameStates.Ending)
                {
                    Debug.Log("gano");
                    UIManager.Instance.Winner(m_pv.Owner.NickName);
                }
            }
        }
        if (transform.position.y < -10)
        {
            m_rb2D.gravityScale = 0;
            m_rb2D.velocity = Vector3.zero;
            transform.position = transform.position;
        }
    }

    [PunRPC]
    private void Punch()
    {
        m_punch.SetActive(true);
        m_punching = true;
        Invoke("CancelPunch", 0.15f);
    }

    [PunRPC]
    private void StopPunch()
    {
        m_punch.SetActive(false);
        m_punching = false;
        m_suelo = true;
    }

    private void CancelPunch()
    {
        Debug.Log("ending punch");
        m_pv.RPC("StopPunch", RpcTarget.All);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor"))
        {
            m_suelo = true;
        }
        else if (collision.CompareTag("DeathZone"))
        {
            m_death= true;
            m_boxCollider.enabled = false;
            m_rb2D.velocity = Vector3.zero;
            m_rb2D.AddForce(Vector3.up * (m_force * 1.5f), ForceMode2D.Impulse);
            UIManager.Instance.addText(m_pv.Owner.NickName + "died");
            UIManager.Instance.PlayerDied();
        }
        else if (collision.CompareTag("Hit"))
        {
            UIManager.Instance.addText(m_pv.Owner.NickName + "got hit");
            float dir = collision.transform.position.x - transform.position.x;
            if(dir < 0)
            {
                m_rb2D.AddForce(Vector3.left * 4, ForceMode2D.Impulse);
            }
            else
            {
                m_rb2D.AddForce(Vector3.right * 4, ForceMode2D.Impulse);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor"))
        {
            m_suelo = false;
        }
    }
}
