using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandle : MonoBehaviour
{
    [SerializeField] private GameObject DialogueFrame;
    private string AgentName = string.Empty;
    public Button TriggerDialogueButton;
    public SendData SendDataScript = null;
    public Text UsernameText;

    void Start()
    {
        TriggerDialogueButton.onClick.AddListener(ToggleDialogueFrame);
        InitializeSize();
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.position = transform.parent.parent.position;
    }
    private void InitializeSize()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null && transform.parent.parent != null)
        {
            rectTransform.sizeDelta = new Vector2(60, 60);  // 设置大小为60px x 60px
            rectTransform.position = transform.parent.parent.position;  // 设置位置为parent的parent的中心位置
            rectTransform.localScale = new Vector2(0.09f, 0.09f);
            Debug.Log("Position and size of the GameObject have been set.");
        }
    }


    private void ToggleDialogueFrame()
    {
        if (DialogueFrame.activeSelf)
        {
            Initialization();  // 如果DialogueFrame被激活，执行初始化
        }
        else
        {
            RemoveListeners();  // 如果DialogueFrame被禁用，移除所有监听器
        }
    }


    private void RemoveListeners()
    {
        TriggerDialogueButton.onClick.RemoveAllListeners();
        Debug.Log("Listeners removed due to DialogueFrame being set inactive.");
    }

    public void Initialization()
    {
        if (DialogueFrame != null && SendDataScript == null)
        {
            SendDataScript = DialogueFrame.GetComponent<SendData>();
        }
        if (SendDataScript != null)
        {
            GetParentName(TriggerDialogueButton.gameObject);
            UsernameText = GameObject.Find("Title").GetComponent<Text>();
            if (UsernameText != null)
            {
                UsernameText.text = "Chat with " + AgentName;
                Debug.Log("User name text found");
            }
            else
                Debug.Log("User name text not found");
            SendDataScript.GetInitialReply();
            Debug.Log("Dialogue Frame initialized and listeners bound.");
        }
        else
        {
            Debug.Log("SendData component not found on DialogueFrame.");
        }
    }


    private void GetParentName(GameObject button)
    {
        AgentName = transform.parent.parent.name;
        Debug.Log("Parent GameObject name: " + AgentName);
        if (DialogueFrame != null)
            SendDataScript.AgentName = AgentName;
    }
}


