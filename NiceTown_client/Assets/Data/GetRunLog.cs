using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;
using Unity.Burst.Intrinsics;
[ExecuteInEditMode]
public class GetRunLog : MonoBehaviour
{
    private const string SERVER_URL = "http://127.0.0.1:5000";
    private string fileName = "Data/Runlog/"; // 文件名

    #if UNITY_EDITOR
        private void OnDisable(){
            GetAllAgentRunlog();
        }
        
        public void GetAllAgentRunlog()
        {   
            List<NPCPosition> NPCPositionList = transform.GetComponent<NPCManager>().npcPositionList;
            foreach (NPCPosition NPCPosition in NPCPositionList){
                Transform npc = NPCPosition.npc;
                StartCoroutine(GetRunLogRequest(npc.name));
            }  
            
        }
        
        private IEnumerator GetRunLogRequest(string agent_name)
        {
            string Url = "http://localhost:5000/runlog";
            string taskId = System.Guid.NewGuid().ToString();
            // 创建JSON数据
            string jsonData = "{\"agent_name\": \"" + agent_name + "\"}";
            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);

            UnityWebRequest request = UnityWebRequest.Put(Url, jsonToSend);
            request.method = "POST";
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error submitting task: " + request.error);
            }
            else
            {
                string logString = request.downloadHandler.text;
                fileName = "Data/Runlog/";
                fileName = fileName+agent_name+".json";
                string filePath = Path.Combine(Application.dataPath, fileName);
                
                File.WriteAllText(filePath, logString);
                Debug.Log("JSON data saved to: " + filePath);
            }
        }
    #endif
}
