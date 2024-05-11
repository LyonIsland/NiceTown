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
    public string agent_name = "empty";
    private const string SERVER_URL = "http://127.0.0.1:5000";
    private string fileName = "Data/"; // 文件名

    #if UNITY_EDITOR
        private void OnDisable(){
            SendRequest();
        }
        
        public void SendRequest()
        {    
            StartCoroutine(GetRunLogRequest());
        }
        
        private IEnumerator GetRunLogRequest()
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
                fileName = fileName+agent_name+".json";
                string filePath = Path.Combine(Application.dataPath, fileName);
                
                File.WriteAllText(filePath, logString);
                Debug.Log("JSON data saved to: " + filePath);
            }
        }
    #endif
}
