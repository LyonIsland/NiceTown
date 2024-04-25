using Assets.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using DialogueFrame;

public class ReceiveData : MonoBehaviour
{
    private string UrlForGet = "http://127.0.0.1:5002/api/get";
    public DialogueFrame.DialogueUI DialogueUI;
    private Data ReceivedData;

    private void Start()
    {
        StartCoroutine(ReceiveDataFromServer());
        //DialogueUI.SetSubmitButtonAction(SubmitData);
    }

    IEnumerator ReceiveDataFromServer()
    {
        using(UnityWebRequest GetRequest = UnityWebRequest.Get(UrlForGet))
        {
            yield return GetRequest.SendWebRequest();
            if (GetRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error : " + GetRequest.error);
                DialogueUI.SetDisplayText("Failed to load Data!", false);
            }
            else
            {
                Debug.Log("Received Data: " + GetRequest.downloadHandler.text);
                string JsonResponse = GetRequest.downloadHandler.text;
                ReceivedData = JsonUtility.FromJson<Data>(JsonResponse);
                DialogueUI.SetDisplayText("Already read data!", false);
            }
        }
    }

    //public void SubmitData()
    //{
    //    string UserInput = DialogueUI.GetUserInput();
    //    Debug.Log("user inputs : " + UserInput);
    //    if (!string.IsNullOrEmpty(UserInput))
    //    {
    //        GetComponent<SendData>().DataSender(UserInput);
    //    }
    //}
}
