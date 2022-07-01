using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AttatchCamera : MonoBehaviour
{
    private CinemachineVirtualCamera _myCam;
    private Transform _owner;
    private void Awake()
    {
        _myCam = GetComponent<CinemachineVirtualCamera>();
        //_myCam.Follow = GameObject.Find("Player(Clone)").transform;
    }

    public AttatchCamera SetOwner(Transform owner)
    {
        _owner = owner;
        _myCam.Follow = _owner;
        return this;
    }
}
