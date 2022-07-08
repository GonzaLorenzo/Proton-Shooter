using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 

public class InteractableDoorFA : MonoBehaviourPun
{
    private Animator _animator;
    private BoxCollider _boxCollider;

    private void Awake()
    {
        _animator = GetComponent<Animator>();    
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void Interact()
    {
        photonView.RPC("RPC_OpenDoor", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_OpenDoor()
    {
        _animator.SetBool("isOpen", true);
        _boxCollider.enabled = false;
    }


}
