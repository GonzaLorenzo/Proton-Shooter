using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsOut : MonoBehaviour
{
    [SerializeField]
    private Light _mySpotlight;
    void OnTriggerEnter(Collider other)
    {
        _mySpotlight.enabled = false;
        Destroy(this);   
    }
}
