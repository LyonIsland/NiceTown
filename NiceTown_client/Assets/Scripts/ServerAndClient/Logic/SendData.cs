using Assets.Utilities;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using DialogueFrame;
using System;


public class SendData : MonoBehaviour
{
    public DialogueFrame.DialogueUI DialogueUI;
    private string ServerIPAndPort= "http://127.0.0.1:5000/submit_task";
    private string ApiMethod = string.Empty;
    private string SendPostRequest = string.Empty;
    public ChatType ChatType;
    public ApiCollection ApiCollection;
    public string UserName = "Lucas";//怎么获取待定
    public string AgentName = "Sophia";
    public string session_id = string.Empty;
    public string AgentReply = string.Empty;
    public string UserInput = string.Empty;
    public string UserId = string.Empty;
    UnityWebRequest PostRequest;//用于处理post方式的
    UnityWebRequest GetRequest;
    public bool InitialReplySent = false;//初始请求是否发送,true代表应该发送
    public bool InitialReplyGot = false;//是否处理完成Start部分
    public bool DialogueFinished = false;//对话是否结束，true为结束，false为未结束
    public int turn = 1;

    //各种临时变量
    private ChatToSend ChatToSend = new ChatToSend();
    private ChatToReceive ChatToReceive = new ChatToReceive();
    private Utterance UtteranceToSend = new Utterance();
    private Utterance UtteranceToReceive = new Utterance();
    private EndToSend EndToSend = new EndToSend();

    /// <summary>
    /// 用于生成用户唯一ID
    /// </summary>
    /// <returns></returns>
    private string GenerateUid()
    {
        string Uid = Guid.NewGuid().ToString();
        return Uid;
    }
    /// <summary>
    /// 用于获取当前游戏时间
    /// </summary>
    /// <returns></returns>
    private string GetCurrentTime()
    {
        //这里要改
        string CurrentTime = DateTime.Now.ToString();
        return CurrentTime;
    }
    /// <summary>
    /// 用于获取当前日期
    /// </summary>
    /// <returns></returns>
    private string GetCurrentDay()
    {
        //这里要改
        string CurrentDay = "1";
        return CurrentDay;
    }
    /// <summary>
    /// 调用/start时，用于生成发送给服务器端的数据结构
    /// </summary>
    /// <returns></returns>
    private ChatToSend GenerateChatToSend()
    {
        ChatToSend.user_id = GenerateUid();
        UserId = ChatToSend.user_id;
        ChatToSend.type = ChatType.Start.ToString();
        ChatToSend.time = GetCurrentTime();
        ChatToSend.day = GetCurrentDay();
        ChatToSend.user_name = UserName;
        return ChatToSend;
    }
    /// <summary>
    /// 调用/end时，用于生成发送给服务器端的数据结构
    /// </summary>
    /// <returns></returns>
    private EndToSend GenerateEnd()
    {
        EndToSend.type = ChatType.End.ToString();
        EndToSend.session_id = session_id;
        return EndToSend;
    }
    /// <summary>
    /// /start进行api调用时的主体代码
    /// </summary>
    public void GetInitialReply()
    {
        Debug.Log("getting initial reply");
        ChatToSend = GenerateChatToSend();
        ApiMethod = ApiCollection.Start.ToString();
        Debug.Log(ApiMethod);
        SendPostRequest = ServerIPAndPort + ApiMethod;
        Debug.Log(SendPostRequest);
        StartCoroutine(SendDataToServer(SendPostRequest, ChatToSend, "Start"));
        StopCoroutine(SendDataToServer(SendPostRequest, ChatToSend, "Start"));
    }
    /// <summary>
    /// 对/Utterance进行api调用时，生成传递给服务器的数据结构
    /// </summary>
    /// <param name="Input">从输入区获得的用户输入</param>
    /// <returns></returns>
    private Utterance GenerateUtteranceSender(string Input)
    {
        UtteranceToSend.type = ChatType.Utterance.ToString();
        UtteranceToSend.turn = (turn + 1).ToString();
        UtteranceToSend.session_id = session_id;
        UtteranceToSend.content = Input;
        return UtteranceToSend;
    }
    /// <summary>
    /// submit和exit添加事件监听
    /// </summary>
    private void Start()
    {
        DialogueUI.SetSubmitButtonAction(ExecuteNormalDialogue);
    }

    /// <summary>
    /// 终止对话的具体逻辑
    /// </summary>
    public void TerminateDialogue()
    {
        ApiMethod = ApiCollection.End.ToString();
        string UrlForEnd = ServerIPAndPort + ApiMethod;
        Debug.Log("Url for end is : " + UrlForEnd);
        EndToSend = GenerateEnd();
        StartCoroutine(SendDataToServer(UrlForEnd, EndToSend, "End"));
        ReInitializeStatus();
    }
    /// <summary>
    /// 终止对话后将部分状态变量重置
    /// </summary>
    private void ReInitializeStatus()
    {
        Debug.Log("reinitializing status");
        InitialReplyGot = false;
        InitialReplySent = false;
        DialogueFinished = false;
        session_id = string.Empty;
        turn = 1;
    }
    /// <summary>
    /// 实现一句一句对话的协程操作
    /// </summary>
    /// <returns></returns>
    IEnumerator ExecuteNormalDialogue()
    {
        Debug.Log("1");
        yield return new WaitUntil(() => InitialReplyGot);

        string UserInput = DialogueUI.GetUserInput();
        Debug.Log("user inputs : " + UserInput);
        if (!string.IsNullOrEmpty(UserInput))
        {
            SetReplyOnScreen(UserInput, UserName);
            UtteranceDataSender(UserInput);
            Debug.Log("utterance data is sent");
            DialogueUI.UserInput.text = string.Empty;//输出结束后进行清空
        }
    }
    /// <summary>
    /// 针对Utterance的数据发送逻辑
    /// </summary>
    /// <param name="UserData"></param>
    public void UtteranceDataSender(string UserData)
    {
        UtteranceToSend = GenerateUtteranceSender(UserData);
        ApiMethod = ApiCollection.Utterance.ToString();
        string UrlForUtterance = ServerIPAndPort + ApiMethod;
        Debug.Log(UrlForUtterance);
        StartCoroutine(SendDataToServer(UrlForUtterance, UtteranceToSend, "Utterance"));

    }
    /// <summary>
    /// 泛型方法，向后端发送http请求
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="UrlForPost"></param>
    /// <param name="data"></param>
    /// <param name="Status"></param>
    /// <returns></returns>
    IEnumerator SendDataToServer<T1>(string UrlForPost,T1 data,string Status)
    {
        Debug.Log("executing send data to server");
        PostRequest = new UnityWebRequest(UrlForPost, "POST");
        //数据封装为Json格式，并使用UTF-8编码
        string ToJson = JsonUtility.ToJson(data);
        byte[] BodyRaw = Encoding.UTF8.GetBytes(ToJson);

        PostRequest.uploadHandler = new UploadHandlerRaw(BodyRaw);
        PostRequest.downloadHandler = new DownloadHandlerBuffer();
        PostRequest.SetRequestHeader("Content-Type", "application/json");
        Debug.Log("Url for post is : " + UrlForPost);
        //发送网络请求并等待返回
        yield return PostRequest.SendWebRequest();

        if (PostRequest.result != UnityWebRequest.Result.Success)
            Debug.LogError("Error:" + PostRequest.error);

        else
        {
            StartCoroutine(CheckResult(UserId));
            //获取网站返回的
            Debug.Log("Data already sent");
            if (Status == "Start")
                HandleStartRequest();//处理异步调用的部分
            else if (Status == "Utterance")
                HandleUtteranceRequest();
            else if(Status =="End")
                ReInitializeStatus();
        }
        
    }
    /// <summary>
    /// 用于处理/Utterance的api返回数据
    /// </summary>
    private void HandleUtteranceRequest()
    {
        UtteranceToReceive = ReceiveFromServer<Utterance>(PostRequest.downloadHandler.text);
        if (UtteranceToReceive.content != null)
        {
            Debug.Log("utterance reply is " + UtteranceToReceive.content);
            SetReplyOnScreen(UtteranceToReceive.content, AgentName);
            turn = int.Parse(UtteranceToReceive.turn);
        }

        else
            Debug.LogError("Utterance reply is null");
    }

    private IEnumerator CheckResult(string TaskID)
    {
        GetRequest = UnityWebRequest.Get(ServerIPAndPort + "/check_result?id=" + TaskID);
        Debug.Log("send check");
        yield return GetRequest.SendWebRequest();
        if(GetRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("error checking result:" + GetRequest.error);
        }
    }
    /// <summary>
    /// 用于处理/Start的api返回数据
    /// </summary>
    private void HandleStartRequest()
    {
        ChatToReceive = ReceiveFromServer<ChatToReceive>(GetRequest.downloadHandler.text);
        Debug.Log("Chat received is : " + ChatToReceive.content.ToString());
        InitialReplyGot = true;

        if (ChatToReceive.content != null)
        {
            SetReplyOnScreen(ChatToReceive.content, AgentName);
            session_id = ChatToReceive.session_id;
            Debug.Log(ChatToReceive.content);
        }
        else
            Debug.LogError("ChatToReceive's content is null");
    }
    /// <summary>
    /// 将agent的话输出到UI
    /// </summary>
    /// <param name="reply"></param>
    private void SetReplyOnScreen(string reply,string role)
    {
        string sentence = role + " : " + reply;
        bool isUser = role == UserName ? true : false;
        DialogueUI.SetDisplayText(sentence, isUser);
    }
    /// <summary>
    /// 泛型方法，将服务器返回的数据进行解析
    /// </summary>
    /// <typeparam name="T2"></typeparam>
    /// <param name="response"></param>
    /// <returns></returns>
    private T2 ReceiveFromServer<T2>(string response)
    {
        T2 receive = JsonUtility.FromJson<T2>(response);
        return receive;
    }


}
