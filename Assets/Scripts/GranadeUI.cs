using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GranadeUI : MonoBehaviour
{
    // Start is called before the first frame update
    public Text txtGranadeCount;
    
    public void UpdateCount(int amount)
    {
        txtGranadeCount.text = amount.ToString();
    }
}
