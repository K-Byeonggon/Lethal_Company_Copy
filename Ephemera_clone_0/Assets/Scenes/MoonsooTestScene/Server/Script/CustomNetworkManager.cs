using Mirror;
using Mirror.Examples.CharacterSelection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField]
    GameObject playerMovePrefab;
    public override void OnServerConnect(NetworkConnectionToClient conn) 
    {
        GameObject playerObject = Instantiate(playerMovePrefab);

        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, playerObject);
    }
}
