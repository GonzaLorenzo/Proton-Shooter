using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
public class CharacterFA : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    private GameObject _uiWin;
    [SerializeField]
    private GameObject _uiLose;
    Player _owner;
    string _myNickName;
    Rigidbody _rb;
    Animator _animator;
    [SerializeField]
    private float maxVelocityChange;
    [SerializeField]
    private ProjectileFA _myProjectile;
    [SerializeField]
    private Transform _projectileSpawn;
    [SerializeField]
    private Transform _myShoulder;
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
    public LayerMask doorMask;
    [SerializeField]
    Material _playerMaterial;
    [SerializeField]
    private Transform _camera;
    private bool _isAlive = true;
    [SerializeField]
    private CinemachineVirtualCamera _mainCamera;
    public event Action<float> onLifeBarUpdate = delegate { };
    public event Action onDestroy = delegate { };

    void Awake()
    {
        //_gm = GameObject.Find("GameManager").GetComponent<GameManagerFA>();
        //_mainCamera = GameObject.Find("MainCamera").GetComponent<CinemachineBrain>();
        _animator = GetComponent<Animator>(); 
        //_camera = _mainCamera.transform.position;
        LifeBarManager _lifeBarManager = FindObjectOfType<LifeBarManager>();
        //_myMat = GetComponent<Renderer>().material;
        //_myMaterial.color = Color.red; TEST
    }

    void Start()
    {
        CanvasLifeBar lifeBarManager = FindObjectOfType<CanvasLifeBar>();
        //if(PhotonNetwork.CurrentRoom.PlayerCount <= 2)
        if (PhotonNetwork.PlayerList.Length <= 2)
        {
            _mainCamera.Priority = 1000;
        }
        lifeBarManager?.SpawnLifeBar(this);
    }

    public void RotateMouse(float h, float v)
    {
        _myShoulder.transform.Rotate(v, 0, 0);
        transform.Rotate(0, h, 0);
    }

    public void Move(Vector3 dir, bool isIdle)
    {
        if(_isAlive)
        {
            dir = transform.TransformDirection(dir) * _speed;
            Vector3 velocity = _rb.velocity;
            Vector3 deltaVelocity = (dir - velocity);
            deltaVelocity.x = Mathf.Clamp(deltaVelocity.x, -maxVelocityChange, maxVelocityChange);
            deltaVelocity.z = Mathf.Clamp(deltaVelocity.z, -maxVelocityChange, maxVelocityChange);
            deltaVelocity.y = 0f;

            _rb.AddForce(deltaVelocity, ForceMode.VelocityChange);
            //_rb.MovePosition(_rb.position + dir * _speed * Time.fixedDeltaTime);

            _animator.SetFloat("FloatX", dir.x);
            _animator.SetFloat("FloatY", dir.y);

            if (dir.x != 0 || dir.y != 0)
            {
                //_isIdle = false;
                _animator.SetBool("IsIdle", isIdle);
            }
            else
            {
                //_isIdle = true;
                _animator.SetBool("IsIdle", isIdle);
            }
        }
    }

    public void Shoot()
    {
        if(_isAlive)
        {
            PhotonNetwork.Instantiate(_myProjectile.name, _projectileSpawn.position, transform.rotation)
             .GetComponent<ProjectileFA>()
             .SetOwner(this)
             .SetDmg(_dmg)
             .SetMaterial(_myMat, _owner);
        }

    }   

    public void Jump()
    {
        if(_isAlive)
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
    }        

    public void TakeDamage(float dmg)
    {
        _currentLife -= dmg;
        onLifeBarUpdate(_currentLife);
        if(_currentLife <= 0 && _isAlive)
        {
            //MyServer.instance.RequestWinner(_owner);
            _isAlive = false;
            MyServer.instance.PlayerDisconnect(_owner);
            photonView.RPC("RPC_DisconnectOwner", _owner);
        }
        else
        {
            //photonView.RPC("RPC_LifeChange", _owner, _currentLife);
        }
    }

    public void Interact()
    {
        if(_isAlive)
        {
            //RaycastHit hit;
            //if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit, 100, doorMask))
            //{
                //hit.transform.GetComponent<InteractableDoorFA>().Interact();
            //}
            //else
            //{
                //Debug.Log("else");
            //}

            Collider[] allTargets = Physics.OverlapSphere(transform.position, 3.5f, doorMask);

            foreach (var items in allTargets)
            {
                if(items != null)
                {
                    items.GetComponent<InteractableDoorFA>().Interact();
                }
            }
        }
    }

    public void Win()
    {
        Instantiate(_uiWin);
    }

    public void Lose()
    {
        Instantiate(_uiLose);
    }

    public CharacterFA SetInitialParameters(Player player)
    {
        _owner = player;
        _myNickName = player.NickName;
        Debug.Log("bronca " + _owner);
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
