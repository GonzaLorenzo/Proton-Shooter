using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class CharacterFA : MonoBehaviourPun
{
    Player _owner;
    Rigidbody _rb;

    [SerializeField]
    float _maxLife;
    float _currentLife;
    [SerializeField]
    float _speed;
    [SerializeField]
    float _dmg;
    Material _myMaterial;
    [SerializeField]
    Material _playerMaterial;
    void Awake()
    {
        _myMaterial = GetComponent<Renderer>().material;
        //_myMaterial.color = Color.red; TEST
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(Vector3 dir)
    {
        _rb.MovePosition(_rb.position + dir * _speed * Time.fixedDeltaTime);
    }

    public void TakeDamage(float dmg)
    {
        _currentLife -= dmg;
        if(_currentLife <= 0)
        {
            MyServer.instance.PlayerDisconnect(_owner); //Por ahora.
            photonView.RPC("RPC_DisconnectOwner", _owner);
            //Animacion muerte, respawn y conteo de vidas.
        }
        else
        {

        }
    }

    public CharacterFA SetInitialParameters(Player player)
    {
        _owner = player;
        _rb = GetComponent<Rigidbody>();
        _currentLife = _maxLife;
        //_myMaterial.color = Color.yellow; TEST

        photonView.RPC("RPC_SetLocalParameters", _owner, _currentLife);
        return this;
    }

    #region RPCs

    [PunRPC]
    void RPC_SetLocalParameters(float life)
    {
        _currentLife = _maxLife = life;

        _myMaterial = _playerMaterial;
    }

    [PunRPC]
    void RPC_DisconnectOwner()
    {
        PhotonNetwork.Disconnect();
    }

    #endregion
}
