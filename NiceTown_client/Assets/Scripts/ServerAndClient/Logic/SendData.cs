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
    public string UserName = "Lucas";//��ô��ȡ����
    public string AgentName = "Sophia";
    public string session_id = string.Empty;
    public string AgentReply = string.Empty;
    public string UserInput = string.Empty;
    public string UserId = string.Empty;
    UnityWebRequest PostRequest;//���ڴ���post��ʽ��
    UnityWebRequest GetRequest;
    public bool InitialReplySent = false;//��ʼ�����Ƿ���,true����Ӧ�÷���
    public bool InitialReplyGot = false;//�Ƿ������Start����
    public bool DialogueFinished = false;//�Ի��Ƿ������trueΪ������falseΪδ����
    public int turn = 1;

    //������ʱ����
    private ChatToSend ChatToSend = new ChatToSend();
    private ChatToReceive ChatToReceive = new ChatToReceive();
    private Utterance UtteranceToSend = new Utterance();
    private Utterance UtteranceToReceive = new Utterance();
    private EndToSend EndToSend = new EndToSend();

    /// <summary>
    /// ���������û�ΨһID
    /// </summary>
    /// <returns></returns>
    private string GenerateUid()
    {
        string Uid = Guid.NewGuid().ToString();
        return Uid;
    }
    /// <summary>
    /// ���ڻ�ȡ��ǰ��Ϸʱ��
    /// </summary>
    /// <returns></returns>
    private string GetCurrentTime()
    {
        //����Ҫ��
        string CurrentTime = DateTime.Now.ToString();
        return CurrentTime;
    }
    /// <summary>
    /// ���ڻ�ȡ��ǰ����
    /// </summary>
    /// <returns></returns>
    private string GetCurrentDay()
    {
        //����Ҫ��
        string CurrentDay = "1";
        return CurrentDay;
    }
    /// <summary>
    /// ����/startʱ���������ɷ��͸��������˵����ݽṹ
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
    /// ����/endʱ���������ɷ��͸��������˵����ݽṹ
    /// </summary>
    /// <returns></returns>
    private EndToSend GenerateEnd()
    {
        EndToSend.type = ChatType.End.ToString();
        EndToSend.session_id = session_id;
        return EndToSend;
    }
    /// <summary>
    /// /start����api����ʱ���������
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
    /// ��/Utterance����api����ʱ�����ɴ��ݸ������������ݽṹ
    /// </summary>
    /// <param name="Input">����������õ��û�����</param>
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
    /// submit��exit����¼�����
    /// </summary>
    private void Start()
    {
        DialogueUI.SetSubmitButtonAction(ExecuteNormalDialogue);
    }

    /// <summary>
    /// ��ֹ�Ի��ľ����߼�
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
    /// ��ֹ�Ի��󽫲���״̬��������
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
    /// ʵ��һ��һ��Ի���Э�̲���
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
            DialogueUI.UserInput.text = string.Empty;//���������������
        }
    }
    /// <summary>
    /// ���Utterance�����ݷ����߼�
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
    /// ���ͷ��������˷���http����
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
        //���ݷ�װΪJson��ʽ����ʹ��UTF-8����
        string ToJson = JsonUtility.ToJson(data);
        byte[] BodyRaw = Encoding.UTF8.GetBytes(ToJson);

        PostRequest.uploadHandler = new UploadHandlerRaw(BodyRaw);
        PostRequest.downloadHandler = new DownloadHandlerBuffer();
        PostRequest.SetRequestHeader("Content-Type", "application/json");
        Debug.Log("Url for post is : " + UrlForPost);
        //�����������󲢵ȴ�����
        yield return PostRequest.SendWebRequest();

        if (PostRequest.result != UnityWebRequest.Result.Success)
            Debug.LogError("Error:" + PostRequest.error);

        else
        {
            StartCoroutine(CheckResult(UserId));
            //��ȡ��վ���ص�
            Debug.Log("Data already sent");
            if (Status == "Start")
                HandleStartRequest();//�����첽���õĲ���
            else if (Status == "Utterance")
                HandleUtteranceRequest();
            else if(Status =="End")
                ReInitializeStatus();
        }
        
    }
    /// <summary>
    /// ���ڴ���/Utterance��api��������
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
    /// ���ڴ���/Start��api��������
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
    /// ��agent�Ļ������UI
    /// </summary>
    /// <param name="reply"></param>
    private void SetReplyOnScreen(string reply,string role)
    {
        string sentence = role + " : " + reply;
        bool isUser = role == UserName ? true : false;
        DialogueUI.SetDisplayText(sentence, isUser);
    }
    /// <summary>
    /// ���ͷ����������������ص����ݽ��н���
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
