using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadePicker : MonoBehaviour
{
    [SerializeField]
    private int _granadeCount;
    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if(player)
        {
            player.AddGranades(_granadeCount);
            Destroy(gameObject);
        } 
    }

}
