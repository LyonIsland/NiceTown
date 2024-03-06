
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDetails///详情
{
    public int itemID;///物品id查询
    public string itemName;///物品名字itemName
    public ItemType itemType;///物品类型
    public Sprite itemIcon;///物品图标
    public Sprite itemOnWorldSprite;///物品在世界地图产生的图片
    public string itemDescription;///详情
    public int itemUseRadius;///物品可使用范围

    ///物品状态
    public bool canPickedup;///拾取
    public bool canDropped;///丢弃
    public bool canCarried;///举着
    public int itemPrice;///价值
    [Range(0, 1)]
    public float sellPercentage;///售卖的折扣
}

[System.Serializable]
public struct InventoryItem
{
    public int itemID;
    public int itemAmount;
}


[System.Serializable]
public class AnimatorType
{    
    public PartType partType;    
    public PartName partName;    
    public AnimatorOverrideController animatorOverride;
}

[System.Serializable]
public class SerializableVector3//可被序列化的坐标存储
{
    public float x, y, z;

    public SerializableVector3(Vector3 pos)//序列化
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int((int)x, (int)y);
    }
}

[System.Serializable]
public class SceneItem
{
    public int itemID;
    public SerializableVector3 position;//坐标
}

[System.Serializable]
public class SceneFurniture
{
    public int itemID;
    public SerializableVector3 position;
    public int boxIndex;
}

[System.Serializable]
public class TileProperty
{
    public Vector2Int tileCoordinate;
    public bool boolTypeValue;
    public GridType gridType;
  
}

[System.Serializable]
public class TileDetails
{
    public int girdX, gridY;
    public bool canDig;
    public bool canDropItem;
    public bool canPlaceFurniture;
    public bool isNPCObstacle;
    public int daysSinceDug = -1;//被挖天数
    public int daysSinceWatered = -1;//被浇水天数
    public int seedItemID = -1;//种子id
    public int growthDays = -1;//成长天数
    public int daysSinceLastHarvest = -1;//反复收割
}

[System.Serializable]
public class NPCPosition
{
    public Transform npc;
    public string startScene;
    public Vector3 position;
}

//场景路径
[System.Serializable]
public class SceneRoute
{
    public string fromSceneName;
    public string gotoSceneName;
    public List<ScenePath> scenePathList;
}

[System.Serializable]
public class ScenePath
{
    public string sceneName;
    public Vector2Int fromGridCell;
    public Vector2Int gotoGridCell;
}

//[System.Serializable]

//public class StoreLocation
//{
//    public Dictionary<string, Vector2Int> location;
//    public Dictionary<string, string> MatchScenes;
//    public void init()
//    {
//        location = new Dictionary<string, Vector2Int>();
//        MatchScenes = new Dictionary<string, string>();
//        location["宿舍"] = new Vector2Int(-11, 0);
//        location["食堂"] = new Vector2Int(3, -9);
//        location["学院"] = new Vector2Int(9, 3);
//        MatchScenes["宿舍"] = "UtilizeTestScene";
//        MatchScenes["食堂"] = "UtilizeTestScene";
//        MatchScenes["学院"] = "UtilizeTestScene";
//    }
//    public StoreLocation()
//    {
//        init();
//    }
//}

//从这儿开始是后加的部分

[System.Serializable]

//这段也要改
public class StoreLocation
{
    public Dictionary<string, Vector2Int> location;
    public Dictionary<string, string> MatchScenes;
    public void MyInit()
    {
        location = new Dictionary<string, Vector2Int>();
        MatchScenes = new Dictionary<string, string>();
        // furnitureList = new GameObject

        location["学院"] = new Vector2Int(84, 35);
        location["宿舍"] = new Vector2Int(94, 15);
        location["餐厅"] = new Vector2Int(73, 18);
        location["咖啡厅"] = new Vector2Int(29, 35);
        location["体育馆"] = new Vector2Int(58, 17);
        location["广场"] = new Vector2Int(-11, -16);
        location["商店"] = new Vector2Int(-8, -11);
        location["住宅"] = new Vector2Int(0, -3);
        //学院里的物品
        location["书桌A"] = new Vector2Int(-1, -6);
        location["书桌B"] = new Vector2Int(3, -6);
        location["咖啡桌"] = new Vector2Int(-10, -4);
        //宿舍里的物品
        location["床"] = new Vector2Int(2, -8);
        location["书桌"] = new Vector2Int(12, -7);
        //餐厅里的物品
        location["餐桌A"] = new Vector2Int(-4, -12);
        location["餐桌B"] = new Vector2Int(-4, -5);
        //咖啡厅里的物品
        location["咖啡桌A"] = new Vector2Int(6, -6);
        location["咖啡桌B"] = new Vector2Int(-3, -6);
        //体育馆
        //广场上的物品
        location["长椅A"] = new Vector2Int(-13, -14);
        location["长椅B"] = new Vector2Int(-16, -18);
        //商店里的物品
        location["柜子A"] = new Vector2Int(11, 1);
        location["柜子B"] = new Vector2Int(14, 1);
        //住宅里的物品
        location["床"] = new Vector2Int(6, -5);
        location["书桌"] = new Vector2Int(8, -7);

        MatchScenes["宿舍"] = "01.Field";//05.Dormintory
        MatchScenes["食堂"] = "01.Field";//06.Restaurant
        MatchScenes["学院"] = "01.Field";//04.Academy
        MatchScenes["餐厅"] = "01.Field";//06.Restaurant
        MatchScenes["体育馆"] = "01.Field";//09.Gym
        MatchScenes["广场"] = "03.Stall";//03.Stall
        MatchScenes["商店"] = "03.Stall";//08.Store
        MatchScenes["住宅"] = "01.Field";//02.Home


        location["餐桌"] = new Vector2Int(1, -2);
        MatchScenes["餐桌"] = "04.Academy";
        //记得回plan.json把7:00的environment这块改回来
    }
    public StoreLocation()
    {
        MyInit();
    }
}

[System.Serializable]

public class InteractObject
{
    public string name;
    //public GameObject obj;
    [SceneName] public string sceneName;//这些物品所在的场景 
    public Vector2Int position;//这些game object的位置
    public bool CanInteract = false;//是否可以与NPC互动
    public AnimationClip InteractAnime;//互动的clip
}

[System.Serializable]
//MissionComponent 类：存储Agent的名字、执行代码的时间
public class MissionComponent
{
    public string Agent;
    public int Day;
    public int version;
    public MissionDetails[] plan;
    public actions action;
    
}

[System.Serializable]

public class actions
{
    public string action;
    public string moveMin;
}

[System.Serializable]
//MissionDetails 类：存储每一个行为的具体内容（时间、地点、动作、是否有对话）
public class MissionDetails
{
    public string time;
    public string building;
    public string place;
    public string action;
    public int session;
}

[System.Serializable]
//DialogueComponent 类：一条对话的组成（轮次，角色，内容）
public class DialogueComponent
{
    //接收到的信息包括turn character dialogue
    public int turn;
    public string character;
    public string dialog;

    }

[System.Serializable]
//DialogueData 类：接收到的一份完整的对话内容（包括这条对话的id 主客体，时长，对话的具体内容）
public class DialogueData
{
    public int session_id;
    public string Subject;
    public string Object;
    public int duration;
    public DialogueComponent[] chat;
}



