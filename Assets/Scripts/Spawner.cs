using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Transform[] spawnpos;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate("Player", spawnpos[PhotonNetwork.CurrentRoom.PlayerCount - 1].position, Quaternion.identity);
    }
}
