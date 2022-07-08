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
    public GameManagerFA gameManager;

    internal void setSpawner(PlayerSpawner spawner)
    {
        this.spawner = spawner;
    }

    internal void setGameManager(GameManagerFA gameManager)
    {
        this.gameManager = gameManager;
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
        gameManager.AddPlayerToCount();
    }
     
    public void PlayerDisconnect(Player player)
    {
        PhotonNetwork.Destroy(_dictModels[player].gameObject);
        _dictModels.Remove(player);
    }

    #region Requests

    public void RequestMove(Player player, Vector3 dir, bool isIdle)
    {
        photonView.RPC("RPC_Move", _server, player, dir, isIdle);
    }
    public void RequestShoot(Player player)
    {
        photonView.RPC("RPC_Shoot", _server, player);
    }

    public void RequestWinner(Player player)
    {
        photonView.RPC("RPC_ShowWinner", RpcTarget.All, player);
    }

    public void RequestMouse(Player player, float h, float v)
    {
        photonView.RPC("RPC_Mouse", _server, player, h, v);
    }

    public void RequestJump(Player player)
    {
        photonView.RPC("RPC_Jump", _server, player);
    }

    public void RequestInteract(Player player)
    {
        photonView.RPC("RPC_Interact", _server, player);
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
    void RPC_Move(Player playerRequest, Vector3 dir, bool isIdle)
    {
        if (_dictModels.ContainsKey(playerRequest))
        {
            _dictModels[playerRequest].Move(dir, isIdle);
        }
    }

    [PunRPC]
    void RPC_Disconnect(Player player)
    {
        PhotonNetwork.Destroy(_dictModels[player].gameObject);
        _dictModels.Remove(player);
    }

    [PunRPC]
    void RPC_Mouse(Player playerRequest, float h, float v)
    {
        if (_dictModels.ContainsKey(playerRequest))
        {
            _dictModels[playerRequest].RotateMouse(h, v);
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
            _dictModels[playerRequest].Interact();
        }
    }

    [PunRPC]
    void RPC_ShowWinner(Player playerRequest)
    {
        //foreach (var player in _dictModels)
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if(player.NickName != playerRequest.NickName)
                {
                    if (_dictModels.ContainsKey(playerRequest))
                    {
                        _dictModels[playerRequest].Win();
                    }
                }
                else
                {
                    //player.Value.Lose();
                }
            }
    }


    #endregion
}



