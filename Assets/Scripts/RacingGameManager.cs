using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class RacingGameManager : MonoBehaviour {
    public GameObject[] playerPrefabs;
    public Transform[] instantiatePositions;
    public GameObject[] FinishOrderTexts;
    public Text TimeUIText;
    public List<GameObject> lapTriggers = new List<GameObject>();

    //singleton
    public static RacingGameManager instance = null;
    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
        //no destruir al recargar la escena
        DontDestroyOnLoad(gameObject);
    }


    void Start() {
        if (PhotonNetwork.IsConnectedAndReady) {
            object playerSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerRacingGame.PLAYER_SELECTION_NUMBER, out playerSelectionNumber)) {

                //actor number 1,2,3...
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = instantiatePositions[actorNumber - 1].position;

                //Photon necesita solo el nombre del prefab y que esté en Resources
                PhotonNetwork.Instantiate(playerPrefabs[(int)playerSelectionNumber].name,
                                        instantiatePosition,
                                        Quaternion.identity);
            }
        }
        foreach (GameObject gm in FinishOrderTexts) {
            gm.SetActive(false);

        }
    }

    // Update is called once per frame
    void Update() {

    }
}
