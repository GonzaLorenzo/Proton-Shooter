using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerPrefab;
    [SerializeField]
    private Transform _spawnPos;
    [SerializeField]
    private Transform _spawnPos2;
    private int _spawnNumber = 1;

    private void Start()
    {
        switch(_spawnNumber)
        {
        case 1:
            PhotonNetwork.Instantiate(_playerPrefab.name, _spawnPos.position, Quaternion.identity);
            _spawnNumber ++;
                break;
        case 2:
            PhotonNetwork.Instantiate(_playerPrefab.name, _spawnPos2.position, Quaternion.identity);
            break;
        }
        
    }
}
