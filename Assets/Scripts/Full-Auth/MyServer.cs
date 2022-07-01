using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MyServer : MonoBehaviourPun
{
    public static MyServer instance;
    Player _server;
    [SerializeField]
    CharacterFA _characterPrefab;
    Dictionary<Player,CharacterFA> _dictModels = new Dictionary<Player, CharacterFA>();
    public int PackagePerSecond { get; private set; }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if(instance == null)
        {
            if(photonView.IsMine)
            {
                photonView.RPC("RPC_SetServer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer, 1);
            }
        }
    }

    void SetServer(Player serverPlayer, int sceneIndex = 1)
    {
        if(instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        _server = serverPlayer;
        PackagePerSecond = 60;
        PhotonNetwork.LoadLevel(sceneIndex);
        var playerLocal = PhotonNetwork.LocalPlayer;

        if(serverPlayer != playerLocal)
        {
            photonView.RPC("RPC_AddPlayer", _server, playerLocal);
        }
    }

    IEnumerator WaitForLevel(Player player)
    {
        while (PhotonNetwork.LevelLoadingProgress > 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }

        //Hacer SpawnManager para instanciar a los jugadores y guardar posiciones.
        CharacterFA newCharacter = PhotonNetwork.Instantiate(_characterPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<CharacterFA>().SetInitialParameters(player);

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
        photonView.RPC("RPC_Move", _server, dir);
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
        if(instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        _server = serverPlayer;
        PackagePerSecond = 60;
        PhotonNetwork.LoadLevel(sceneIndex);
        var playerLocal = PhotonNetwork.LocalPlayer;

        if(serverPlayer != playerLocal)
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

    #endregion
}



