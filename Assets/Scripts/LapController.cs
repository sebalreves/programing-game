using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class LapController : MonoBehaviourPun {
    private List<GameObject> lapTriggers = new List<GameObject>();
    private int finishOrder = 0;
    public enum raiseEventCodes {
        WhoFinished = 1
    }

    void Start() {
        foreach (GameObject lapTrigger in RacingGameManager.instance.lapTriggers) {
            lapTriggers.Add(lapTrigger);
        }
    }

    private void OnEnable() {
        PhotonNetwork.NetworkingClient.EventReceived += onEvent;
    }

    private void OnDisable() {
        PhotonNetwork.NetworkingClient.EventReceived -= onEvent;
    }

    void onEvent(EventData _eventData) {
        if (_eventData.Code == (byte)raiseEventCodes.WhoFinished) {
            object[] data = (object[])_eventData.CustomData;
            string nickName = (string)data[0];
            int position = (int)data[1];
            int viewID = (int)data[2];

            finishOrder = position;
            Debug.Log(nickName + " " + position);

            GameObject orderUIText = RacingGameManager.instance.FinishOrderTexts[position - 1];
            orderUIText.SetActive(true);
            orderUIText.GetComponent<Text>().text = position + " - " + nickName;

            if (viewID == photonView.ViewID) {
                //si es que soy yo terminando la carrera
                orderUIText.GetComponent<Text>().text = position + " - " + nickName + "(YOU)";
                orderUIText.GetComponent<Text>().color = Color.red;
            } else
                orderUIText.GetComponent<Text>().text = position + " - " + nickName;
        }
    }

    public void OnTriggerEnter(Collider other) {
        if (lapTriggers.Contains(other.gameObject)) {
            int triggerIndex = lapTriggers.IndexOf(other.gameObject);
            lapTriggers[triggerIndex].SetActive(false);

            if (other.name == "FinishTrigger") {
                //Game finished or new lap
                GameFinished();
            }
        }
    }


    public void GameFinished() {
        //liberar la camara del jugador
        GetComponent<PlayerSetUp>().playerCamera.transform.parent = null;
        GetComponent<CarMovement>().enabled = false;
        finishOrder += 1;

        //raise eventdata
        string nickName = photonView.Owner.NickName;
        int viewID = photonView.ViewID;
        object[] raiseData = new object[] { nickName, finishOrder, viewID };

        RaiseEventOptions raiseOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };
        SendOptions sendOptions = new SendOptions //exitgame.client.photon
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte)raiseEventCodes.WhoFinished, raiseData, raiseOptions, sendOptions);
    }
    void Update() {

    }
}
