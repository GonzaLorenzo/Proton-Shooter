using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _dmg = 20;
    [SerializeField]
    private float _speed;
    private NonPlayer _owner;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _rb.velocity = transform.forward * _speed;
    }

    public Projectile SetDmg(float dmg)
    {
        _dmg = dmg;
        return this;
    }

    public Projectile SetOwner(NonPlayer owner)
    {
        _owner = owner;
        return this;
    }

    public Projectile SetForward(Vector3 aimDir)
    {
        transform.rotation = Quaternion.LookRotation(aimDir, Vector3.up);
        return this;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<NonPlayer>();
        if(player && player != _owner)
        {
            player.TakeDamage(_dmg);      
            Destroy(gameObject);
            //PhotonNetwork.Destroy(gameObject);
            //_owner.OwnerDestroy(this.gameObject);
        }
        else
        {
            
            Destroy(gameObject);
            //PhotonNetwork.Destroy(gameObject);
            //_owner.OwnerDestroy(this.gameObject);
        }
    }

}
