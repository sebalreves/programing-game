using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSelection : MonoBehaviour {

    public GameObject[] SelectablePlayers;
    public int playerSelectionNumber;


    // Start is called before the first frame update
    void Start() {
        playerSelectionNumber = 0;
        ActivatePlayer(playerSelectionNumber);


    }

    private void ActivatePlayer(int _value) {
        foreach (GameObject SelectablePlayer in SelectablePlayers) {
            SelectablePlayer.SetActive(false);
        }
        SelectablePlayers[_value].SetActive(true);

        //set custom propertyExitg
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable()
            { { MultiplayerRacingGame.PLAYER_SELECTION_NUMBER, playerSelectionNumber } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
    }

    public void NextPlayer() {
        playerSelectionNumber = (playerSelectionNumber + 1) % SelectablePlayers.Length;
        ActivatePlayer(playerSelectionNumber);
    }

    public void PreviousPlayer() {
        playerSelectionNumber -= 1;
        playerSelectionNumber = playerSelectionNumber < 0
        ? SelectablePlayers.Length - 1 : playerSelectionNumber;

        ActivatePlayer(playerSelectionNumber);

    }
}
