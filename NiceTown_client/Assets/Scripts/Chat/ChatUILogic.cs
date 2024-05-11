using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatUILogic : MonoBehaviour
{
    public GameObject btn_startChat;
    public GameObject area_statChatBtnShow;
    public GameObject chatUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void BtnShowChatCkick()
    {
      Debug.Log("!!!");
       btn_startChat.SetActive(true);
    }
    public void BtnBackLayerClick()
    {
      //TODO: 查找所有NPC下的面板，并关闭 Find的过程
       btn_startChat.SetActive(false);
    }
    public void BtnStartChatClick()
    {
      Debug.Log("!!");
       chatUI.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
