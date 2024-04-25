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
            string color = IsUser ? "green" : "white"; // ���roleΪtrue��ʹ����ɫ������ʹ�ð�ɫ                                              
            DisplayText.text += "\n<color=" + color + ">" + text + "</color>";// ʹ�ø��ı���ʽ��Ӵ���ɫ���ı�
        }
        public string GetUserInput()
        {
            return UserInput.text;
        }
        public void SetSubmitButtonAction(Func<IEnumerator> ExecuteDialogue)
        {
            SubmitButton.onClick.RemoveAllListeners();  // ������м��������Ա����ظ�����
            SubmitButton.onClick.AddListener(() => StartCoroutine(ExecuteDialogue()));
        }
        //��UI��������Ϊ���ɼ�
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
            // ����������еļ�����
            ExitButton.onClick.RemoveAllListeners();

            // ����µļ�����
            ExitButton.onClick.AddListener(GetComponent<SendData>().TerminateDialogue);
            ExitButton.onClick.AddListener(SetInvisible);

            // ��Ӵ��� SetExitButtonAction �ļ�����
            ExitButton.onClick.AddListener(() => SetExitButtonAction());
        }
    }
}