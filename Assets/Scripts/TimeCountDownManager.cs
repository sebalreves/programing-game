using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class TimeCountDownManager : MonoBehaviourPunCallbacks {

    private Text TimeUIText;
    private float timeToStart = 5f;

    void Awake() {
        TimeUIText = RacingGameManager.instance.TimeUIText;
    }
    void Start() {

    }

    void Update() {
        //los jugadores se sincronizan con el master solamente
        if (PhotonNetwork.IsMasterClient) {
            if (timeToStart >= 0f) {
                timeToStart -= Time.deltaTime;
                photonView.RPC("setTime", RpcTarget.AllBuffered, timeToStart);
            } else if (timeToStart < 0) {
                photonView.RPC("startRace", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    public void setTime(float _time) {
        if (_time > 0f)
            TimeUIText.text = _time.ToString("F1");
        else
            TimeUIText.text = "";
    }

    [PunRPC]
    public void startRace() {
        GetComponent<CarMovement>().controlsEnabled = true;
        this.enabled = false;
    }
}
