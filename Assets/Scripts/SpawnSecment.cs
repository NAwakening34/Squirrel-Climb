using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSecment : MonoBehaviour
{
    public static SpawnSecment Instance;

    [SerializeField]
    GameObject[] m_segments;
    [SerializeField]
    Transform m_spawnpos;
    PhotonView m_pv;
    int m_segmentID;

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
    // Start is called before the first frame update
    void Start()
    {
        m_pv= GetComponent<PhotonView>();
        InvokeRepeating("SpawnSegment", 33, 66);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //[PunRPC]
    //void returnValue()
    //{
    //    m_segmentID = Random.Range(0, m_segments.Length);
    //}
    void SpawnSegment()
    {
        //m_pv.RPC("returnValue", RpcTarget.AllBuffered);
        //m_segmentID = Random.Range(0, m_segments.Length);
        Instantiate(m_segments[m_segmentID], m_spawnpos);
        m_segmentID++;
        if (m_segmentID == m_segments.Length)
        {
            m_segmentID = 0;
        }
    }
}
