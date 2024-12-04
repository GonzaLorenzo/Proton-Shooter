using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{

    public Text txtName;
    public Image image;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetTextName(string name)
    {
        txtName.text = name;
    }

    public void SetColorPanel(bool isLocal)
    {
        image.color = isLocal ? Color.green : Color.red;
    }

}
