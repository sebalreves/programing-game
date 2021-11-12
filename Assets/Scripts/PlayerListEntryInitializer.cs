using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerListEntryInitializer : MonoBehaviour {
    [Header("UI references")]
    public Text PlayerNameText;
    public Button PlayerReadyButton;
    public Image PlayerReadyImage;

    private bool isPlayerReady = false;

    public void initializer(int playerID, string playerName) {
        PlayerNameText.text = playerName;

        if (PhotonNetwork.LocalPlayer.ActorNumber != playerID) {
            PlayerReadyButton.gameObject.SetActive(false);
        } else {
            // Local player
            ExitGames.Client.Photon.Hashtable LocalProps = new ExitGames.Client.Photon.Hashtable() { { MultiplayerRacingGame.PLAYER_READY, isPlayerReady } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(LocalProps);
            PlayerReadyButton.onClick.AddListener(
                () => {
                    isPlayerReady = !isPlayerReady;
                    ExitGames.Client.Photon.Hashtable LocalProps = new ExitGames.Client.Photon.Hashtable() { { MultiplayerRacingGame.PLAYER_READY, isPlayerReady } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(LocalProps);

                }
            );
        }
    }

    //
    public void setPlayerReady(bool _value) {
        PlayerReadyImage.enabled = _value;
        if (_value == true)
            PlayerReadyButton.GetComponentInChildren<Text>().text = "Ready!";
        else
            PlayerReadyButton.GetComponentInChildren<Text>().text = "Ready???";
    }
}
