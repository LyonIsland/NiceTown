using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;
using Unity.Burst.Intrinsics;
using Newtonsoft.Json;
using UnityEngine.UIElements;

public class SetMission : MonoBehaviour
{
    private const string fileName = "Data/runlog.json"; // 文件名
    private MyDataObject[][] runLog = new MyDataObject[1][];

    private void Start()
    {
        //执行从本地文件loadmission的工作
        readLog();
        SetMissionSO();
    }

    private void readLog(){
        // 设置JSON文件的路径，这里假设JSON文件在Assets文件夹下
        string filePath = Path.Combine(Application.dataPath, fileName);
        string jsonContent = File.ReadAllText(filePath);

        // 使用Json.NET解析JSON字符串
        runLog = JsonConvert.DeserializeObject<MyDataObject[][]>(jsonContent);

        // 输出解析后的数据
        
    }

    private void SetMissionSO(){
        List<NPCPosition> NPCPositionList = transform.GetComponent<NPCManager>().npcPositionList;
        foreach (NPCPosition NPCPosition in NPCPositionList){
            Transform npc = NPCPosition.npc;
            List<ScheduleDetails> npcMissionList = npc.GetComponent<NPCMovement>().scheduleData.scheduleList; 
            npcMissionList.Clear();
            for (int day = 0; day < runLog.Length; day++)
            {
                for (int index = 0; index < runLog[day].Length; index++)
                {
                    int hour;
                    string hourString = runLog[day][index].time.Substring(0, 2);
                    if (hourString.StartsWith("0")){
                        hour = int.Parse(hourString.Substring(1, 1));
                    }
                    else{
                        hour = int.Parse(hourString.Substring(0, 2));
                    }
                    string minuteString = runLog[day][index].time.Substring(3, 2);
                    int minute = int.Parse(minuteString);
                    string place = runLog[day][index].place;
                    Debug.Log(place);
                    int placePosX = (int)GameObject .Find(place).transform.position.x;
                    int placePosY = (int)GameObject .Find(place).transform.position.y;
                    Vector2Int placePos =  new Vector2Int(placePosX,placePosY);
                    Debug.Log(placePos);
                    

                    npcMissionList.Add(new ScheduleDetails(hour, minute, day+1, 0, Season.春天, "testScene", placePos, null, false));
                }
            }
            
        }
        
    }
}

[System.Serializable]
public class MyDataObject
{
    public string place;
    public string time;
}


