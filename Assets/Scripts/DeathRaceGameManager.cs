using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeathRaceGameManager : MonoBehaviour {
    public GameObject[] playerPrefabs;

    void Start() {
        if (PhotonNetwork.IsConnectedAndReady) {
            object playerSelectionNumber;
            Debug.Log("AAAAAAAAA");
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerRacingGame.PLAYER_SELECTION_NUMBER, out playerSelectionNumber)) {
                Debug.Log("BBBBBBBBBBB");
                int randomPosition = Random.Range(-15, 15);
                Debug.Log(playerPrefabs[(int)playerSelectionNumber].name);
                PhotonNetwork.Instantiate(playerPrefabs[(int)playerSelectionNumber].name,
                                            new Vector3(randomPosition, 0, randomPosition),
                                            Quaternion.identity);
            }
        }
    }

    void Update() {

    }
}
