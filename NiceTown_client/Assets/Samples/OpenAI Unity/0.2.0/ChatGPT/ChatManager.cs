using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;

namespace OpenAI
{
    public class ChatManager : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private Button close_button;
        [SerializeField] private ScrollRect scroll;
        
        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        private float height;
        private List<ChatMessage> messages = new List<ChatMessage>();

        public int session_id = 0;
        public string task_id = "";
        public string latest_query = "";
        private const string SERVER_URL = "http://127.0.0.1:5000";
        string userId = "testUesrId";
        public string userName = "佳乐";
        public static string agentName = "Benjamin";
        string day = "1";
        string time = "14:00";


        private void Start()
        {
            button.onClick.AddListener(Chat);
            
        }
        public void CreateSession(){
            StartCoroutine(Create());
        }

        public void EndSession(){
            StartCoroutine(End());
        }

        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        void Chat(){
            StartCoroutine(Utterance());
        }
        public IEnumerator  Create(){
            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;
            string Url = SERVER_URL+"/chat/create";  
            string jsonData = "{\"user_id\": \"" + userId + "\", \"user_name\": \"" + userName + "\", \"agent_name\": \"" + agentName + "\", \"day\": \"" + day + "\", \"time\": \"" + time + "\"}";
            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);

            UnityWebRequest request = UnityWebRequest.Put(Url, jsonToSend);
            request.method = "POST";
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            button.enabled = true;
            inputField.text = "";
            inputField.enabled = true;
            string result = request.downloadHandler.text;
            JObject jsonObject = JObject.Parse(result);
            // Access the value of session_id
            session_id = (int)jsonObject["session_id"];
            Debug.Log("" + session_id);
        }

        private IEnumerator Utterance()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };
            AppendMessage(newMessage);      
            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;
            task_id = System.Guid.NewGuid().ToString();
            string Url = SERVER_URL+"/chat/utterance";   
            string jsonData = "{\"session_id\": \"" + session_id + "\", \"id\": \"" + task_id + "\", \"utterance\": \"" + newMessage.Content + "\", \"agent_name\": \"" + agentName + "\"}";

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
                StartCoroutine(Query(task_id));
            }

            
        }

        private IEnumerator Query(string taskID)
        {
            // 构建带参数的GET请求
            string Url = SERVER_URL+"/chat/query";   
            string jsonData = "{\"id\": \"" + task_id + "\"}";
            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
            UnityWebRequest request = UnityWebRequest.Put(Url, jsonToSend);
            request.method = "POST";
            request.SetRequestHeader("Content-Type", "application/json");
            
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
                JObject jsonObject = JObject.Parse(result);
                string queryContent = (string)jsonObject["query"];
                Debug.Log("Result: " + result);
                Debug.Log("content"+ queryContent);
                var query = new ChatMessage()
                {
                    Content = queryContent
                };
                AppendMessage(query);
                button.enabled = true;
                inputField.enabled = true;
            }
        }

        public IEnumerator End()
        {
            Debug.Log("!!!");
            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;
            string Url = SERVER_URL+"/chat/end";  
            string jsonData = "{\"agent_name\": \"" + agentName + "\", \"session_id\": \"" + session_id + "\"}";
            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);

            UnityWebRequest request = UnityWebRequest.Put(Url, jsonToSend);
            request.method = "POST";
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            string result = request.downloadHandler.text;
            Debug.Log("" + result);
        }
    } 
}
