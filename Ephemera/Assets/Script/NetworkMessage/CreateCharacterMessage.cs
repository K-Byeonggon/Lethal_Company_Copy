using Mirror;
public struct CreateCharacterMessage : NetworkMessage
{
    public string name;
}
public struct CreateRoomCharacterMessage : NetworkMessage
{
    public string name;
}