using UnityEngine;
using DunGen;

public class DungeonGeneration : MonoBehaviour
{
    public DungeonGenerator dungeonGenerator; // 던전 생성기 프리팹 또는 스크립트 객체
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
            // 시작 타일 가져오기
            var startTile = dungeonGenerator.CurrentDungeon.MainPathTiles[0];
            //var endTile = dungeonGenerator.CurrentDungeon.MainPathTiles[dungeonGenerator.CurrentDungeon.MainPathTiles.Count - 1];

            if (startTile != null)
            {
                //타일의 위치 반환
                return startTile.Entrance.transform.position;
            }

        }
        Debug.LogError("Start tile not found");
        return Vector3.zero;
    }
}