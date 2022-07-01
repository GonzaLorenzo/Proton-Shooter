using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CanvasLifeBar : MonoBehaviour
{
    [SerializeField]
    private LifeBarFA _lifeBarPrefab;

    public event Action updateBars = delegate { };

    public void SpawnLifeBar(CharacterFA target)
    {
        LifeBarFA lifeBar = Instantiate(_lifeBarPrefab, target.transform.position, Quaternion.identity)
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
