using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using CloudCommunication;

[System.Serializable]
public class AgentPlan : MonoBehaviour
{
    [Header("文本文件")]
    public TextAsset Plans;
    public int index = 0;
    List<MissionDetails> Stuff=new List<MissionDetails>();
    MissionComponent ReceivedMission;
    //public string WholePlan;
    // Start is called before the first frame update
    void Awake()
    {

        CloudCommunication.CloudCommunication.Starter();

        index = 0;
        /*
        Debug.Log("agent plan awake");
        Debug.Log("in agent plan's awake, NPCManager is null? " + (NPCManager.Instance == null).ToString());
        */
        if (NPCManager.Instance == null)
            Debug.Log(2);
        ReceivedMission = JsonConvert.DeserializeObject<MissionComponent>(Plans.text);
        ReadData(Plans);
        dispatchStuff();
        Debug.Log("agent awake finish");
    }

    private void Start()
    {
        Debug.Log("agent plan start");

        if (NPCManager.Instance == null)
            Debug.Log(5);
        Debug.Log("agent plan end");
    }
    public void dispatchStuff()
    {
        
        foreach(var NPC_id in NPCManager.Instance.NPC_ID)
        {
            Debug.Log("NPC_ID:"+NPC_id.ToString());
            if (NPC_id == ReceivedMission.Agent)
            {
                NPCManager.Instance.MatchNPCbyID[NPC_id] = ReceivedMission;
                Debug.Log(NPCManager.Instance.MatchNPCbyID[NPC_id].Agent);
                NPCManager.Instance.ToDoDict[NPC_id] = Stuff;
                
                break;//找到这个计划的归属者是谁
            }
        }
        Debug.Log("任务分配完成");
        
    }

    public void ReadData(TextAsset file)
    {
        Debug.Log("reading schedule");

        if (NPCManager.Instance == null)
            Debug.Log(1);
        NPCManager.Instance.AllMissions.Add(ReceivedMission);//将所有任务输入到一个人头上
        foreach(var MD in ReceivedMission.plan)
        {
            Stuff.Add(MD);
        }
    }
    private void FixedUpdate()
    {
        //如果换到下一天则重置任务列表
        /*
        Debug.Log(ReceivedMission.Day.ToString() + "--" + TimeControlSystem.TimeControl.Day.ToString());
        if (ReceivedMission.Day < TimeControlSystem.TimeControl.Day)
        {
            Stuff.Clear();
            foreach(string id in NPCManager.Instance.NPC_ID)
            {
                NPCManager.Instance.ToDoDict[id].Clear();
                //过了这天就把所有要做的事重置
            }
        }
        */
    }
    // Update is called once per frame
    void Update()
    {
        
    }

}

public class MissionComponent
{
    public string Agent;
    public int Day;
    public int version;
    public MissionDetails[] plan;
}

public class MissionDetails
{
    public string time;
    public string environment;
    public string action;
    public int session;
}
