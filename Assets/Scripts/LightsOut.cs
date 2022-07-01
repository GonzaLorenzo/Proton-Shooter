using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LightsOut : MonoBehaviourPun
{
    [SerializeField]
    private Light _mySpotlight;
    void OnTriggerEnter(Collider other)
    {
        photonView.RPC("RPC_DestroyThisLight", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_DestroyThisLight()
    {
        _mySpotlight.enabled = false;
        Destroy(this);  
    }
}
