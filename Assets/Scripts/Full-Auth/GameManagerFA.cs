using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Realtime;

public class GameManagerFA : MonoBehaviourPun
{
    private string _startingText = "Se deben conectar al menos dos jugadores para comenzar el juego.";
    private string _gameText = "Comienzo del juego";
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
        photonView.RPC("RPC_ChangeText", RpcTarget.AllBuffered, _startingText);
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
            startGame = true;
            //StartCoroutine(StartGame());
            photonView.RPC("RPC_StartGame", RpcTarget.AllBuffered);
            photonView.RPC("RPC_ChangeText", RpcTarget.AllBuffered, _gameText);
        }
    }

    //IEnumerator StartGame()
    //{
        //photonView.RPC("RPC_StartGame", RpcTarget.AllBuffered);
        //yield return null;
    //}

    public void AnnounceWinnerFA(string nickName)
    {
        string finalText = "El ganador es: " + nickName;
        photonView.RPC("RPC_ChangeText", RpcTarget.AllBuffered, finalText);
    }

    [PunRPC]
    void RPC_StartGame()
    {
        door.GetComponent<Animator>().SetBool("isOpen", true);
    }

    [PunRPC]
    void RPC_ChangeText(string textToUse)
    {
        txtCount.text = textToUse;
        txtCount.fontSize = 20;
    }
}
