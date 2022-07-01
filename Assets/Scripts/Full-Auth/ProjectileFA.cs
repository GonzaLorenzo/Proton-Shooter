using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ProjectileFA : MonoBehaviourPun
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _dmg;

    CharacterFA _owner;

    void Update()
    {
        if(!photonView.IsMine)
            return;
        transform.position += transform.forward * _speed * Time.deltaTime;
        
    }

    public ProjectileFA SetDmg(float dmg)
    {
        _dmg = dmg;
        return this;
    }

    public ProjectileFA SetOwner(CharacterFA owner)
    {
        _owner = owner;
        return this;
    }

    public ProjectileFA SetMaterial(Material newMat, Player clientOwner)
    {
        GetComponent<Renderer>().material = newMat;

        photonView.RPC("SetRPCMaterial", RpcTarget.Others, clientOwner);
        return this;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;
        var character = other.GetComponent<CharacterFA>();

        if(character && character != _owner)
        {
            character.TakeDamage(_dmg);

            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    void SetRPCMaterial(Player clientOwner)
    {
        if(PhotonNetwork.LocalPlayer != clientOwner)
        {
            //GetComponent<Renderer>().material = newMat; Enemy Material
        }
        else
        {
            //GetComponent<Renderer>().material = newMat; Player Material
        }
    }
}
