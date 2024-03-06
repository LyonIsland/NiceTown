using System;
using UnityEngine;

[Serializable]
public class ScheduleDetails: IComparable<ScheduleDetails>
{
    public int hour, minute, day;
    public int priority;    //优先级越小优先执行
    [HideInInspector]
    public Season season;
    public string targetScene;
    public Vector2Int targetGridPosition;
    public AnimationClip clipAtStop;
    public bool interactable;
    //从这儿开始是后加的
    public DialogueData dialogueData=new DialogueData();

    // priority, season, clipatstop, interactable 不是传入的东西，先不管
    public ScheduleDetails(int hour, int minute, int day, int priority, Season season, string targetScene, Vector2Int targetGridPosition, AnimationClip clipAtStop, bool interactable, DialogueData dialogue = null)
    {
        this.hour = hour;
        this.minute = minute;
        this.day = day;
        this.priority = priority;
        this.season = season;
        this.targetScene = targetScene;
        this.targetGridPosition = targetGridPosition;
        this.clipAtStop = clipAtStop;
        this.interactable = interactable;
        this.dialogueData = dialogue;
    }

    public int Time => (hour * 100) + minute;

    public int CompareTo(ScheduleDetails other)
    {
        if (Time == other.Time)
        {
            if (priority > other.priority)
                return 1;
            else
                return -1;
        }
        else if (Time > other.Time)
        {
            return 1;
        }
        else if (Time < other.Time)
        {
            return -1;
        }

        return 0;
    }
}