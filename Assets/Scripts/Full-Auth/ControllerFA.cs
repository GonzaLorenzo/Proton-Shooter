using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ControllerFA : MonoBehaviourPun
{
    Player _localPlayer;
    float _h;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        _localPlayer = PhotonNetwork.LocalPlayer;
    }

    void Update()
    {
        _h = Input.GetAxis("Horizontal");

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
        if(_h != 0)
        {
            var dir = Vector3.right * _h;

            MyServer.instance.RequestMove(_localPlayer, dir);
        }

    }

    private void OnApplicationQuit()
    {
        MyServer.instance.RequestDisconnection(_localPlayer);
        PhotonNetwork.Disconnect();
    }
}