using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class ShowNicknames : MonoBehaviour
{
    [SerializeField]
    Text _playerListText;
    Player[] _allPlayers;
    private bool _skipFirst;

    void Update()
    {
        UpdatePlayer();
    }

    public void UpdatePlayer()
    {
        _playerListText.text = "Players: \n";

        _allPlayers = PhotonNetwork.PlayerList;
        _skipFirst = false;

        foreach(var pl in _allPlayers)
        {
            if(_skipFirst) _playerListText.text += pl.NickName + "\n";
            else _skipFirst = true;
        }
    }

}
