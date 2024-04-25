

namespace Assets.Utilities
{
    [System.Serializable]
    public class DataToReceive
    {
        public string status;
        public string message;
    }

    [System.Serializable]
    public class Data 
    {
        public string Uid { get; set; }
        public string context { get; set; }
    }
    [System.Serializable]
    public class ChatToSend
    {
        public string user_id;
        public string user_name;
        public string agent_name;
        public string day;
        public string time;
        public string type;
    }
    [System.Serializable]
    public class ChatToReceive
    {
        public string session_id;//对话id
        public string content;//agent的第一句话
    }
    [System.Serializable]
    //接收和发送用的是同一个数据结构
    public class Utterance
    {
        public string session_id;
        public string content;
        public string type;
        public string turn;
    }
    [System.Serializable]
    //发送他代表结束
    public class EndToSend
    {
        public string session_id;
        public string type;
    }
}
