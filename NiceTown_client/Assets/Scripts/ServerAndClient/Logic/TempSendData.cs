using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;

public class TempSendData : MonoBehaviour
{
    public DialogueFrame.DialogueUI DialogueUI;
    public int session_id = 0;
    private const string SERVER_URL = "http://127.0.0.1:5000";
    string userId = "testUesrId";
    string userName = "testUesrName";
    string agentName = "Benjamin";
    string day = "1";
    string time = "14:00";

    public string currentSessionId;
    // Start is called before the first frame update
    void Start()
    {
        DialogueUI.SetSubmitButtonAction(Create);
    }

    IEnumerator Create(){
        string Url = SERVER_URL+"/chat/create";  
        string jsonData = "{\"user_id\": \"" + userId + "\", \"user_name\": \"" + userName + "\", \"agent_name\": \"" + agentName + "\", \"day\": \"" + day + "\", \"time\": \"" + time + "\"}";
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);

        UnityWebRequest request = UnityWebRequest.Put(Url, jsonToSend);
        request.method = "POST";
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        string result = request.downloadHandler.text;
        JObject jsonObject = JObject.Parse(result);
        // Access the value of session_id
        session_id = (int)jsonObject["session_id"];
    }
    IEnumerator Utterance(){
        if (session_id != 0){

            //yield return request.SendWebRequest();
        }
        else{
            yield return "session not create";
        }
    }    
    
}
