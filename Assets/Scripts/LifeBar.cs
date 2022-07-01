using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    [SerializeField]
    private Slider _lifeBar;

    [SerializeField]
    private NonPlayer _target;

    [SerializeField]
    private float _verticalOffset = 0; //500

    public LifeBar SetTarget(NonPlayer target)
    {
        _target = target;
        _target.onLifeBarUpdate += UpdateBar;
        _target.onDestroy += () => Destroy(gameObject);
        return this;
    }
    public LifeBar SetParent(Transform parent)
    {
        transform.SetParent(parent);
        return this;
    }

    public void UpdateBar(float amount)
    {
        _lifeBar.value = amount;
    }

    public void UpdatePosition()
    {
        transform.position = _target.transform.position + Vector3.up * _verticalOffset;
    }
}
