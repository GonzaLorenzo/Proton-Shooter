using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    public List<Player> playersConected;
    public TMP_Text txtCount;
    public GameObject door;
    public bool startGame;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        txtCount.text = playersConected.Count.ToString();
        if (playersConected.Count < 2)
        {
            txtCount.fontSize = 20;
            txtCount.text = "Se debe conectar al menos dos jugadores. Para comenzar el juego.";
            //playersConected.ForEach(x => x.canMove = false);
            //Debug.Log("Se debe conectar al menos dos jugadores");
        }
        else
        {
            txtCount.text = "Comienzo del juego";
            txtCount.fontSize = 20;
            startGame = true;
            StartCoroutine(StartGame());
        }
    }
    IEnumerator StartGame()
    {
        photonView.RPC("Start_Game", RpcTarget.AllBuffered);
        yield return null;

    }
    [PunRPC]
    void Start_Game()
    {
        door.GetComponent<Animator>().SetBool("isOpen", true);
    }
}
