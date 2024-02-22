using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPCManager : Singleton<NPCManager>
{
    public SceneRouteDataList_SO sceneRouteDate;
    public List<NPCPosition> npcPositionList;
    public List<MissionComponent> AllMissions = new List<MissionComponent>();
    public List<string> NPC_ID = new List<string>() { "A1", "A2", "A3", "A4", "A5" };
    public Dictionary<string, List<MissionDetails>> ToDoDict=new Dictionary<string, List<MissionDetails>>();//根据id去检索到对应的人，并将任务分配到他头上
    public Dictionary<string, MissionComponent> MatchNPCbyID = new Dictionary<string, MissionComponent>();
    //先初始化五个初始npc成员姓名
    private Dictionary<string, SceneRoute> sceneRouteDict = new Dictionary<string, SceneRoute>();

    protected override void Awake()
    {
        Debug.Log("npc manager awake");
        base.Awake();
        if (NPCManager.Instance == null)
            Debug.Log(6);
        InitSceneRouteDict();
        Debug.Log("npc manager awake end");
    }

    private void OnEnable()
    {
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }
    //----这个可能是工作内容----
    private void OnStartNewGameEvent(int obj)
    {
        Debug.Log("NewGameEventActivated");
        foreach (var character in npcPositionList)
        {
            Debug.Log("---" + character.ToString());
            character.npc.position = character.position;
            character.npc.GetComponent<NPCMovement>().currentScene = character.startScene;
        }
    }

    /// <summary>
    /// 初始化路径字典
    /// </summary>
    private void InitSceneRouteDict()
    {
        if (sceneRouteDate.sceneRouteList.Count > 0)
        {
            foreach (SceneRoute route in sceneRouteDate.sceneRouteList)
            {
                var key = route.fromSceneName + route.gotoSceneName;

                if (sceneRouteDict.ContainsKey(key))
                    continue;

                sceneRouteDict.Add(key, route);
            }
        }
    }

    /// <summary>
    /// 获得两个场景间的路径
    /// </summary>
    /// <param name="fromSceneName">起始场景</param>
    /// <param name="gotoSceneName">目标场景</param>
    /// <returns></returns>
    public SceneRoute GetSceneRoute(string fromSceneName, string gotoSceneName)
    {
        return sceneRouteDict[fromSceneName + gotoSceneName];
    }
}
