using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;


public class GetRunLog : MonoBehaviour
{
    private const string SERVER_URL = "http://127.0.0.1:5000";
    private void Start() {
        SendRequest();
    }
    

    public void SendRequest()
    {
        
        StartCoroutine(SubmitTask());

    }

    private IEnumerator SubmitTask()
    {
        string Url = "http://localhost:5000/runlog";
        string taskId = System.Guid.NewGuid().ToString();
        Debug.Log(taskId);

        // 创建JSON数据
        string jsonData = "{\"id\": \"" + taskId + "\"}";
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
            Debug.Log("runlog get successfully");
            Debug.Log(request.downloadHandler.text);
        }
    }

}
