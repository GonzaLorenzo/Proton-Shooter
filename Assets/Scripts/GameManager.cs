using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPun
{
    public List<Player> playersConected;
    public TMP_Text txtCount;
    public GameObject door;
    public GameObject doorTwo;
    public bool startGame;

    [SerializeField] private Text nameText;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (photonView.IsMine)
            nameText.text = PhotonNetwork.NickName;
        else
            nameText.text = photonView.Owner.NickName;

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
        yield return 10;

    }
    [PunRPC]
    void Start_Game()
    {
        door.GetComponent<Animator>().SetBool("isOpen", true);
        doorTwo.GetComponent<Animator>().SetBool("isOpen", true);
    }


}
