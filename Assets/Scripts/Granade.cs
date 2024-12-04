using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Explosion Prefab")]
    [SerializeField]
    private GameObject _explosionEffectPrefab;

    [SerializeField]
    private Vector3 _expliosionParticleEffecOffSet = new Vector3(0, 1, 0);
    
    [SerializeField]
    private Vector3 _throwPosition;
    
    [SerializeField]
    private Vector3 throwDirection = new Vector3(0, 1, 0);

    private float _explosionDelay = 3f;
    private float _explosionForce = 700f;
    [SerializeField]
    private float _explosionRadius = 5f;

    private float countDown;
    private bool hasExploded = false;

    [SerializeField]
    private float throwForce = 10f;

    [SerializeField]
    private float maxForce = 20f;

    [SerializeField]
    private float _dmg = 50;
    private Player _owner;
    private Rigidbody _rb;

    [SerializeField]
    private Vector3 _aimDir;

    void Start()
    {
        countDown = _explosionDelay;
        _rb = GetComponent<Rigidbody>();
        ThrowGrenade();

    }

    public Granade SetOwner(Player owner)
    {
        _owner = owner;
        return this;
    }
    // Update is called once per frame
    void Update()
    {
        if (!hasExploded)
        {
            countDown -= Time.deltaTime;
            if (countDown < 0)
            {
                Explode();
                hasExploded = true;
            }
        }
    }

    void Explode()
    {
        GameObject explosionEffect = Instantiate(_explosionEffectPrefab, transform.position + _expliosionParticleEffecOffSet, Quaternion.identity);
        CheckExplode();
        Destroy(explosionEffect, 1f);
        Destroy(gameObject);
    }

    void ThrowGrenade()
    {
        Vector3 spawnPosition = _throwPosition + _aimDir;

        Vector3 finalTrowDirection = (transform.forward + throwDirection).normalized;

        _rb.AddForce(finalTrowDirection * throwForce, ForceMode.VelocityChange);
    }

    public Granade SetForward(Vector3 aimDir)
    {
        transform.rotation = Quaternion.LookRotation(aimDir, Vector3.up);
        _aimDir = aimDir;
        return this;
    }

    

    public Granade SetMyThrowPosition(Vector3 throwPosition)
    {
        Debug.Log("mydebug throwPosition" + throwPosition);
        _throwPosition = throwPosition;
        return this;
    }

    void CheckExplode()
    {
        //for (int i = 0; i < 360; i += 10) // Incrementa para mayor densidad de líneas
        //{
        //    float angleRad = Mathf.Deg2Rad * i;
        //    Vector3 direction = new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
        //    Debug.DrawLine(transform.position, transform.position + direction * _explosionRadius, Color.red, 2f);
        //}
       
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach (Collider hit in hitColliders)
        {
          
            Player damageable = hit.GetComponent<Player>();
            if (damageable != null)
            {
                // Aplica el daño
                damageable.TakeDamage(_dmg);
            }
        }

       
        Destroy(gameObject);
    }

}
