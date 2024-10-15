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
    private bool _canShoot = true;
    private Vector2 _input;
    private Animator _animator;
    [SerializeField]
    private GameObject _uiLose;
    [SerializeField]
    private GameObject _uiWin;
    [SerializeField]
    private float _life;
    private Camera _mainCamera;
    private GameManager gameManager;
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
    private Vector3 _mySpawnPos;
    private Vector3 aimDir;
    private bool _isIdle = true;
    private bool _canWalk = true;
    private bool _isDead = false;
    private bool _isThrowingGranades = false;
    private bool _canAim = true;
    private int _totalLifes = 3;
    private bool _isAlive = true;
    private Renderer[] _myChildrenRenderers;
    public event Action<float> onLifeBarUpdate = delegate { };
    public event Action<int> onGranadeUiUpdate = delegate { };
    public event Action onDestroy = delegate { };

    [SerializeField]
    private int _Granades;

    [SerializeField]

    private int _maxGranades;

    void Start()
    {
        _maxGranades = 3;
        _Granades = 3;
        LifeBarManager _lifeBarManager = FindObjectOfType<LifeBarManager>();
        _lifeBarManager?.SpawnLifeBar(this);
        _myChildrenRenderers = GetComponentsInChildren<Renderer>();


        if (!photonView.IsMine)
        {
            //APLICARLE EL MATERIAL ENEMIGO EN CADA CHILD.
            //GetComponentInChildren<Renderer>().material = _enemyMaterial;
            foreach (Renderer r in _myChildrenRenderers)
            {
                r.material = _enemyMaterial;
            }

        }

        _canShoot = false;
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _life = _maxLife;
        _mySpawnPos = transform.position;
        //GetComponentInChildren<Renderer>().material = _myMaterial;

        if (photonView.IsMine)
            _mainCamera = Camera.main;
        else
            _myThirdPersonCamera.SetActive(false);

        gameManager = FindObjectOfType<GameManager>();
        gameManager.playersConected.Add(this);

        if (!photonView.IsMine) return;
        this.gameObject.name = "PlayerOwner";

    }


    void Update()
    {
        if (!photonView.IsMine) return;

        _mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            //debugTransform.position = raycastHit.point;
            _mouseWorldPosition = raycastHit.point;
            aimDir = (_mouseWorldPosition - _projectileSpawner.position).normalized;
        }

        SetInputAnims();
        SetIdle();

        float h = _horizontalSpeed * Input.GetAxis("Mouse X");
        //h = h + _cameraTurnOffset;
        float v = _verticalSpeed * -Input.GetAxis("Mouse Y");
        transform.Rotate(0, h, 0);
        _myShoulder.transform.Rotate(v, 0, 0);

        if (Input.GetKeyDown(KeyCode.Mouse0) && _canShoot)
        {
            //photonView.RPC("RPC_Shoot", RpcTarget.All);
            PhotonNetwork.Instantiate(_myProjectile.name, _projectileSpawner.position, Quaternion.LookRotation(aimDir, Vector3.up));

        }

        if (Input.GetKeyDown(KeyCode.G) && _Granades > 0)
        {
            //photonView.RPC("RPC_Shoot", RpcTarget.All);
            Debug.Log("Tirar Granadas");
            StartCoroutine(StartThrowingGranades());

        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && _canAim)
        {
            SwitchCameras();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            TakeDamage(20);
        }
    }

    IEnumerator StartThrowingGranades()
    {
        _animator.SetBool("IsThrowingGranades", true);
        yield return new WaitForSeconds(1);
        _animator.SetBool("IsThrowingGranades", false);
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        {
            if (_canWalk)
                Walk();
        }
    }

    private void ResetValues()
    {
        _life = _maxLife;
        onLifeBarUpdate(_life);
        _canWalk = true;
        _isDead = false;
        _animator.SetBool("IsDead", _isDead);
        _canAim = true;
        _isAlive = true;
        _canShoot = true;
        _animator.SetBool("IsAlive", _isAlive);
    }

    private void SetIdle()
    {
        if (_input.x != 0 || _input.y != 0)
        {
            _isIdle = false;
            _animator.SetBool("IsIdle", _isIdle);
        }
        else
        {
            _isIdle = true;
            _animator.SetBool("IsIdle", _isIdle);
        }
    }

    private void SetInputAnims()
    {
        _input.x = Input.GetAxis("Horizontal");
        _input.y = Input.GetAxis("Vertical");

        _animator.SetFloat("FloatX", _input.x);
        _animator.SetFloat("FloatY", _input.y);
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

    public void Respawn()
    {
        if (_totalLifes != 0)
        {
            ResetValues();
            //_animator.Play("Idle_Shoot_Ar 0");  
            transform.position = _mySpawnPos;
            _totalLifes--;
        }
        else
        {
            LoseScreen();
        }
    }

    private void LoseScreen()
    {
        if (!photonView.IsMine)
        {
            Instantiate(_uiWin);
            return;
        }
        Instantiate(_uiLose);
    }

    private void SwitchCameras()
    {
        if (_myThirdPersonCamera.activeInHierarchy)
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
        if (photonView.IsMine)
        {
            _life -= dmg;

            onLifeBarUpdate(_life);

            if (_life <= 0)
            {
                photonView.RPC("RPC_Die", RpcTarget.All);
            }
        }
    }

    public void OwnerDestroy(GameObject projectile)
    {
        Destroy(projectile);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_life);
        }
        else
        {
            var lifeBarFillAmount = (float)stream.ReceiveNext();

            onLifeBarUpdate(lifeBarFillAmount);
        }
    }

    public void AddGranades(int granades)
    {
        if (photonView.IsMine)
        {
            if (_Granades < _maxGranades)
                _Granades += _maxGranades;

            onLifeBarUpdate(_life);

            if (_life <= 0)
            {
                photonView.RPC("RPC_Die", RpcTarget.All);
            }
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
        .SetDmg(_dmg)
        .SetForward(aimDir);
    }

    [PunRPC]
    void RPC_Die()
    {
        //Cambiar color de children a material de soldado negro.

        //Si hay respawn, lo hace solo el original y se le reinician los valores con un void ResetValues().
        _isDead = true;
        _canWalk = false;
        _canShoot = false;
        _isAlive = false;
        _animator.SetBool("IsAlive", _isAlive);
        _canAim = false;
        _animator.SetBool("IsDead", _isDead);
    }
    #endregion
}
