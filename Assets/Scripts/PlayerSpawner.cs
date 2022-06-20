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

    private void Start()
    {
        PhotonNetwork.Instantiate(_playerPrefab.name, _spawnPos.position, Quaternion.identity);
    }
}
