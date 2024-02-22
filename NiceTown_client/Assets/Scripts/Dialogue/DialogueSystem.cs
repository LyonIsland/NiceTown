using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;



namespace DialogueFrameSystem
{
    [System.Serializable]
    public class DialogueFrame : MonoBehaviour
    {
        [HeaderAttribute("ui组件")]
        public Image FaceImage;//头像
        public TextMeshProUGUI DialogueMessage;//展示出来的内容

        [HeaderAttribute("文本文件")]
        public TextAsset DialogueText;//文本文件

        public int idx = 0;
        public DialogueComponent[] DC;
        List<string> textList = new List<string>();
        public void GetTextFromFile(TextAsset file)
        {
            textList.Clear();
            idx = 0;
            var dialogueData = JsonConvert.DeserializeObject<DialogueData>(file.text);
            Debug.Log(dialogueData.duration);
            foreach (var line in dialogueData.chat)
            {
                Debug.Log($"{line.turn}, {line.character}, {line.dialog}");
                textList.Add(line.dialog);
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            GetTextFromFile(DialogueText);
            Debug.Log("read file finished");
            idx = 0;
            Debug.Log("initialized");
        }

        // Update is called once per frame
        void FetchTextFromSession(int code)
        {

        }
        void Update()
        {

            if (Input.GetKeyDown(KeyCode.R) && idx == textList.Count)
            {
                //如果已经没词了，那就关闭对话框
                Debug.Log("Dialogue window shut down");
                gameObject.SetActive(false);
                return;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("Updating");
                //按下R键开始输出
                DialogueMessage.text = textList[idx++];
                //按下标开始输出信息
            }

        }
    }

    public class DialogueComponent
    {
        //接收到的信息包括turn character dialogue
        public int turn;
        public string character;
        public string dialog;

    }

    public class DialogueData
    {
        public int session_id;
        public string Subject;
        public string Object;
        public int duration;
        public DialogueComponent[] chat;
    }
}