using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ControllerFA : MonoBehaviourPun
{
    private float _horizontalSpeed = 2.0f;
    private float _verticalSpeed = 2.0f;
    private float _speed = 5;
    private bool _isIdle;
    [SerializeField]
    Player _localPlayer;
    [SerializeField]
    Transform _myShoulder;
    private Animator _animator;
    float _h;
    Vector3 input;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        _animator = GetComponent<Animator>();
        _localPlayer = PhotonNetwork.LocalPlayer;
    }

    void Update()
    {
        SetIdle();
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        _h = Input.GetAxis("Horizontal");
        float h = _horizontalSpeed * Input.GetAxis("Mouse X");
        float v = _verticalSpeed * -Input.GetAxis("Mouse Y");

        if (h != 0 || v != 0)
        {
            MyServer.instance.RequestMouse(_localPlayer, h, v);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            MyServer.instance.RequestShoot(_localPlayer);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            MyServer.instance.RequestJump(_localPlayer);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            MyServer.instance.RequestInteract(_localPlayer);
        }

        //SetIdle();
    }

    void FixedUpdate()
    {
        /*  if(_h != 0)
        {
            var dir = Vector3.right * _h;

            MyServer.instance.RequestMove(_localPlayer, dir);
        }  */

        if (input != Vector3.zero)
        {
            MyServer.instance.RequestMove(_localPlayer, input, _isIdle);
        }

    }

    private void OnApplicationQuit()
    {
        //MyServer.instance.RequestWinner(_localPlayer);
        MyServer.instance.RequestDisconnection(_localPlayer);
        PhotonNetwork.Disconnect();
    }

    private void SetIdle()
    {
        if (input.x != 0 || input.y != 0)
        {
            _isIdle = false;
            //_animator.SetBool("IsIdle", _isIdle);
        }
        else
        {
            _isIdle = true;
            //_animator.SetBool("IsIdle", _isIdle);
        }
    }

    private void SetInputAnims()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        _animator.SetFloat("FloatX", input.x);
        _animator.SetFloat("FloatY", input.y);

    }

}
