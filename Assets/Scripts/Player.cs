using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;

public class Player : MonoBehaviourPun, IPunObservable
{
    private Vector2 _input;
    private Animator _animator;
    [SerializeField]
    private float _life;
    private Camera _mainCamera;
    private Rigidbody _rb;
    [SerializeField]
    private float turnSpeed = 15f;
    [SerializeField]
    private float _cameraTurnOffset;
    [SerializeField]
    private float _maxLife;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _jumpForce;
    [SerializeField]
    private float _dmg;
    [SerializeField]
    private Projectile _myProjectile;
    [SerializeField]
    private Transform _projectileSpawner;
    [SerializeField]
    private Material _myMaterial;
    [SerializeField]
    private Material _enemyMaterial;
    [SerializeField]
    private Transform _myShoulder;
    [SerializeField]
    private GameObject _myThirdPersonCamera;
    [SerializeField]
    private GameObject _myAimingCamera;

    public event Action<float> onLifeBarUpdate = delegate { };
    public event Action onDestroy = delegate { };

    void Start()
    {
        LifeBarManager _lifeBarManager = FindObjectOfType<LifeBarManager>();
        _lifeBarManager?.SpawnLifeBar(this);

        if (!photonView.IsMine)
        {
            GetComponentInChildren<Renderer>().material = _enemyMaterial;
        }

        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>(); 
        _life = _maxLife;
        GetComponentInChildren<Renderer>().material = _myMaterial;
        _mainCamera = Camera.main;
    }


    void Update()
    {
        if (!photonView.IsMine) return;

        //Third Person Movement

        //_input.x = Input.GetAxis("Horizontal");
        //_input.y = Input.GetAxis("Vertical");
       
        //_animator.SetFloat("FloatX", _input.x);
        //_animator.SetFloat("FloatY", _input.y);

        float YCamera = _mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, YCamera + _cameraTurnOffset, 0), turnSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            photonView.RPC("RPC_Shoot", RpcTarget.All); 
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            //SwitchCameras();
            TakeDamage(20);
        }
    }

    private void FixedUpdate()
    {
        if(!photonView.IsMine) return;
        {
            //Third Person Movement.
        }
    }

    private void Jump()
    {
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
    }

    private void SwitchCameras()
    {
        if(_myThirdPersonCamera.activeInHierarchy)
        {
            _myThirdPersonCamera.SetActive(false);
            _myAimingCamera.SetActive(true);
        }
        else
        {
            _myThirdPersonCamera.SetActive(true);
            _myAimingCamera.SetActive(false);
        }
    }

    public void TakeDamage(float dmg)
    {
        if(photonView.IsMine)
        {
            _life -= dmg;

            onLifeBarUpdate(_life);

            if(_life <= 0)
            {
                photonView.RPC("RPC_Die", RpcTarget.All);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(_life);
        }
        else
        {
            var lifeBarFillAmount = (float)stream.ReceiveNext();

            onLifeBarUpdate(lifeBarFillAmount);
        }
    }

    private void OnDestroy()
    {
        //OnDestroy();
        onDestroy();
    }
    #region RPCs
    [PunRPC]
    void RPC_Shoot()
    {
        Instantiate(_myProjectile, _projectileSpawner.position, Quaternion.identity)
        .SetOwner(this)
        .SetMaterial(_myMaterial)
        .SetDmg(_dmg);
    }

    [PunRPC]
    void RPC_Die()
    {
        //Cambiar color de children a material de soldado negro.

        //Si hay respawn, lo hace solo el original y se le reinician los valores con un void ResetValues().
    }
    #endregion
}
