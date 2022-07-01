using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class CharacterFA : MonoBehaviourPun, IPunObservable
{
    Player _owner;
    Rigidbody _rb;

    [SerializeField]
    private ProjectileFA _myProjectile;
    [SerializeField]
    private Transform _projectileSpawn;

    [SerializeField]
    float _maxLife;
    float _currentLife;
    [SerializeField]
    float _speed;
    [SerializeField]
    float _dmg;
    [SerializeField]
    float _jumpForce;
    private Renderer[] _myChildrenRenderers;
    private Material _myMat;
    [SerializeField]
    Material _playerMaterial;

    public event Action<float> onLifeBarUpdate = delegate { };
    public event Action onDestroy = delegate { };

    void Awake()
    {
        LifeBarManager _lifeBarManager = FindObjectOfType<LifeBarManager>();
        //_myMat = GetComponent<Renderer>().material;
        //_myMaterial.color = Color.red; TEST
    }

    void Start()
    {
        CanvasLifeBar lifeBarManager = FindObjectOfType<CanvasLifeBar>();
        lifeBarManager?.SpawnLifeBar(this);
    }

    public void Move(Vector3 dir)
    {
        _rb.MovePosition(_rb.position + dir * _speed * Time.fixedDeltaTime);
    }

    public void Shoot()
    {
        PhotonNetwork.Instantiate(_myProjectile.name, _projectileSpawn.position, transform.rotation)
                     .GetComponent<ProjectileFA>()
                     .SetOwner(this)
                     .SetDmg(_dmg)
                     .SetMaterial(_myMat, _owner);
    }   

    public void Jump()
    {
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
    }        

    public void TakeDamage(float dmg)
    {
        _currentLife -= dmg;
        onLifeBarUpdate(_currentLife);
        if(_currentLife <= 0)
        {
            MyServer.instance.PlayerDisconnect(_owner); //Por ahora lo rajamos.
            photonView.RPC("RPC_DisconnectOwner", _owner);
            //Animacion muerte, respawn y conteo de vidas.
        }
        else
        {
            //photonView.RPC("RPC_LifeChange", _owner, _currentLife);
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

    void OnDestroy()
    {
        onDestroy();
    }

    #region RPCs

    [PunRPC]
    void RPC_LifeChange(float currentLife)
    {
        _currentLife = currentLife;
    }

    [PunRPC]
    void RPC_SetLocalParameters(float life)
    {
        //_currentLife = _maxLife = life;


        //_myChildrenMaterial = _playerMaterial;
    }

    [PunRPC]
    void RPC_DisconnectOwner()
    {
        PhotonNetwork.Disconnect();
    }

    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(_currentLife);
            stream.SendNext(_maxLife);
        }
        else
        {
            _currentLife = (float)stream.ReceiveNext();
            _maxLife = (float)stream.ReceiveNext();

            onLifeBarUpdate(_currentLife);
        }
    }
}
