using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPun
{
    // Start is called before the first frame update

    public List<Player> playersConected;
    public bool startGame;

    public Button button_StartGame;

    void Start()
    {
        
    }

    void Update()
    {
        
        if (playersConected.Count < 2)
        {
            
        }
        else
        {
            
        }
    }
}
