using UnityEngine;
using DunGen;

public class DungeonGeneration : MonoBehaviour
{
    public DungeonGenerator dungeonGenerator; // ���� ������ ������ �Ǵ� ��ũ��Ʈ ��ü
    public RuntimeDungeon runtimeDungeon;

    private void Start()
    {
        dungeonGenerator.Generate();
        Debug.Log(GetStartTilePosition());

    }

    private Vector3 GetStartTilePosition()
    {
        if (dungeonGenerator.CurrentDungeon != null && dungeonGenerator.CurrentDungeon.MainPathTiles != null)
        {
            // ���� Ÿ�� ��������
            var startTile = dungeonGenerator.CurrentDungeon.MainPathTiles[0];
            //var endTile = dungeonGenerator.CurrentDungeon.MainPathTiles[dungeonGenerator.CurrentDungeon.MainPathTiles.Count - 1];

            if (startTile != null)
            {
                //Ÿ���� ��ġ ��ȯ
                return startTile.Entrance.transform.position;
            }

        }
        Debug.LogError("Start tile not found");
        return Vector3.zero;
    }
}