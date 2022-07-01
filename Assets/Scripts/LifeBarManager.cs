using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LifeBarManager : MonoBehaviour
{
    [SerializeField]
    private LifeBar _lifeBarPrefab;

    public event Action updateBars = delegate { };

    public void SpawnLifeBar(NonPlayer target)
    {
        LifeBar lifeBar = Instantiate(_lifeBarPrefab, target.transform.position, Quaternion.identity)
        .SetParent(transform)
        .SetTarget(target);

        updateBars += lifeBar.UpdatePosition;

        target.onDestroy += () => updateBars -= lifeBar.UpdatePosition; 
    }

    private void LateUpdate()
    {
        updateBars();
    }

}
