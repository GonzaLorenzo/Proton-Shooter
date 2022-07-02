using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class MyServer : MonoBehaviourPun
{
    public static MyServer instance;
    Player _server;
    [SerializeField]
    CharacterFA _characterPrefab;
    [SerializeField]
    private GameObject _playerPrefab;
    Dictionary<Player, CharacterFA> _dictModels = new Dictionary<Player, CharacterFA>();
    public int PackagePerSecond { get; private set; }
    public PlayerSpawner spawner;

    internal void setSpawmer(PlayerSpawner spawner)
    {
        this.spawner = spawner;
    }

    void Start()
    {

        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            if (photonView.IsMine)
            {
                Debug.Log("1");
                photonView.RPC("RPC_SetServer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer, 1);
            }
        }
    }

    void SetServer(Player serverPlayer, int sceneIndex = 1)
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        _server = serverPlayer;
        PackagePerSecond = 60;
        PhotonNetwork.LoadLevel(sceneIndex);
        var playerLocal = PhotonNetwork.LocalPlayer;

        if (serverPlayer != playerLocal)
        {
            Debug.Log("INSTANCIATE PLAyER");
            photonView.RPC("RPC_AddPlayer", _server, playerLocal);
        }
    }

    IEnumerator WaitForLevel(Player player)
    {
        while (PhotonNetwork.LevelLoadingProgress > 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }

        if (spawner == null)
            Debug.Log("PlayerSpawner is nullll");

        Debug.Log("GetSpawnPosition " + spawner.GetSpawnPosition());
        //Hacer SpawnManager para instanciar a los jugadores y guardar posiciones.
        //CharacterFA newCharacter = PhotonNetwork.Instantiate(_characterPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<CharacterFA>().SetInitialParameters(player);
        //CharacterFA newCharacter = PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<CharacterFA>().SetInitialParameters(player);
        CharacterFA newCharacter = PhotonNetwork.Instantiate(_playerPrefab.name, spawner.GetSpawnPosition().position, Quaternion.identity).transform.GetChild(0).GetComponent<CharacterFA>().SetInitialParameters(player);

        _dictModels.Add(player, newCharacter);
    }

    public void PlayerDisconnect(Player player)
    {
        PhotonNetwork.Destroy(_dictModels[player].gameObject);
        _dictModels.Remove(player);
    }

    #region Requests

    public void RequestMove(Player player, Vector3 dir)
    {
        photonView.RPC("RPC_Move", _server, player, dir);
    }

    public void RequestShoot(Player player)
    {
        photonView.RPC("RPC_Shoot", _server, player);
    }

    public void RequestMouseX(Player player, float dir)
    {
        photonView.RPC("RPC_MouseX", _server, player, dir);
    }

    public void RequestMouseY(Player player, float dir)
    {
        photonView.RPC("RPC_MouseY", _server, player, dir);
    }

    public void RequestJump(Player player)
    {
        photonView.RPC("RPC_Jump", _server, player);
    }

    public void RequestDisconnection(Player player)
    {
        photonView.RPC("RPC_Disconnect", _server, player);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    #endregion

    #region RPCs

    [PunRPC]
    void RPC_AddPlayer(Player player)
    {
        StartCoroutine(WaitForLevel(player));
    }

    [PunRPC]
    void RPC_SetServer(Player serverPlayer, int sceneIndex = 1)
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        _server = serverPlayer;
        PackagePerSecond = 60;
        PhotonNetwork.LoadLevel(sceneIndex);
        var playerLocal = PhotonNetwork.LocalPlayer;

        if (serverPlayer != playerLocal)
        {
            photonView.RPC("RPC_AddPlayer", _server, playerLocal);
        }
    }

    [PunRPC]
    void RPC_Move(Player playerRequest, Vector3 dir)
    {
        if (_dictModels.ContainsKey(playerRequest))
        {
            _dictModels[playerRequest].Move(dir);
        }
    }

    [PunRPC]
    void RPC_Disconnect(Player player)
    {
        PhotonNetwork.Destroy(_dictModels[player].gameObject);
        _dictModels.Remove(player);
    }

    [PunRPC]
    void RPC_MouseX(Player playerRequest, float dir)
    {
        if (_dictModels.ContainsKey(playerRequest))
        {
            _dictModels[playerRequest].RotateMouseX(dir);
        }
    }

    [PunRPC]
    void RPC_MouseY(Player playerRequest, float dir)
    {
        if (_dictModels.ContainsKey(playerRequest))
        {
            _dictModels[playerRequest].RotateMouseY(dir);
        }
    }

    [PunRPC]
    void RPC_Shoot(Player playerRequest)
    {
        if (_dictModels.ContainsKey(playerRequest))
        {
            _dictModels[playerRequest].Shoot();
        }
    }

    [PunRPC]
    void RPC_Jump(Player playerRequest)
    {
        if (_dictModels.ContainsKey(playerRequest))
        {
            _dictModels[playerRequest].Jump();
        }
    }

    [PunRPC]
    void RPC_Interact(Player playerRequest)
    {
        if (_dictModels.ContainsKey(playerRequest))
        {
            //_dictModels[playerRequest].Interact();
        }
    }

    #endregion
}



