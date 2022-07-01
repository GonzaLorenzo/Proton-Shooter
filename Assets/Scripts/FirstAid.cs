using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FirstAid : MonoBehaviourPun
{
    [SerializeField]
    Player _playerToHeal;
    [SerializeField]
    private int _healValue;
    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if(player)
        {
            _playerToHeal = other.GetComponent<Player>(); 
            photonView.RPC("RPC_DestroyThisGO", RpcTarget.AllBuffered);
            //Destroy(gameObject);
        } 
    }

    [PunRPC]
    void RPC_DestroyThisGO()
    {
        _playerToHeal.TakeDamage(-_healValue);
        Destroy(gameObject);
    }
}
