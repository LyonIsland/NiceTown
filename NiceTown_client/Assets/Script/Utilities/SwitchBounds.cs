using Cinemachine;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{
    //TODO: 切换场景调用
    private void OnEnable()
    {
        EventHandler.TransitionEvent += event2;
    }

    private void event2(string arg1, Vector3 arg2)
    {
        print("注册的第二个事件");
    }


    private void Start()
    {
        SwitchConfinerShape();
    }
    private void SwitchConfinerShape()
    {
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();
        CinemachineConfiner confiner = GetComponent <CinemachineConfiner> ();
        confiner.m_BoundingShape2D = confinerShape;

        confiner.InvalidatePathCache();
    }
}
