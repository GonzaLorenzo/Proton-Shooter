using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _dmg;
    [SerializeField]
    private float _speed;
    private Player _owner;

    void Update()
    {
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    public Projectile SetDmg(float dmg)
    {
        _dmg = dmg;
        return this;
    }

    public Projectile SetMaterialColor(Color newColor)
    {
        GetComponent<Renderer>().material.color = newColor;
        return this;
    }

    public Projectile SetOwner(Player owner)
    {
        _owner = owner;
        return this;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if(player && player != _owner)
        {
            player.TakeDamage(_dmg);
            Destroy(gameObject);
        }
    }



}
