using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GetDialogues;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class DisplayDialogue : MonoBehaviour
{
    [Header("播放对话相关")]
    //delayBetweenCharacters 每个字输出的速度
    public float delayBetweenCharacters = 0.1f;
    //delayAfterSentence 输出完一句完整的话的显示时间
    public float delayAfterSentence = 1.0f;
    //textData用于暂存这一波要输出的所有数据
    public DialogueData textData=new DialogueData();
    //当两个NPC距离<=这个值则开始播放
    public float triggerDistance = 5.0f;
    //记录总共的播放时间
    public float totalDialogueTime = 0f;

    [HeaderAttribute("ui组件")]
    public Image FaceImage;//头像
    public TextMeshProUGUI DialogueText;//展示出来的内容
    public GameObject frame;//头像+展示文本的组合体


    private GameObject NPC_1, NPC_2;
    
    public void Initialize(int session)
    {
        textData = Dialogue.MatchDialogues[session];
        //根据Agent名找到对应的gameObject
        NPC_1 = GameObject.Find(textData.Subject);
        NPC_2 = GameObject.Find(textData.Object);
    }
    //根据两npc距离判断是否可以展开动画的内容
    public bool AllowToDisplay()
    {
        if (NPC_1.activeInHierarchy && NPC_2.activeInHierarchy)
        {
            float distance = Vector3.Distance(NPC_1.transform.position, NPC_1.transform.position);
            if (distance < triggerDistance)
            {
                Debug.Log("允许展开对话");
                return true;
            }
        }
        return false;
    }
    //时时检查，如果满足距离要求、场景要求，则开始进行播放
    private void Update()
    {
        if (AllowToDisplay())
        {
            Debug.Log("准备播放对话内容");
            DisplayTextContent();
        }
    }
    public void Awake()
    {

    }
    
    //TypeSentence 展示对话内容
    IEnumerator TypeSentence(string sentence)
    {
        Debug.Log("开始打印一句话");
        float startTime = Time.time;
        DialogueText.text = "";
        foreach (char character in sentence)
        {
            DialogueText.text += character;
            yield return new WaitForSeconds(delayBetweenCharacters);
        }
        //更新对话的总时间
        totalDialogueTime += Time.time - startTime + delayAfterSentence;
    }
    //往对话框里写上内容
    IEnumerator TypeDialogue()
    {
        Debug.Log("writing dialogues");
        foreach(var dialogues in textData.chat)
        {
            //如果对话时间超了直接结束这部分
            if (totalDialogueTime > textData.duration)
                break;
            string sentence = dialogues.dialog;
            yield return StartCoroutine(TypeSentence(sentence));
            yield return new WaitForSeconds(delayAfterSentence);

        }
        Debug.Log("finish writing");
    }
    //开始输出对话内容时将其设置为可见
    //输出结束设置为不可见
    public void DisplayTextContent()
    {
        frame.SetActive(true);
        StartCoroutine(TypeDialogue());
        frame.SetActive(false);
    }
}
