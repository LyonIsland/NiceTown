using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;



namespace GetDialogues
{
    [System.Serializable]
    public class Dialogue : MonoBehaviour
    {
        //根据session找对话信息
        public static Dictionary<int, DialogueData> MatchDialogues = new Dictionary<int, DialogueData>();
        [HeaderAttribute("文本文件")]
        public TextAsset DialogueText;//文本文件

        public int idx = 0;
        public DialogueComponent[] DC;
        public static DialogueData dialogueData = new DialogueData();

        public void GetTextFromFile(TextAsset file)
        {
            //textList.Clear();
            idx = 0;
            dialogueData = JsonConvert.DeserializeObject<DialogueData>(file.text);
            Debug.Log(dialogueData.duration);

            MatchDialogues[dialogueData.session_id] = dialogueData;

            
        }
        
        void Start()
        {
            GetTextFromFile(DialogueText);
            Debug.Log("read file finished");
            idx = 0;
            Debug.Log("initialized");
        }
    }

}