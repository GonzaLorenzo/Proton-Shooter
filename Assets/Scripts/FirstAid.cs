using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAid : MonoBehaviour
{
    [SerializeField]
    private int _healValue;
    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<NonPlayer>();
        if(player)
        {
            player.TakeDamage(-_healValue);
            Destroy(gameObject);
        } 
    }

    private void Heal()
    {
        
    }
}
