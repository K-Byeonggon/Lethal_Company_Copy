using System;
using UnityEngine;

public enum ObjectType
{
    Player,
    Monster,
}

public enum CanvasType
{
    PopupBackCanvas,
    PopupFrontCanvas,
    SceneInformationCanvas,
    SceneStaticCanvas,
    OverlayCanvas,
}
public enum UIType
{

    GameStatic,
    SkillPanel,
    InventoryBack,
    InventoryFront,
    SkillTreeBack,
    SkillTreeFront,
    InformationOverlay,
    MenuBack,
    MenuFront,
}

#region 몬스터
public enum MonsterType
{
    FlyingEye,
    Goblin,
    Mushroom,
    Skeleton,
    MaxCount,
}
public enum BossType
{
    BigFlyingEye,
    GoblinKing,
    SuperMushroom,
    UltimateSkeleton,
    MaxCount,
}
#endregion

#region 퀘스트 보상
public enum RewardType
{
    //돈
    Gold,
    //장비
    Equipment,
    //경험치
    Exp,
    //스킬 포인트
    SkillPoint,
}
#endregion

#region 상태이상
//버프의 종류
public enum SkillBuffType
{
    //없음
    None,
    //공격력 증가
    IncreaseAttack,
    //이동속도 증가
    IncreaseMoveSpeed,
    //경직 면역
    StiffImmunity,
    //체력 회복
    StaminaRecovery,
}
//디버프의 종류
public enum SkillDebuffType
{
    //없음
    None,
    //둔화
    Slowdown,
    //취약
    Weak,
    //빙결
    Freezing,
    //감전
    Shock,
    //기절
    Stun,
    //출혈
    Bleeding,
}
#endregion

#region 스킬타입
public enum SkillEquipmentType
{
    Attack,
    Skill,
    Special,
    MaxCount
}
public enum SkillRangeType
{
    Circle,
    SemiCircle,
    Box,
}
public enum SkillMovementType
{
    Idle,
    Rush,
    Follow,
    SwapPosition,
    Teleportation,
}
public enum SkillDirectionType
{
    Idle,
    Front,
    Back,
    Left,
    Right,
}
public enum SkillTargetMovementType
{
    //없음
    None,
    //당기다
    Grab,
    //밀치다
    Thrust,
}
public enum SkillSlotType
{
    SkillTree,
    SkillEquip,
}
public enum SkillTreeType
{
    Fire,
    Frost,
    Tetanus
}
#endregion

#region 아이템
public enum ItemSlotType
{
    Equipment,
    Inventory,
}
public enum EquipType
{
    First,
    Second,
    Third,
}
public enum IntrinsicProperties
{
    None,
    VenomousSnake,
    PermanentOrgan,
}
public enum ItemStatType
{
    None,
    Hp,
    Mp,
    Attack,
    Defence,
}

#endregion

public enum AtlasType
{
    TileSprite,
    UnitSprite,
}