using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;

public class Player : MonoBehaviourPun, IPunObservable
{
    private float _horizontalSpeed = 2.0f;
    private float _verticalSpeed = 2.0f;
    private bool _canShoot;
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
    private float maxVelocityChange;
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
    [SerializeField]
    private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField]
    private Transform debugTransform;
    private Vector3 _mouseWorldPosition;

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
        _canShoot = false;
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>(); 
        _life = _maxLife;
        GetComponentInChildren<Renderer>().material = _myMaterial;
        _mainCamera = Camera.main;
    }


    void Update()
    {
        if (!photonView.IsMine) return;

        _mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            _mouseWorldPosition = raycastHit.point;
        }

        //Third Person Movement

        _input.x = Input.GetAxis("Horizontal");
        _input.y = Input.GetAxis("Vertical");
       
        _animator.SetFloat("FloatX", _input.x);
        _animator.SetFloat("FloatY", _input.y);

        float h = _horizontalSpeed * Input.GetAxis("Mouse X");
        //h = h + _cameraTurnOffset;
        float v = _verticalSpeed * -Input.GetAxis("Mouse Y");
        transform.Rotate(0, h, 0);
        _myShoulder.transform.Rotate(v, 0, 0);

        if (Input.GetKeyDown(KeyCode.Mouse0) && _canShoot)
        {
            photonView.RPC("RPC_Shoot", RpcTarget.All); 
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            SwitchCameras();
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
            Walk();
        }
    }

    private void Walk()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        input = transform.TransformDirection(input) * _speed;

        Vector3 velocity = _rb.velocity;
        Vector3 deltaVelocity = (input - velocity);
        deltaVelocity.x = Mathf.Clamp(deltaVelocity.x, -maxVelocityChange, maxVelocityChange);
        deltaVelocity.z = Mathf.Clamp(deltaVelocity.z, -maxVelocityChange, maxVelocityChange);
        deltaVelocity.y = 0f;

        _rb.AddForce(deltaVelocity, ForceMode.VelocityChange);

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
            _canShoot = true;
        }
        else
        {
            _myThirdPersonCamera.SetActive(true);
            _myAimingCamera.SetActive(false);
            _canShoot = false;
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
        Vector3 aimDir = (_mouseWorldPosition - _projectileSpawner.position).normalized;
        Instantiate(_myProjectile, _projectileSpawner.position, Quaternion.LookRotation(aimDir, Vector3.up))
        .SetOwner(this)
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
