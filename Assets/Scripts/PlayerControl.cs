using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerControl: MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_NicknameUI;
    [SerializeField] int m_speed, m_force;
    float m_hor;
    bool m_suelo = true, m_death;
    Rigidbody2D m_rb2D;
    BoxCollider2D m_boxCollider;
    Animator m_myanim;
    PhotonView m_pv;

    void Start()
    {
        m_rb2D = GetComponent<Rigidbody2D>();
        m_myanim= GetComponent<Animator>();
        m_pv = GetComponent<PhotonView>();
        m_NicknameUI.text = m_pv.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        m_myanim.SetInteger("Vel", (int)m_hor);
        m_myanim.SetBool("Floor", m_suelo);
        if (m_pv.IsMine && UIManager.Instance.CanMove && !m_death)
        {
            m_hor = Input.GetAxisRaw("Horizontal");
            m_rb2D.MovePosition(m_rb2D.position + new Vector2(m_hor, m_rb2D.position.y) * m_speed * Time.fixedDeltaTime);
            if (Input.GetKeyDown(KeyCode.Space) && m_suelo)
            {
                m_rb2D.AddForce(transform.up * m_force * Time.fixedDeltaTime);
                //m_myanim.Play("Jump");
            }
        }
        if (transform.position.y > -10)
        {
            m_rb2D.gravityScale = 0;
            m_rb2D.velocity = Vector3.zero;
        }
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
            m_boxCollider.gameObject.SetActive(false);
            m_rb2D.AddForce(transform.up * (m_force * 3) * Time.fixedDeltaTime);
            m_myanim.Play("Death");
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
