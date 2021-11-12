using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetUp : MonoBehaviourPunCallbacks {
    //camara del prefab, activa solo si es la instancia del cliente local
    public Camera playerCamera;
    public TextMeshProUGUI playerNameText;

    void Start() {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc")) {

            if (photonView.IsMine) {
                //si este player pertenece a este cliente
                GetComponent<CarMovement>().enabled = true;
                GetComponent<LapController>().enabled = true;
                playerCamera.enabled = true;

            } else {
                //disable carmovement y camara de los oponentes
                GetComponent<CarMovement>().enabled = false;
                GetComponent<LapController>().enabled = false;
                playerCamera.enabled = false;
            }
        } else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr")) {
            if (photonView.IsMine) {
                //si este player pertenece a este cliente
                GetComponent<CarMovement>().enabled = true;
                GetComponent<CarMovement>().controlsEnabled = true;
                playerCamera.enabled = true;

            } else {
                //disable carmovement y camara de los oponentes
                GetComponent<CarMovement>().enabled = false;
                playerCamera.enabled = false;
            }
        }
        SetPlayerUI();
    }

    private void SetPlayerUI() {
        if (playerNameText == null) return;
        playerNameText.text = photonView.Owner.NickName;
        if (photonView.IsMine) {
            playerNameText.gameObject.SetActive(false);
        }
    }
}
