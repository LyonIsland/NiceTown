using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;


public class TestCommunicate : MonoBehaviour
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
        string Url = "http://localhost:5000/submit_task";
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
            Debug.Log("Task submitted successfully");
            // 获取任务ID
            //string taskID = requestData["id"];
            // 开始检查结果
            //StartCoroutine(CheckResult(taskID));
        }
    }


    private IEnumerator CheckResult(string taskID)
    {
        // 构建带参数的GET请求

        UnityWebRequest request = UnityWebRequest.Get(SERVER_URL + "/check_result?id=" + taskID);
        Debug.Log("send check");
        yield return request.SendWebRequest();
        Debug.Log("get check");
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error checking result: " + request.error);
        }
        else
        {
            // 解析并处理结果
            string result = request.downloadHandler.text;
            Debug.Log("Result: " + result);
        }
    }
}
