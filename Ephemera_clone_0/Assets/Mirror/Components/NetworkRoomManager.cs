using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Mirror
{
    /// <summary>
    /// This is a specialized NetworkManager that includes a networked room.
    /// </summary>
    /// <remarks>
    /// <para>The room has slots that track the joined players, and a maximum player count that is enforced. It requires that the NetworkRoomPlayer component be on the room player objects.</para>
    /// <para>NetworkRoomManager is derived from NetworkManager, and so it implements many of the virtual functions provided by the NetworkManager class. To avoid accidentally replacing functionality of the NetworkRoomManager, there are new virtual functions on the NetworkRoomManager that begin with "OnRoom". These should be used on classes derived from NetworkRoomManager instead of the virtual functions on NetworkManager.</para>
    /// <para>The OnRoom*() functions have empty implementations on the NetworkRoomManager base class, so the base class functions do not have to be called.</para>
    /// </remarks>
    [AddComponentMenu("Network/Network Room Manager")]
    [HelpURL("https://mirror-networking.gitbook.io/docs/components/network-room-manager")]
    public class NetworkRoomManager : NetworkManager
    {
        public struct PendingPlayer
        {
            public NetworkConnectionToClient conn;
            public GameObject roomPlayer;
        }

        [Header("Room Settings")]
        [FormerlySerializedAs("m_ShowRoomGUI")]
        [SerializeField]
        [Tooltip("This flag controls whether the default UI is shown for the room, 이 플래그는 방에 기본 UI를 표시할지 여부를 제어합니다.")]
        public bool showRoomGUI = true;

        [FormerlySerializedAs("m_MinPlayers")]
        [SerializeField]
        [Tooltip("Minimum number of players to auto-start the game, 게임 자동 시작을 위한 최소 플레이어 수")]
        public int minPlayers = 1;

        [FormerlySerializedAs("m_RoomPlayerPrefab")]
        [SerializeField]
        [Tooltip("Prefab to use for the Room Player, 룸 플레이어에 사용하기 위한 프리팹")]
        public NetworkRoomPlayer roomPlayerPrefab;

        /// <summary>
        /// The scene to use for the room. This is similar to the offlineScene of the NetworkManager.
        /// 방에 사용할 씬입니다. NetworkManager의 오프라인 씬과 유사합니다.
        /// </summary>
        [Scene]
        public string RoomScene;

        /// <summary>
        /// The scene to use for the playing the game from the room. This is similar to the onlineScene of the NetworkManager.
        /// 방에서 게임을 플레이할 때 사용할 장면입니다. NetworkManager의 온라인 씬과 유사합니다.
        /// </summary>
        [Scene]
        public string GameplayScene;

        /// <summary>
        /// List of players that are in the Room
        /// 방에 있는 플레이어 목록
        /// </summary>
        [FormerlySerializedAs("m_PendingPlayers")]
        public List<PendingPlayer> pendingPlayers = new List<PendingPlayer>();

        [Header("Diagnostics")]
        /// <summary>
        /// True when all players have submitted a Ready message
        /// 모든 플레이어가 준비 메시지를 제출한 경우 True입니다.
        /// </summary>
        [Tooltip("Diagnostic flag indicating all players are ready to play")]
        [FormerlySerializedAs("allPlayersReady")]
        [ReadOnly, SerializeField] bool _allPlayersReady;

        /// <summary>
        /// These slots track players that enter the room.
        /// 이 슬롯은 방에 들어오는 플레이어를 추적합니다.
        /// <para>The slotId on players is global to the game - across all players.</para>
        /// <para>플레이어의 슬롯아이디는 모든 플레이어에 걸쳐 게임에 글로벌하게 적용됩니다.</para>
        /// </summary>
        [ReadOnly, Tooltip("List of Room Player objects")]
        public List<NetworkRoomPlayer> roomSlots = new List<NetworkRoomPlayer>();

        public bool allPlayersReady
        {
            get => _allPlayersReady;
            set
            {
                bool wasReady = _allPlayersReady;
                bool nowReady = value;

                if (wasReady != nowReady)
                {
                    _allPlayersReady = value;

                    if (nowReady)
                    {
                        OnRoomServerPlayersReady();
                    }
                    else
                    {
                        OnRoomServerPlayersNotReady();
                    }
                }
            }
        }

        public override void OnValidate()
        {
            base.OnValidate();

            // always <= maxConnections
            minPlayers = Mathf.Min(minPlayers, maxConnections);

            // always >= 0
            minPlayers = Mathf.Max(minPlayers, 0);

            if (roomPlayerPrefab != null)
            {
                NetworkIdentity identity = roomPlayerPrefab.GetComponent<NetworkIdentity>();
                if (identity == null)
                {
                    roomPlayerPrefab = null;
                    Debug.LogError("RoomPlayer prefab must have a NetworkIdentity component.");
                }
            }
        }

        void SceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
        {
            //Debug.Log($"NetworkRoom SceneLoadedForPlayer scene: {SceneManager.GetActiveScene().path} {conn}");

            if (Utils.IsSceneActive(RoomScene))
            {
                // cant be ready in room, add to ready list
                PendingPlayer pending;
                pending.conn = conn;
                pending.roomPlayer = roomPlayer;
                pendingPlayers.Add(pending);
                return;
            }

            GameObject gamePlayer = OnRoomServerCreateGamePlayer(conn, roomPlayer);
            if (gamePlayer == null)
            {
                // get start position from base class
                Transform startPos = GetStartPosition();
                gamePlayer = startPos != null
                    ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                    : Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            }

            if (!OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer))
                return;

            // replace room player with game player
            NetworkServer.ReplacePlayerForConnection(conn, gamePlayer, true);
        }

        internal void CallOnClientEnterRoom()
        {
            OnRoomClientEnter();
            foreach (NetworkRoomPlayer player in roomSlots)
                if (player != null)
                {
                    player.OnClientEnterRoom();
                }
        }

        internal void CallOnClientExitRoom()
        {
            OnRoomClientExit();
            foreach (NetworkRoomPlayer player in roomSlots)
                if (player != null)
                {
                    player.OnClientExitRoom();
                }
        }

        /// <summary>
        /// CheckReadyToBegin checks all of the players in the room to see if their readyToBegin flag is set.
        /// CheckReadyToBegin은 방에 있는 모든 플레이어의 준비 완료 플래그가 설정되어 있는지 확인합니다.
        /// <para>If all of the players are ready, then the server switches from the RoomScene to the PlayScene, essentially starting the game. This is called automatically in response to NetworkRoomPlayer.CmdChangeReadyState.</para>
        /// <para>모든 플레이어가 준비되면 서버는 룸 씬에서 플레이 씬으로 전환하여 기본적으로 게임을 시작합니다. 이 함수는 NetworkRoomPlayer.CmdChangeReadyState에 대한 응답으로 자동으로 호출됩니다.</para>
        /// </summary>
        public void CheckReadyToBegin()
        {
            if (!Utils.IsSceneActive(RoomScene))
                return;

            int numberOfReadyPlayers = NetworkServer.connections.Count(conn =>
                conn.Value != null &&
                conn.Value.identity != null &&
                conn.Value.identity.TryGetComponent(out NetworkRoomPlayer nrp) &&
                nrp.readyToBegin);

            bool enoughReadyPlayers = minPlayers <= 0 || numberOfReadyPlayers >= minPlayers;
            if (enoughReadyPlayers)
            {
                pendingPlayers.Clear();
                allPlayersReady = true;
            }
            else
                allPlayersReady = false;
        }

        #region server handlers

        /// <summary>
        /// Called on the server when a new client connects.
        /// <para>Unity calls this on the Server when a Client connects to the Server. Use an override to tell the NetworkManager what to do when a client connects to the server.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            // cannot join game in progress
            if (!Utils.IsSceneActive(RoomScene))
            {
                Debug.Log($"Not in Room scene...disconnecting {conn}");
                conn.Disconnect();
                return;
            }

            base.OnServerConnect(conn);
            OnRoomServerConnect(conn);
        }

        /// <summary>
        /// Called on the server when a client disconnects.
        /// <para>This is called on the Server when a Client disconnects from the Server. Use an override to decide what should happen when a disconnection is detected.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            if (conn.identity != null)
            {
                NetworkRoomPlayer roomPlayer = conn.identity.GetComponent<NetworkRoomPlayer>();

                if (roomPlayer != null)
                    roomSlots.Remove(roomPlayer);

                foreach (NetworkIdentity clientOwnedObject in conn.owned)
                {
                    roomPlayer = clientOwnedObject.GetComponent<NetworkRoomPlayer>();
                    if (roomPlayer != null)
                        roomSlots.Remove(roomPlayer);
                }
            }

            allPlayersReady = false;

            foreach (NetworkRoomPlayer player in roomSlots)
            {
                if (player != null)
                    player.GetComponent<NetworkRoomPlayer>().readyToBegin = false;
            }

            if (Utils.IsSceneActive(RoomScene))
                RecalculateRoomPlayerIndices();

            OnRoomServerDisconnect(conn);
            base.OnServerDisconnect(conn);

            if (Utils.IsHeadless())
            {
                if (numPlayers < 1)
                    StopServer();
            }
        }

        // Sequential index used in round-robin deployment of players into instances and score positioning
        public int clientIndex;

        /// <summary>
        /// Called on the server when a client is ready.
        /// <para>The default implementation of this function calls NetworkServer.SetClientReady() to continue the network setup process.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            //Debug.Log($"NetworkRoomManager OnServerReady {conn}");
            base.OnServerReady(conn);

            if (conn != null && conn.identity != null)
            {
                GameObject roomPlayer = conn.identity.gameObject;

                // if null or not a room player, don't replace it
                if (roomPlayer != null && roomPlayer.GetComponent<NetworkRoomPlayer>() != null)
                    SceneLoadedForPlayer(conn, roomPlayer);
            }
        }

        /// <summary>
        /// Called on the server when a client adds a new player with NetworkClient.AddPlayer.
        /// <para>The default implementation for this function creates a new player object from the playerPrefab.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            // increment the index before adding the player, so first player starts at 1
            clientIndex++;

            if (Utils.IsSceneActive(RoomScene))
            {
                allPlayersReady = false;

                //Debug.Log("NetworkRoomManager.OnServerAddPlayer playerPrefab: {roomPlayerPrefab.name}");

                GameObject newRoomGameObject = OnRoomServerCreateRoomPlayer(conn);
                if (newRoomGameObject == null)
                    newRoomGameObject = Instantiate(roomPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);

                NetworkServer.AddPlayerForConnection(conn, newRoomGameObject);
            }
            else
            {
                // Late joiners not supported...should've been kicked by OnServerDisconnect
                Debug.Log($"Not in Room scene...disconnecting {conn}");
                conn.Disconnect();
            }
        }

        [Server]
        public void RecalculateRoomPlayerIndices()
        {
            if (roomSlots.Count > 0)
            {
                for (int i = 0; i < roomSlots.Count; i++)
                {
                    roomSlots[i].index = i;
                }
            }
        }

        /// <summary>
        /// This causes the server to switch scenes and sets the networkSceneName.
        /// <para>Clients that connect to this server will automatically switch to this scene. This is called automatically if onlineScene or offlineScene are set, but it can be called from user code to switch scenes again while the game is in progress. This automatically sets clients to be not-ready. The clients must call NetworkClient.Ready() again to participate in the new scene.</para>
        /// </summary>
        /// <param name="newSceneName"></param>
        public override void ServerChangeScene(string newSceneName)
        {
            if (newSceneName == RoomScene)
            {
                foreach (NetworkRoomPlayer roomPlayer in roomSlots)
                {
                    if (roomPlayer == null)
                        continue;

                    // find the game-player object for this connection, and destroy it
                    NetworkIdentity identity = roomPlayer.GetComponent<NetworkIdentity>();

                    if (NetworkServer.active)
                    {
                        // re-add the room object
                        roomPlayer.GetComponent<NetworkRoomPlayer>().readyToBegin = false;
                        NetworkServer.ReplacePlayerForConnection(identity.connectionToClient, roomPlayer.gameObject);
                    }
                }

                allPlayersReady = false;
            }

            base.ServerChangeScene(newSceneName);
        }

        /// <summary>
        /// Called on the server when a scene is completed loaded, when the scene load was initiated by the server with ServerChangeScene().
        /// </summary>
        /// <param name="sceneName">The name of the new scene.</param>
        public override void OnServerSceneChanged(string sceneName)
        {
            if (sceneName != RoomScene)
            {
                // call SceneLoadedForPlayer on any players that become ready while we were loading the scene.
                foreach (PendingPlayer pending in pendingPlayers)
                    SceneLoadedForPlayer(pending.conn, pending.roomPlayer);

                pendingPlayers.Clear();
            }

            OnRoomServerSceneChanged(sceneName);
        }

        /// <summary>
        /// This is invoked when a server is started - including when a host is started.
        /// <para>StartServer has multiple signatures, but they all cause this hook to be called.</para>
        /// </summary>
        public override void OnStartServer()
        {
            if (string.IsNullOrWhiteSpace(RoomScene))
            {
                Debug.LogError("NetworkRoomManager RoomScene is empty. Set the RoomScene in the inspector for the NetworkRoomManager");
                return;
            }

            if (string.IsNullOrWhiteSpace(GameplayScene))
            {
                Debug.LogError("NetworkRoomManager PlayScene is empty. Set the PlayScene in the inspector for the NetworkRoomManager");
                return;
            }

            OnRoomStartServer();
        }

        /// <summary>
        /// This is invoked when a host is started.
        /// <para>StartHost has multiple signatures, but they all cause this hook to be called.</para>
        /// </summary>
        public override void OnStartHost()
        {
            OnRoomStartHost();
        }

        /// <summary>
        /// This is called when a server is stopped - including when a host is stopped.
        /// </summary>
        public override void OnStopServer()
        {
            roomSlots.Clear();
            OnRoomStopServer();
        }

        /// <summary>
        /// This is called when a host is stopped.
        /// </summary>
        public override void OnStopHost()
        {
            OnRoomStopHost();
        }

        #endregion

        #region client handlers

        /// <summary>
        /// This is invoked when the client is started.
        /// </summary>
        public override void OnStartClient()
        {
            if (roomPlayerPrefab == null || roomPlayerPrefab.gameObject == null)
                Debug.LogError("NetworkRoomManager no RoomPlayer prefab is registered. Please add a RoomPlayer prefab.");
            else
                NetworkClient.RegisterPrefab(roomPlayerPrefab.gameObject);

            if (playerPrefab == null)
                Debug.LogError("NetworkRoomManager no GamePlayer prefab is registered. Please add a GamePlayer prefab.");

            OnRoomStartClient();
        }

        /// <summary>
        /// Called on the client when connected to a server.
        /// <para>The default implementation of this function sets the client as ready and adds a player. Override the function to dictate what happens when the client connects.</para>
        /// </summary>
        public override void OnClientConnect()
        {
            OnRoomClientConnect();
            base.OnClientConnect();
        }

        /// <summary>
        /// Called on clients when disconnected from a server.
        /// <para>This is called on the client when it disconnects from the server. Override this function to decide what happens when the client disconnects.</para>
        /// </summary>
        public override void OnClientDisconnect()
        {
            OnRoomClientDisconnect();
            base.OnClientDisconnect();
        }

        /// <summary>
        /// This is called when a client is stopped.
        /// </summary>
        public override void OnStopClient()
        {
            OnRoomStopClient();
            CallOnClientExitRoom();
            roomSlots.Clear();
        }

        /// <summary>
        /// Called on clients when a scene has completed loaded, when the scene load was initiated by the server.
        /// <para>Scene changes can cause player objects to be destroyed. The default implementation of OnClientSceneChanged in the NetworkManager is to add a player object for the connection if no player object exists.</para>
        /// </summary>
        public override void OnClientSceneChanged()
        {
            if (Utils.IsSceneActive(RoomScene))
            {
                if (NetworkClient.isConnected)
                    CallOnClientEnterRoom();
            }
            else
                CallOnClientExitRoom();

            base.OnClientSceneChanged();
            OnRoomClientSceneChanged();
        }

        #endregion

        #region room server virtuals

        /// <summary>
        /// This is called on the host when a host is started.
        /// 호스트가 시작될 때 호스트에서 호출됩니다.
        /// </summary>
        public virtual void OnRoomStartHost() {}

        /// <summary>
        /// This is called on the host when the host is stopped.
        /// 호스트가 중지되면 호스트에서 호출됩니다.
        /// </summary>
        public virtual void OnRoomStopHost() {}

        /// <summary>
        /// This is called on the server when the server is started - including when a host is started.
        /// 호스트가 시작될 때를 포함하여 서버가 시작될 때 서버에서 호출됩니다.
        /// </summary>
        public virtual void OnRoomStartServer() {}

        /// <summary>
        /// This is called on the server when the server is started - including when a host is stopped.
        /// 호스트가 중지된 경우를 포함하여 서버가 시작될 때 서버에서 호출됩니다.
        /// </summary>
        public virtual void OnRoomStopServer() {}

        /// <summary>
        /// This is called on the server when a new client connects to the server.
        /// 새 클라이언트가 서버에 연결할 때 서버에서 호출됩니다.
        /// </summary>
        /// <param name="conn">The new connection.</param>
        public virtual void OnRoomServerConnect(NetworkConnectionToClient conn) {}

        /// <summary>
        /// This is called on the server when a client disconnects.
        /// 클라이언트가 연결을 끊을 때 서버에서 호출됩니다.
        /// </summary>
        /// <param name="conn">The connection that disconnected.</param>
        public virtual void OnRoomServerDisconnect(NetworkConnectionToClient conn) {}

        /// <summary>
        /// This is called on the server when a networked scene finishes loading.
        /// 네트워크 씬 로딩이 완료되면 서버에서 호출됩니다.
        /// </summary>
        /// <param name="sceneName">Name of the new scene.</param>
        public virtual void OnRoomServerSceneChanged(string sceneName) {}

        /// <summary>
        /// This allows customization of the creation of the room-player object on the server.
        /// 이를 통해 서버에서 룸 플레이어 오브젝트 생성을 커스터마이징할 수 있습니다.
        /// <para>By default the roomPlayerPrefab is used to create the room-player, but this function allows that behaviour to be customized.</para>
        /// <para>기본적으로 룸 플레이어를 생성하는 데는 roomPlayerPrefab이 사용되지만 이 함수를 사용하면 해당 동작을 사용자 지정할 수 있습니다.</para>
        /// </summary>
        /// <param name="conn">The connection the player object is for.</param>
        /// <returns>The new room-player object.</returns>
        public virtual GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
        {
            return null;
        }

        /// <summary>
        /// This allows customization of the creation of the GamePlayer object on the server.
        /// 이를 통해 서버에서 게임플레이어 오브젝트 생성을 커스터마이징할 수 있습니다.
        /// <para>By default the gamePlayerPrefab is used to create the game-player, but this function allows that behaviour to be customized. The object returned from the function will be used to replace the room-player on the connection.</para>
        /// <para>기본적으로 게임 플레이어를 생성하는 데는 게임플레이어 프리팹이 사용되지만 이 함수를 사용하면 해당 동작을 사용자 지정할 수 있습니다. 함수에서 반환된 객체는 연결에서 룸 플레이어를 대체하는 데 사용됩니다.</para>
        /// </summary>
        /// <param name="conn">The connection the player object is for.</param>
        /// <param name="roomPlayer">The room player object for this connection.</param>
        /// <returns>A new GamePlayer object.</returns>
        public virtual GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
        {
            return null;
        }

        /// <summary>
        /// This allows customization of the creation of the GamePlayer object on the server.
        /// 이를 통해 서버에서 게임플레이어 오브젝트 생성을 커스터마이징할 수 있습니다.
        /// <para>This is only called for subsequent GamePlay scenes after the first one.</para>
        /// <para>See <see cref="OnRoomServerCreateGamePlayer(NetworkConnectionToClient, GameObject)">OnRoomServerCreateGamePlayer(NetworkConnection, GameObject)</see> to customize the player object for the initial GamePlay scene.</para>
        /// </summary>
        /// <param name="conn">The connection the player object is for.</param>
        public virtual void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
        }

        // for users to apply settings from their room player object to their in-game player object
        /// <summary>
        /// This is called on the server when it is told that a client has finished switching from the room scene to a game player scene.
        /// 클라이언트가 방 씬에서 게임 플레이어 씬으로 전환이 완료되었다는 알림을 받으면 서버에서 호출됩니다.
        /// <para>When switching from the room, the room-player is replaced with a game-player object. This callback function gives an opportunity to apply state from the room-player to the game-player object.</para>
        /// </summary>
        /// <param name="conn">The connection of the player</param>
        /// <param name="roomPlayer">The room player object.</param>
        /// <param name="gamePlayer">The game player object.</param>
        /// <returns>False to not allow this player to replace the room player.</returns>
        public virtual bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            return true;
        }

        /// <summary>
        /// This is called on server from NetworkRoomPlayer.CmdChangeReadyState when client indicates change in Ready status.
        /// </summary>
        public virtual void ReadyStatusChanged()
        {
            int CurrentPlayers = 0;
            int ReadyPlayers = 0;

            foreach (NetworkRoomPlayer item in roomSlots)
            {
                if (item != null)
                {
                    CurrentPlayers++;
                    if (item.readyToBegin)
                        ReadyPlayers++;
                }
            }

            if (CurrentPlayers == ReadyPlayers)
                CheckReadyToBegin();
            else
                allPlayersReady = false;
        }

        /// <summary>
        /// This is called on the server when all the players in the room are ready.
        /// <para>The default implementation of this function uses ServerChangeScene() to switch to the game player scene. By implementing this callback you can customize what happens when all the players in the room are ready, such as adding a countdown or a confirmation for a group leader.</para>
        /// </summary>
        public virtual void OnRoomServerPlayersReady()
        {
            // all players are readyToBegin, start the game
            ServerChangeScene(GameplayScene);
        }

        /// <summary>
        /// This is called on the server when CheckReadyToBegin finds that players are not ready
        /// <para>May be called multiple times while not ready players are joining</para>
        /// </summary>
        public virtual void OnRoomServerPlayersNotReady() {}

        #endregion

        #region room client virtuals

        /// <summary>
        /// This is a hook to allow custom behaviour when the game client enters the room.
        /// </summary>
        public virtual void OnRoomClientEnter() {}

        /// <summary>
        /// This is a hook to allow custom behaviour when the game client exits the room.
        /// </summary>
        public virtual void OnRoomClientExit() {}

        /// <summary>
        /// This is called on the client when it connects to server.
        /// </summary>
        public virtual void OnRoomClientConnect() {}

        /// <summary>
        /// This is called on the client when disconnected from a server.
        /// </summary>
        public virtual void OnRoomClientDisconnect() {}

        /// <summary>
        /// This is called on the client when a client is started.
        /// </summary>
        public virtual void OnRoomStartClient() {}

        /// <summary>
        /// This is called on the client when the client stops.
        /// </summary>
        public virtual void OnRoomStopClient() {}

        /// <summary>
        /// This is called on the client when the client is finished loading a new networked scene.
        /// </summary>
        public virtual void OnRoomClientSceneChanged() {}

        #endregion

        #region optional UI

        /// <summary>
        /// virtual so inheriting classes can roll their own
        /// </summary>
        public virtual void OnGUI()
        {
            if (!showRoomGUI)
                return;

            if (NetworkServer.active && Utils.IsSceneActive(GameplayScene))
            {
                GUILayout.BeginArea(new Rect(Screen.width - 150f, 10f, 140f, 30f));
                if (GUILayout.Button("Return to Room"))
                    ServerChangeScene(RoomScene);
                GUILayout.EndArea();
            }

            if (Utils.IsSceneActive(RoomScene))
                GUI.Box(new Rect(10f, 180f, 520f, 150f), "PLAYERS");
        }

        #endregion
    }
}
