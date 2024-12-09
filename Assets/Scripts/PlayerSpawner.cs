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
    [SerializeField]
    private Transform _spawnPos3;
    [SerializeField]
    private Transform _spawnPos4;


    private void Start()
    {
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber; // ActorNumber comienza desde 1

        switch (playerIndex)
        {
            case 0:

                break;
            case 1:
                PhotonNetwork.Instantiate(_playerPrefab.name, _spawnPos.position, Quaternion.identity);
                break;
            case 2:
                PhotonNetwork.Instantiate(_playerPrefab.name, _spawnPos2.position, Quaternion.identity);
                break;
            case 3:
                PhotonNetwork.Instantiate(_playerPrefab.name, _spawnPos.position, Quaternion.identity);
                break;
            case 4:
                PhotonNetwork.Instantiate(_playerPrefab.name, _spawnPos2.position, Quaternion.identity);
                break;
        }

    }
}
