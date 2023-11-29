using Cinemachine;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{
    //TODO: ÇÐ»»³¡¾°µ÷ÓÃ
    private void OnEnable()
    {
        EventHandler.AfterSceneloadedEvent += SwitchConfinerShape;
    }

    private void Start()
    {
        SwitchConfinerShape();
    }
    private void SwitchConfinerShape()
    {
        print(GameObject.FindGameObjectWithTag("BoundsConfiner"));
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();
        CinemachineConfiner confiner = GetComponent <CinemachineConfiner> ();
        confiner.m_BoundingShape2D = confinerShape;

        confiner.InvalidatePathCache();
    }
}
