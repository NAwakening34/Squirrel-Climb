using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    Rigidbody2D m_rb2D;
    [SerializeField]
    float m_fallingspeed;
    // Start is called before the first frame update
    void Start()
    {
        m_rb2D= GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (UIManager.Instance.State == GameStates.Playing)
        {
            //m_rb2D.velocity = Vector3.down * m_fallingspeed;
        }
        else
        {
            m_rb2D.velocity = Vector3.zero;
        }

        if (transform.position.y < -20)
        {
            Destroy(gameObject);
        }
    }
}
