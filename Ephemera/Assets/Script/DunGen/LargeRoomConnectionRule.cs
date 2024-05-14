using DunGen;
using DunGen.Tags;
using UnityEngine;

public class LargeRoomConnectionRule : MonoBehaviour
{
    [SerializeField] DoorwaySocket LargeSocket;
    public Tag LargeRoomTag;

    private TileConnectionRule rule;


    private void OnEnable()
    {
        rule = new TileConnectionRule(CanTilesConnect);
        DoorwayPairFinder.CustomConnectionRules.Add(rule);
    }

    private void OnDisable()
    {
        DoorwayPairFinder.CustomConnectionRules.Remove(rule);
        rule = null;
    }

    private TileConnectionRule.ConnectionResult CanTilesConnect(Tile tileA, Tile tileB, Doorway doorwayA, Doorway doorwayB)
    {
        // Check if the two tiles are large. This is using DunGen's tag system, but we could
        // also check the tile names, or look for a specific component
        bool tileAIsLarge = tileA.Tags.HasTag(LargeRoomTag);
        bool tileBIsLarge = tileB.Tags.HasTag(LargeRoomTag);

        // Are we interested in this connection?...
        if (tileAIsLarge && tileBIsLarge)
        {
            // If both sockets are large, allow the connection, otherwise deny
            if (doorwayA.Socket == doorwayB.Socket && doorwayA.Socket == LargeSocket)
                return TileConnectionRule.ConnectionResult.Allow;
            else
                return TileConnectionRule.ConnectionResult.Deny;
        }
        // ...if not, pass it on to be handled later
        else
            return TileConnectionRule.ConnectionResult.Passthrough;
    }
}
