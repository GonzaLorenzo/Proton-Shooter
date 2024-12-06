using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviourPunCallbacks
{
    //public UnityEngine.UI.InputField serverNameField;
    public TMPro.TMP_InputField serverNameField;
    public TMPro.TMP_InputField userNameField;

    public GameObject panelLobby;
    public GameObject parentUI;
    public GameObject prebaUI_playerInfo;
    public GameObject ConnectedScreen;
    public GameObject WarningMessage;
    public Button btnConnect;
    public void BTN_CreateRoom()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        PhotonNetwork.NickName = userNameField.text;
        PhotonNetwork.CreateRoom(serverNameField.text, options);
        btnConnect.interactable = false;

    }

    public void BTN_JoinRoom()
    {
        PhotonNetwork.NickName = userNameField.text;
        PhotonNetwork.JoinRoom(serverNameField.text);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created.");
    }

    public override void OnJoinedRoom()
    {
        panelLobby.SetActive(true);
        LobbyManager lobby = panelLobby.GetComponent<LobbyManager>();
        ConnectedScreen.SetActive(false);


        foreach (var player in PhotonNetwork.PlayerList)
        {
            //Debug.Log(player.NickName);
            var playerInfo = Instantiate(prebaUI_playerInfo, parentUI.transform);
            playerInfo.GetComponent<PlayerInformation>().SetTextName(player.NickName);
            playerInfo.GetComponent<PlayerInformation>().SetColorPanel(player == PhotonNetwork.LocalPlayer);
        }

        CheckPlayerCount();

    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"El jugador {newPlayer.NickName} se unió a la sala.");

        var playerInfo = Instantiate(prebaUI_playerInfo, parentUI.transform);
        playerInfo.GetComponent<PlayerInformation>().SetTextName(newPlayer.NickName);
        playerInfo.GetComponent<PlayerInformation>().SetColorPanel(newPlayer == PhotonNetwork.LocalPlayer);

        CheckPlayerCount();
    }

    private void CheckPlayerCount()
    {
        if (PhotonNetwork.PlayerList.Count() >= 2)
        {
            WarningMessage.SetActive(false);

            if (PhotonNetwork.IsMasterClient)
                btnConnect.interactable = true;
            else { 
                btnConnect.interactable = false;
                Text buttonText = btnConnect.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = "Wait for the host";
                }
            }
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartGameForAll", RpcTarget.All);
        }
    }

    [PunRPC]
    void StartGameForAll()
    {
        // Código para iniciar el juego (por ejemplo, cargar la escena)
        PhotonNetwork.LoadLevel("Test_Map");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Failed to create room {returnCode}, message {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Failed to join room {returnCode}, message {message}");
    }
}
