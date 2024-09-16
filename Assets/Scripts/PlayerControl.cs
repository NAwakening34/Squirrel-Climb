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
    [SerializeField]
    bool m_suelo = true, m_death;
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
        if (Input.GetKeyDown(KeyCode.Space) && m_suelo && m_pv.IsMine && UIManager.Instance.CanMove && !m_death)
        {
            m_rb2D.AddForce(Vector3.up * m_force, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        if (m_pv.IsMine)
        {
            m_myanim.SetInteger("Vel", (int)m_hor);
            m_myanim.SetBool("Floor", m_suelo);
            m_myanim.SetBool("Death", m_death);
            if (UIManager.Instance.CanMove && !m_death)
            {
                m_hor = Input.GetAxisRaw("Horizontal");
                m_rb2D.velocity = new Vector2(m_hor * m_speed, m_rb2D.velocity.y);
            }
            if (m_hor < 0)
            {
                m_spriteRenderer.flipX= true;
            }
            else if (m_hor > 0)
            {
                m_spriteRenderer.flipX= false;
            }
        }
        if (transform.position.y < -10)
        {
            m_rb2D.gravityScale = 0;
            m_rb2D.velocity = Vector3.zero;
            transform.position = transform.position;
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
            m_boxCollider.enabled = false;
            m_rb2D.velocity = Vector3.zero;
            m_rb2D.AddForce(Vector3.up * (m_force * 1.5f), ForceMode2D.Impulse);
            UIManager.Instance.addText(m_pv.Owner.NickName + "died");
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
