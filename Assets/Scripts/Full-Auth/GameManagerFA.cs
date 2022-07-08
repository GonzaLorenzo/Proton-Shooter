using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Realtime;

public class GameManagerFA : MonoBehaviourPun
{
    public int playersConnected = 0;
    public TMP_Text txtCount;
    public GameObject door;
    public bool startGame;

    private void Awake()
    {
        MyServer.instance.setGameManager(this);
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //txtCount.fontSize = 20;
        //txtCount.text = "Se debe conectar al menos dos jugadores. Para comenzar el juego.";
        photonView.RPC("RPC_ChangeStartingText", RpcTarget.AllBuffered);
        //playersConected.ForEach(x => x.canMove = false);
        //Debug.Log("Se debe conectar al menos dos jugadores");
    }
    void Update()
    {
        //txtCount.text = playersConnected.Count.ToString();
        //if (playersConnected.Count < 2)
        //{
            //txtCount.fontSize = 20;
            //txtCount.text = "Se debe conectar al menos dos jugadores. Para comenzar el juego.";

            //playersConected.ForEach(x => x.canMove = false);
            //Debug.Log("Se debe conectar al menos dos jugadores");
        //}
        //else
        //{
            //txtCount.text = "Comienzo del juego";
            //txtCount.fontSize = 20;
            //startGame = true;
            //StartCoroutine(StartGame());
        //}
    }

    public void AddPlayerToCount()
    {
        playersConnected++;
        txtCount.text = playersConnected.ToString();

        if (playersConnected == 2)
        {
            txtCount.text = "Comienzo del juego";
            txtCount.fontSize = 20;
            startGame = true;
            //StartCoroutine(StartGame());
            photonView.RPC("RPC_StartGame", RpcTarget.AllBuffered);
            photonView.RPC("RPC_ChangeGameText", RpcTarget.AllBuffered);
        }
    }

    //IEnumerator StartGame()
    //{
        //photonView.RPC("RPC_StartGame", RpcTarget.AllBuffered);
        //yield return null;
    //}

    [PunRPC]
    void RPC_StartGame()
    {
        door.GetComponent<Animator>().SetBool("isOpen", true);
    }

    [PunRPC]
    void RPC_ChangeGameText()
    {
        txtCount.text = "Comienzo del juego";
        txtCount.fontSize = 20;
    }

    [PunRPC]
    void RPC_ChangeStartingText()
    {
        txtCount.fontSize = 20;
        txtCount.text = "Se debe conectar al menos dos jugadores. Para comenzar el juego.";
    }
}
