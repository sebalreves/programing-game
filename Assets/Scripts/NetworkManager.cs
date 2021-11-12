using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

//Clase con callbacks de PUN
public class NetworkManager : MonoBehaviourPunCallbacks {

    [Header("Login UI")]
    public GameObject LoginUIPanel;
    public InputField PlayerNameInput;


    [Header("Connecting Info Panel")]
    public GameObject ConnectingInfoUIPanel;

    [Header("Creating Room Info Panel")]
    public GameObject CreatingRoomInfoUIPanel;

    [Header("GameOptions  Panel")]
    public GameObject GameOptionsUIPanel;

    [Header("Create Room Panel")]
    public GameObject CreateRoomUIPanel;
    public InputField RoomNameInputField;
    public string GameMode;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomUIPanel;
    public Text RoomInfoText;
    public GameObject PlayerListPrefab;
    public GameObject PlayerListContent;
    public GameObject StartGameButton;
    public Text GameModeText;
    public Image PanelBackground;
    public Sprite RacingBackground;
    public Sprite DeathRaceBackground;
    public GameObject[] playerSelectionUIGameObjects;
    public DeathRacePlayer[] deathRacePlayers;
    public RacingPlayer[] racingPlayers;


    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomUIPanel;

    public Dictionary<int, GameObject> playerListGameobjects;
    #region UNITY_METHODS
    void Start() {
        ActivatePanel(LoginUIPanel.name);
        PhotonNetwork.AutomaticallySyncScene = true;

    }

    #endregion

    #region UI_CALLBACKS
    //Intenta conectarse usando el nombre entregado por el jugador
    public void onLoginButtonClick() {
        string playerName = PlayerNameInput.text;

        if (!string.IsNullOrEmpty(playerName)) {
            ActivatePanel(ConnectingInfoUIPanel.name);
            if (!PhotonNetwork.IsConnected) {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        } else {
            Debug.Log("Nombre inválido");
        }
    }

    public void onLeaveGameButtonClick() {
        PhotonNetwork.LeaveRoom();
    }


    //Crear una room con un modo de juego específico
    //Se puede asignar visibilidad, propidad custom y maximo de jugadores
    //https://doc.photonengine.com/en-us/realtime/current/lobby-and-matchmaking/matchmaking-and-lobby
    public void CreateAndJoinRoom() {
        if (!string.IsNullOrEmpty(GameMode)) {
            ActivatePanel(CreatingRoomInfoUIPanel.name);
            string roomName = RoomNameInputField.text;
            if (string.IsNullOrEmpty(roomName)) {
                roomName = "Room: " + Random.Range(1000, 10000);
            }
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            roomOptions.EmptyRoomTtl = 0;

            //custom properties for lobby rc=racing  // dr=deathrace
            string[] roomPropsInLobby = { "GM", }; //game mode

            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "GM", GameMode } };
            roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    public void onCreateRoomButtonClick() {
        CreateAndJoinRoom();
    }

    public void OnJoinRandomRoomButtonClick(string _gameMode) {
        GameMode = _gameMode;
        ExitGames.Client.Photon.Hashtable expectedRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "GM", _gameMode } };
        PhotonNetwork.JoinRandomRoom(expectedRoomProperties, 0); //0 max players -> indefinido
    }

    public void OnStartButtonClick() {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("GM")) {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc")) {
                //Racing gamemode
                PhotonNetwork.LoadLevel("RacingScene");
            } else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr")) {
                //Death Race
                PhotonNetwork.LoadLevel("DeathRaceScene");

            }
        }
    }


    #endregion

    #region PHOTON CALLBACKS
    //Conexion a internet
    public override void OnConnected() {
        Debug.Log("Conectado a internet!!");
    }

    //conexion a photon server
    public override void OnConnectedToMaster() {
        ActivatePanel(GameOptionsUIPanel.name);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " se ha conctado a photon");
    }

    public override void OnCreatedRoom() {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created");
    }

    //agrega un jugador a la lista del caché, y a la UI
    public void AddPlayerToList(Player _player) {
        GameObject playerListGameObject = Instantiate(PlayerListPrefab);
        playerListGameObject.transform.SetParent(PlayerListContent.transform);
        playerListGameObject.transform.localScale = Vector3.one;
        playerListGameObject.GetComponent<PlayerListEntryInitializer>().initializer(_player.ActorNumber, _player.NickName);
        playerListGameobjects.Add(_player.ActorNumber, playerListGameObject);

        //lee el custom properties de otros players para    
        object isPlayerReady;
        if (_player.CustomProperties.TryGetValue(MultiplayerRacingGame.PLAYER_READY, out isPlayerReady)) {
            playerListGameObject.GetComponent<PlayerListEntryInitializer>().setPlayerReady((bool)isPlayerReady);
        }
    }

    public void updateRoomInfo() {
        //actualizar info de la room
        RoomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " +
        "Players / Max Players: " +
        PhotonNetwork.CurrentRoom.PlayerCount + " / " +
        PhotonNetwork.CurrentRoom.MaxPlayers;

        //actualizar imagen 
        //actualziar titulo
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc")) {
            GameModeText.text = "CARRERA";
            PanelBackground.sprite = RacingBackground;
            for (int i = 0; i < playerSelectionUIGameObjects.Length; i++) {
                playerSelectionUIGameObjects[i].transform.Find("PlayerName").GetComponent<Text>().text = racingPlayers[i].playerName;
                playerSelectionUIGameObjects[i].GetComponent<Image>().sprite = racingPlayers[i].playerSprite;
                playerSelectionUIGameObjects[i].transform.Find("PlayerProperty").GetComponent<Text>().text = "";
            }

        } else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr")) {
            GameModeText.text = "DEATH RACE";
            PanelBackground.sprite = DeathRaceBackground;

            for (int i = 0; i < playerSelectionUIGameObjects.Length; i++) {
                playerSelectionUIGameObjects[i].transform.Find("PlayerName").GetComponent<Text>().text = deathRacePlayers[i].playerName;
                playerSelectionUIGameObjects[i].GetComponent<Image>().sprite = deathRacePlayers[i].playerSprite;
                playerSelectionUIGameObjects[i].transform.Find("PlayerProperty").GetComponent<Text>().text = deathRacePlayers[i].weaponName +
                " - " + deathRacePlayers[i].damage + " - " + deathRacePlayers[i].bulletSpeed + " - " + deathRacePlayers[i].fireRate;
            }
        }

    }

    //unirse a la sala y consultar gamemode
    public override void OnJoinedRoom() {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
        ActivatePanel(InsideRoomUIPanel.name);
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("GM")) {

            //diccionario con los jugadores de la sala en local cache
            if (playerListGameobjects == null)
                playerListGameobjects = new Dictionary<int, GameObject>();

            updateRoomInfo();


            //Cargar jugadores en la sala
            foreach (Player _player in PhotonNetwork.PlayerList) {
                AddPlayerToList(_player);
            }
        }
        StartGameButton.SetActive(false);
    }

    //otro jugador entra cuando yo estoy dentro
    public override void OnPlayerEnteredRoom(Player newPlayer) {
        AddPlayerToList(newPlayer);
        updateRoomInfo();
        StartGameButton.SetActive(CheckPlayersReady());
    }

    //otro jugador sale de la room
    public override void OnPlayerLeftRoom(Player otherPlayer) {
        Destroy(playerListGameobjects[otherPlayer.ActorNumber].gameObject);
        playerListGameobjects.Remove(otherPlayer.ActorNumber);
        updateRoomInfo();
        StartGameButton.SetActive(CheckPlayersReady());
    }

    //yo salgo de la room
    public override void OnLeftRoom() {
        ActivatePanel(GameOptionsUIPanel.name);
        foreach (GameObject playerListObject in playerListGameobjects.Values) {
            Destroy(playerListObject);
        }
        playerListGameobjects.Clear(); playerListGameobjects = null;
    }


    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log(message);
        CreateAndJoinRoom();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {
        // if (targetPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber) return;
        //reflejar cambios de estado de otros jugadores en el estado local
        GameObject playerListGameObject;
        if (playerListGameobjects.TryGetValue(targetPlayer.ActorNumber, out playerListGameObject)) {
            object isPlayerReady;
            if (changedProps.TryGetValue(MultiplayerRacingGame.PLAYER_READY, out isPlayerReady)) {
                playerListGameObject.GetComponent<PlayerListEntryInitializer>().setPlayerReady((bool)isPlayerReady);
            }
        }
        StartGameButton.SetActive(CheckPlayersReady());
    }

    public override void OnMasterClientSwitched(Player newMasterClient) {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber) {
            StartGameButton.SetActive(CheckPlayersReady());
        }
    }
    #endregion

    #region Public Methods

    public void ActivatePanel(string panelNameToBeActivated) {
        LoginUIPanel.SetActive(LoginUIPanel.name.Equals(panelNameToBeActivated));
        ConnectingInfoUIPanel.SetActive(ConnectingInfoUIPanel.name.Equals(panelNameToBeActivated));
        CreatingRoomInfoUIPanel.SetActive(CreatingRoomInfoUIPanel.name.Equals(panelNameToBeActivated));
        CreateRoomUIPanel.SetActive(CreateRoomUIPanel.name.Equals(panelNameToBeActivated));
        GameOptionsUIPanel.SetActive(GameOptionsUIPanel.name.Equals(panelNameToBeActivated));
        JoinRandomRoomUIPanel.SetActive(JoinRandomRoomUIPanel.name.Equals(panelNameToBeActivated));
        InsideRoomUIPanel.SetActive(InsideRoomUIPanel.name.Equals(panelNameToBeActivated));
    }

    public void SetGameMode(string _gameMode) {
        GameMode = _gameMode;
    }
    #endregion

    #region PRIVATE METHODS
    private bool CheckPlayersReady() {
        //start solo disponible para owner
        if (!PhotonNetwork.IsMasterClient) return false;

        foreach (Player _player in PhotonNetwork.PlayerList) {
            object isPlayerReady;
            if (_player.CustomProperties.TryGetValue(MultiplayerRacingGame.PLAYER_READY, out isPlayerReady)) {
                if (!(bool)isPlayerReady) return false;
            } else return false;
        }
        return true;

    }
    #endregion
}
