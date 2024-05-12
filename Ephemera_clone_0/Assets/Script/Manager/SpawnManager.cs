using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : SingleTon<SpawnManager>
{
    string playerStatusBar = "UI_PlayerStatusBar";
    string monsterStatusBar = "UI_MonsterStatusBar";
    //string bossStatusBar = "UI_BossStatusBar";

    public GameObject SpawnPlayer(GameObject playerPrefab, GameObject bladePrefab, Vector3 pos)
    {
        /*//플레이어, 무기 생성
        GameObject player = Instantiate(playerPrefab, pos, Quaternion.identity);
        GameObject blade = Instantiate(bladePrefab);
        player.name = playerPrefab.name;
        blade.name = bladePrefab.name;

        //카메라 타겟 설정
        Camera camera = Camera.main;
        camera.transform.position = player.transform.position;
        camera.gameObject.GetComponent<CameraFollow>().Target = player.transform;

        //플레이어 초기화
        Player playerComponent = player.GetComponent<Player>();
        playerComponent.Init(playerStatusBar);
        WeaponController weaponController = blade.GetComponent<WeaponController>();
        playerComponent.SetWeaponController(weaponController);
        weaponController.Player = playerComponent;
        GameManager.Instance.player = playerComponent;
        GameManager.Instance.playerController = playerComponent.GetPlayerController();
        */
        return null;
    }
    public GameObject SpawnMonster(MonsterType type, Vector3 pos)
    {
        /*var prefab = ResourceManager.Instance.GetPrefab(type.ToString());
        GameObject go = ObjectPool.Instance.GetGameObject(prefab);


        *//*Vector3 sourcePostion = pos;//The position you want to place your agent
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(sourcePostion, out closestHit, 500, 1))
        {
            go.transform.position = closestHit.position;
            
            //TODO
        }*//*
        go.transform.position = pos;

        Monster monster = go.GetComponent<Monster>();
        monster.Init(monsterStatusBar);
        GameManager.Instance.AddMonster(monster);

        go.GetComponent<NavMeshAgent>().enabled = true;
        */
        return null;
    }
    public GameObject SpawnBoss(BossType type, Vector3 pos)
    {
        /*var prefab = ResourceManager.Instance.GetPrefab(type.ToString());
        GameObject go = ObjectPool.Instance.GetGameObject(prefab);

        go.transform.position = pos;

        Boss monster = go.GetComponent<Boss>();
        monster.Init(monsterStatusBar);
        GameManager.Instance.AddMonster(monster);

        go.GetComponent<NavMeshAgent>().enabled = true;*/
        return null;
    }
    public GameObject SpawnUnit(GameObject prefab)
    {
        /*GameObject go = ObjectPool.Instance.GetGameObject(prefab);
        go.GetComponent<Unit>().Init(monsterStatusBar);*/
        return null;
    }
}
