using Mirror;
using UnityEngine;
public struct CreateCharacterMessage : NetworkMessage
{
    public string name;
}
public struct CreateRoomCharacterMessage : NetworkMessage
{
    public string name;
}
/*public struct PlanetActivateMessage : NetworkMessage 
{
    public Planet planet;
    public bool isActive;
}*/