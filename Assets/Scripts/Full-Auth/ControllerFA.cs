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
    [SerializeField]
    Player _localPlayer;
    [SerializeField]
    Transform _myShoulder;
    float _h;
    Vector3 input;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        _localPlayer = PhotonNetwork.LocalPlayer;
    }

    void Update()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");   

        input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        _h = Input.GetAxis("Horizontal");
        float h = _horizontalSpeed * Input.GetAxis("Mouse X");
        float v = _verticalSpeed * -Input.GetAxis("Mouse Y");
        
        if(h != 0)
        {
            MyServer.instance.RequestMouseX(_localPlayer, h);
        }

        if(v != 0)
        {
            MyServer.instance.RequestMouseY(_localPlayer, v);
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            MyServer.instance.RequestShoot(_localPlayer);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            MyServer.instance.RequestJump(_localPlayer);
        }
    }

    void FixedUpdate()
    {
        /*  if(_h != 0)
        {
            var dir = Vector3.right * _h;

            MyServer.instance.RequestMove(_localPlayer, dir);
        }  */

        if(input != Vector3.zero)
        {
            MyServer.instance.RequestMove(_localPlayer, input);
        }

    }

    private void OnApplicationQuit()
    {
        MyServer.instance.RequestDisconnection(_localPlayer);
        PhotonNetwork.Disconnect();
    }
}
