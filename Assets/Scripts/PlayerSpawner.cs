using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviourPun
{
    [SerializeField]
    private Transform _spawnPos;
    [SerializeField]
    private Transform _spawnPos2;
    [SerializeField]
    private Transform _spawnPos3;
    [SerializeField]
    private Transform _spawnPos4;


    private void Awake()
    {
        MyServer.instance.setSpawner(this);
    }
    public Transform GetSpawnPosition()
    {
        switch (PhotonNetwork.CurrentRoom.PlayerCount)
        {
            case 0:
            case 1:
                return _spawnPos;
            case 2:
                return _spawnPos2;
            case 3:
                return _spawnPos3;
            case 4:
                return _spawnPos4;
            default:
                return _spawnPos;
        }

    }


}
