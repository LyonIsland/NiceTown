using System.Collections;
using System.Collections.Generic;
using MFarm.AStar;
using System;
using MFarm.Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using GetDialogues;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class NPCMovement : MonoBehaviour, ISaveable
{
    public ScheduleDataList_SO scheduleData;
    private SortedSet<ScheduleDetails> scheduleSet;
    private ScheduleDetails currentSchedule;

    //临时存储信息
    public string currentScene;
    private string targetScene;
    //public string ID;//
    //public int day;
    
    //public List<MissionDetails> missionDetails;//
    //public MissionComponent DayWork;
    private Vector3Int currentGridPosition;
    private Vector3Int tragetGridPosition;
    private Vector3Int nextGridPosition;
    private Vector3 nextWorldPosition;

    public string StartScene { set => currentScene = value; }

    [Header("移动属性")]
    public float normalSpeed = 2f;
    private float minSpeed = 1;
    private float maxSpeed = 3;
    private Vector2 dir;
    public bool isMoving;

    //Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;
    private Animator anim;
    private Grid gird;
    private Stack<MovementStep> movementSteps;
    private Coroutine npcMoveRoutine;

    private bool isInitialised;
    private bool npcMove;
    private bool sceneLoaded;
    public bool interactable;
    public bool isFirstLoad;
    private Season currentSeason;
    //动画计时器
    private float animationBreakTime;
    private bool canPlayStopAnimaiton;
    private AnimationClip stopAnimationClip;
    public AnimationClip blankAnimationClip;
    private AnimatorOverrideController animOverride;

    private TimeSpan GameTime => TimeManager.Instance.GameTime;

    //从这儿开始后面的也是后加的

    //public string ID;//每个npc都有的独立ID
    //NPCPlan 类 存储这个NPC要做的东西
    public MissionComponent NPCPlan;//DayWork
    //NPCMissionDetails 列表 这个NPC要做的每件事的集合
    public List<MissionDetails> NPCMissionDetails;
    //DialogueComponent 堆 存储这个NPC要执行的对话内容（如果有的话）
    public Stack<DialogueData> DialogueComponent = new Stack<DialogueData>();
    //判定是不是要进行对话
    public bool DialogueRelated = false;

    public string GUID => GetComponent<DataGUID>().guid;


    public void TransferInputDataToSchedule_SOFormat()
    {
        Debug.Log("transferring");
        if (NPCManager.Instance == null)
            Debug.Log("-------");
        NPCPlan = NPCManager.Instance.MatchNPCbyID[rb.name];
        //这里不确定这个方法能不能得到这个npc在inspector的名字是什么
        Debug.Log(rb.name);
        //Debug.Log(DayWork.plan[1].environment + "*******");
        //设置新的一天npc移动逻辑时要把前一天的覆盖掉
        scheduleData.scheduleList.Clear();
        foreach (var details in NPCManager.Instance.ToDoDict[rb.name])
        {
            NPCMissionDetails.Add(details);
            //获取minute 和 hour
            string time = details.time;
            string[] parts = time.Split(':');//当心可能是中文冒号，代码里写的是英文冒号
            int hour = int.Parse(parts[0]);
            int minute = int.Parse(parts[1]);
            int day = NPCPlan.Day;
            //这里要重新写一下
            string targetBuilding = details.building;
            StoreLocation Findplace = new StoreLocation();
            string targetScene = Findplace.MatchScenes[targetBuilding];
            //Vector2Int pos = Findplace.location[targetBuilding];
            Vector2Int targetPos = new Vector2Int(0, 0);
            bool FindTargetPos = false;
            foreach(var item in InteractItems_SO.interactableObjects)
            {
                if (item.ID == details.place)
                {
                    FindTargetPos = true;
                    targetPos = new Vector2Int((int)item.obj.transform.position.x, (int)item.obj.transform.position.y);
                    break;
                }
            }
            if (!FindTargetPos)
                Debug.Log("did not find ID : " + details.place + " in interactable list");
            //如果有对话就把对话相关的东西扔进去
            if (details.session != 0)
            {
                DialogueData dialogue = new DialogueData();
                dialogue = Dialogue.MatchDialogues[details.session];
                scheduleData.scheduleList.Add(new ScheduleDetails(hour, minute, day, 0, Season.春天, targetScene, targetPos, null, false, dialogue));
            }
            else
            {
                scheduleData.scheduleList.Add(new ScheduleDetails(hour, minute, day, 0, Season.春天, targetScene, targetPos, null, false));
            }
            //还没有测试数据，这段先不启用
            //string[] places = details.environment.Split('-');
            //string endpos = places[1];
            //foreach (var destiny in InteractiveObj.InteractObjectList)
            //{
            //    if (destiny.name == endpos && destiny.sceneName == targetScene)
            //    {
            //        pos = destiny.position;
            //        break;
            //    }
            //}
            //if (details.session != 0)
            //{
            //    try
            //    {
            //        DialogueComponent.Push(Dialogue.MatchDialogues[details.session]);
            //    }
            //    catch
            //    {
            //        Console.WriteLine("find dialogue component fail");
            //    }

            //}
            //scheduleData.scheduleList.Add(new ScheduleDetails(hour, minute, day, 0, Season.春天, targetScene, pos, null, false));
        }
        Debug.Log("transfer end");
    }

    private void Awake()
    {
        Debug.Log("NPCMovement Awake");
        if (NPCManager.Instance != null)
            Debug.Log(7);
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        movementSteps = new Stack<MovementStep>();
        NPCMissionDetails = new List<MissionDetails>();//新建这个npc的行为逻辑
        NPCPlan = new MissionComponent();
        animOverride = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = animOverride;
        scheduleSet = new SortedSet<ScheduleDetails>();
        Debug.Log("npc movement awake end");
    }
    
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    private void Start()
    {
        Debug.Log("NPC mpvement start");
        if (NPCManager.Instance == null)
            Debug.Log(8);
        //ISaveable saveable = this;
        //saveable.RegisterSaveable();
        //scheduleData.scheduleList.Add(new ScheduleDetails(7, 5, 0, 0, Season.春天, "01.Field", new Vector2Int(10,20), null, false));
        Debug.Log("schedule set activated");
        //Debug.Log(rb.name);
        if (NPCManager.Instance == null)
        {
            Debug.Log("fail to assign mission to every npcs");
        }
        TransferInputDataToSchedule_SOFormat();//把传入的数据转化成Schedule可以识别的东西
        foreach (var schedule in scheduleData.scheduleList)
        {
            scheduleSet.Add(schedule);
        }
        Debug.Log("npc movement start end");
    }

    private void Update()
    {
        if (sceneLoaded)
            SwitchAnimation();

        //计时器
        /*
        animationBreakTime -= Time.deltaTime;
        canPlayStopAnimaiton = animationBreakTime <= 0;
        */
    }

    private void FixedUpdate()
    {
        if (sceneLoaded)
            Movement();
    }
    private void OnEndGameEvent()
    {
        sceneLoaded = false;
        npcMove = false;
        if (npcMoveRoutine != null)
            StopCoroutine(npcMoveRoutine);
    }

    private void OnStartNewGameEvent(int obj)
    {
        //isInitialised = false;
        isFirstLoad = true;
    }

    private void OnGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        int time = (hour * 100) + minute;
        currentSeason = season;

        ScheduleDetails matchSchedule = null;
        foreach (var schedule in scheduleSet)
        {
            if (schedule.Time == time)
            {
                if (schedule.day != day && schedule.day != 0)
                    continue;
                if (schedule.season != season)
                    continue;
                matchSchedule = schedule;
            }
            else if (schedule.Time > time)
            {
                break;
            }
        }
        if (matchSchedule != null)
        {
            //如果涉及到对话内容，则读取并且将最终坐标更改为对话的另一个人的坐标
            if (matchSchedule.dialogueData != null && rb.name == matchSchedule.dialogueData.Subject)
            {
                Debug.Log("有对话" + matchSchedule.dialogueData.session_id.ToString());
                GameObject mid = GameObject.Find(matchSchedule.dialogueData.Object);
                Vector3 pos = new Vector3(0, 0, 0);
                if (rb.name == matchSchedule.dialogueData.Subject)
                    pos = GameObject.Find(matchSchedule.dialogueData.Object).transform.position;
                else
                    pos = GameObject.Find(matchSchedule.dialogueData.Subject).transform.position; ;
                //Vector3 pos = (GameObject.Find(matchSchedule.dialogueData.Object).transform.position);
                matchSchedule.targetGridPosition = new Vector2Int((int)pos.x, (int)pos.y);
            }
            BuildPath(matchSchedule);
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        sceneLoaded = false;
    }

    private void OnAfterSceneLoadedEvent()
    {
        gird = FindObjectOfType<Grid>();
        CheckVisiable();

        if (!isInitialised)
        {
            InitNPC();
            isInitialised = true;
        }

        sceneLoaded = true;

        if (!isFirstLoad)
        {
            currentGridPosition = gird.WorldToCell(transform.position);
            var schedule = new ScheduleDetails(0, 0, 0, 0, currentSeason, targetScene, (Vector2Int)tragetGridPosition, stopAnimationClip, interactable);
            BuildPath(schedule);
            isFirstLoad = true;
        }
    }

    private void CheckVisiable()
    {
        if (currentScene == SceneManager.GetActiveScene().name)
            SetActiveInScene();
        else
            SetInactiveInScene();
    }


    private void InitNPC()
    {
        Debug.Log("npc initialized");
        targetScene = currentScene;

        //保持在当前坐标的网格中心点
        currentGridPosition = gird.WorldToCell(transform.position);
        transform.position = new Vector3(currentGridPosition.x + Settings.gridCellSize / 2f, currentGridPosition.y + Settings.gridCellSize / 2f, 0);

        tragetGridPosition = currentGridPosition;
    }

    /// <summary>
    /// 主要移动方法
    /// </summary>
    private void Movement()
    {
        if (!npcMove)
        {
            Debug.Log("npc steps : " + movementSteps.Count);
            if (movementSteps.Count > 0)
            {
                MovementStep step = movementSteps.Pop();

                currentScene = step.sceneName;

                CheckVisiable();

                nextGridPosition = (Vector3Int)step.gridCoordinate;
                TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);

                MoveToGridPosition(nextGridPosition, stepTime);
            }
            else if (!isMoving && canPlayStopAnimaiton)
            {
                StartCoroutine(SetStopAnimation());
            }
        }
    }

    private void MoveToGridPosition(Vector3Int gridPos, TimeSpan stepTime)
    {
        npcMoveRoutine = StartCoroutine(MoveRoutine(gridPos, stepTime));
        Debug.Log("npc move routine "+npcMoveRoutine.ToString());
    }

    private IEnumerator MoveRoutine(Vector3Int gridPos, TimeSpan stepTime)
    {
        npcMove = true;
        nextWorldPosition = GetWorldPostion(gridPos);

        //还有时间用来移动
        if (stepTime > GameTime)
        {
            //用来移动的时间差，以秒为单位
            float timeToMove = (float)(stepTime.TotalSeconds - GameTime.TotalSeconds);
            //实际移动距离
            float distance = Vector3.Distance(transform.position, nextWorldPosition);
            //实际移动速度
            float speed = Mathf.Max(minSpeed, (distance / timeToMove / Settings.secondThreshold));

            if (speed <= maxSpeed)
            {
                while (Vector3.Distance(transform.position, nextWorldPosition) > Settings.pixelSize)
                {
                    dir = (nextWorldPosition - transform.position).normalized;

                    Vector2 posOffset = new Vector2(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime);
                    rb.MovePosition(rb.position + posOffset);
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        //如果时间已经到了就瞬移
        rb.position = nextWorldPosition;
        currentGridPosition = gridPos;
        nextGridPosition = currentGridPosition;

        npcMove = false;
    }


    /// <summary>
    /// 根据Schedule构建路径
    /// </summary>
    /// <param name="schedule"></param>
    public void BuildPath(ScheduleDetails schedule)
    {
        Debug.Log("building path");
        movementSteps.Clear();
        currentSchedule = schedule;
        targetScene = schedule.targetScene;
        tragetGridPosition = (Vector3Int)schedule.targetGridPosition;
        stopAnimationClip = schedule.clipAtStop;
        this.interactable = schedule.interactable;
        Debug.Log("schedule's target pos is : " + schedule.targetGridPosition + " and hour is : " + schedule.hour);
        Debug.Log("check if target scene is current scene");
        if (schedule.targetScene == currentScene)
        {
            Debug.Log("using astar");
            if (AStar.Instance != null) 
            { 
                Debug.Log("Astar has been initialized"); 
            }
            else 
                Debug.Log("Astar not initialized");
            
            AStar.Instance.BuildPath(schedule.targetScene, (Vector2Int)currentGridPosition, schedule.targetGridPosition, movementSteps);
        }
        else if (schedule.targetScene != currentScene)
        {
            Debug.Log("need to change scene");
            //这个东西得加上,从哪来去到哪
            SceneRoute sceneRoute = NPCManager.Instance.GetSceneRoute(currentScene, schedule.targetScene);

            if (sceneRoute != null)
            {
                for (int i = 0; i < sceneRoute.scenePathList.Count; i++)
                {
                    Vector2Int fromPos, gotoPos;
                    ScenePath path = sceneRoute.scenePathList[i];

                    if (path.fromGridCell.x >= Settings.maxGridSize)
                    {
                        fromPos = (Vector2Int)currentGridPosition;
                    }
                    else
                    {
                        fromPos = path.fromGridCell;
                    }

                    if (path.gotoGridCell.x >= Settings.maxGridSize)
                    {
                        gotoPos = schedule.targetGridPosition;
                    }
                    else
                    {
                        gotoPos = path.gotoGridCell;
                    }

                    AStar.Instance.BuildPath(path.sceneName, fromPos, gotoPos, movementSteps);
                }
            }
        }

        if (movementSteps.Count > 1)
        {
            //更新每一步对应的时间戳
            UpdateTimeOnPath();
        }
    }


    /// <summary>
    /// 更新路径上每一步的时间
    /// </summary>
    private void UpdateTimeOnPath()
    {
        MovementStep previousSetp = null;

        TimeSpan currentGameTime = GameTime;

        foreach (MovementStep step in movementSteps)
        {
            if (previousSetp == null)
                previousSetp = step;

            step.hour = currentGameTime.Hours;
            step.minute = currentGameTime.Minutes;
            step.second = currentGameTime.Seconds;

            TimeSpan gridMovementStepTime;

            if (MoveInDiagonal(step, previousSetp))
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / normalSpeed / Settings.secondThreshold));
            else
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.secondThreshold));

            //累加获得下一步的时间戳
            currentGameTime = currentGameTime.Add(gridMovementStepTime);
            //循环下一步
            previousSetp = step;
        }
    }

    /// <summary>
    /// 判断是否走斜方向
    /// </summary>
    /// <param name="currentStep"></param>
    /// <param name="previousStep"></param>
    /// <returns></returns>
    private bool MoveInDiagonal(MovementStep currentStep, MovementStep previousStep)
    {
        return (currentStep.gridCoordinate.x != previousStep.gridCoordinate.x) && (currentStep.gridCoordinate.y != previousStep.gridCoordinate.y);
    }

    /// <summary>
    /// 网格坐标返回世界坐标中心点
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    private Vector3 GetWorldPostion(Vector3Int gridPos)
    {
        Vector3 worldPos = gird.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize / 2f, worldPos.y + Settings.gridCellSize / 2);
    }


    private void SwitchAnimation()
    {
        isMoving = transform.position != GetWorldPostion(tragetGridPosition);

        anim.SetBool("IsMoving", isMoving);
        if (isMoving)
        {
            //anim.SetBool("Exit", true);
            anim.SetFloat("dirX", dir.x);
            anim.SetFloat("dirY", dir.y);
        }
        else
        {
            anim.SetBool("Exit", false);
        }
    }

    private IEnumerator SetStopAnimation()
    {
        //强制面向镜头
        anim.SetFloat("DirX", 0);
        anim.SetFloat("DirY", -1);

        animationBreakTime = Settings.animationBreakTime;
        if (stopAnimationClip != null)
        {
            animOverride[blankAnimationClip] = stopAnimationClip;
            anim.SetBool("EventAnimation", true);
            yield return null;
            anim.SetBool("EventAnimation", false);
        }
        else
        {
            animOverride[stopAnimationClip] = blankAnimationClip;
            anim.SetBool("EventAnimation", false);
        }
    }

    #region 设置NPC显示情况
    private void SetActiveInScene()
    {
        spriteRenderer.enabled = true;
        coll.enabled = true;

        //transform.GetChild(0).gameObject.SetActive(true);
    }

    private void SetInactiveInScene()
    {
        spriteRenderer.enabled = false;
        coll.enabled = false;

        //transform.GetChild(0).gameObject.SetActive(false);
    }
    #endregion
    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.characterPosDict = new Dictionary<string, SerializableVector3>();
        saveData.characterPosDict.Add("targetGridPosition", new SerializableVector3(tragetGridPosition));
        saveData.characterPosDict.Add("currentPosition", new SerializableVector3(transform.position));
        saveData.dataSceneName = currentScene;
        saveData.targetScene = this.targetScene;
        if (stopAnimationClip != null)
        {
            saveData.animationInstanceID = stopAnimationClip.GetInstanceID();
        }
        saveData.interactable = this.interactable;//是否可以互动
        saveData.timeDict = new Dictionary<string, int>();
        saveData.timeDict.Add("currentSeason", (int)currentSeason);
        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        isInitialised = true;
        isFirstLoad = false;

        currentScene = saveData.dataSceneName;
        targetScene = saveData.targetScene;

        Vector3 pos = saveData.characterPosDict["currentPosition"].ToVector3();
        Vector3Int gridPos = (Vector3Int)saveData.characterPosDict["targetGridPosition"].ToVector2Int();

        transform.position = pos;
        tragetGridPosition = gridPos;

        if (saveData.animationInstanceID != 0)
        {
            this.stopAnimationClip = Resources.InstanceIDToObject(saveData.animationInstanceID) as AnimationClip;
        }

        this.interactable = saveData.interactable;
        this.currentSeason = (Season)saveData.timeDict["currentSeason"];
    }
    
}
