using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

namespace DialogueFrame
{
    public class DialogueUI : MonoBehaviour
    {
        public Text DisplayText;
        public InputField UserInput;
        public Button SubmitButton;
        public Button ExitButton;

        void Start()
        {
            SetExitButtonAction();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                SubmitButton.onClick.Invoke();
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                ExitButton.onClick.Invoke();
            }
        }

        public void SetDisplayText(string text, bool IsUser)
        {
            string color = IsUser ? "green" : "white"; // 如果role为true，使用绿色，否则使用白色                                              
            DisplayText.text += "\n<color=" + color + ">" + text + "</color>";// 使用富文本格式添加带颜色的文本
        }
        public string GetUserInput()
        {
            return UserInput.text;
        }
        public void SetSubmitButtonAction(Func<IEnumerator> ExecuteDialogue)
        {
            SubmitButton.onClick.RemoveAllListeners();  // 清除现有监听器，以避免重复设置
            SubmitButton.onClick.AddListener(() => StartCoroutine(ExecuteDialogue()));
        }
        //将UI界面设置为不可见
        public void SetInvisible()
        {
            gameObject.SetActive(false);
        }


        public void SetVisible()
        {
            gameObject.SetActive(true);
        }

        public void SetExitButtonAction()
        {
            // 清除所有已有的监听器
            ExitButton.onClick.RemoveAllListeners();

            // 添加新的监听器
            ExitButton.onClick.AddListener(GetComponent<SendData>().TerminateDialogue);
            ExitButton.onClick.AddListener(SetInvisible);

            // 添加触发 SetExitButtonAction 的监听器
            ExitButton.onClick.AddListener(() => SetExitButtonAction());
        }
    }
}