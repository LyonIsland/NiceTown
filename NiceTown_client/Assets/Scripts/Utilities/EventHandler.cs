using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Dialogue;

public static class EventHandler
{
  public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;      
  public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)      
  {              
    UpdateInventoryUI?.Invoke(location, list);      
  }
  
  public static event Action<int, Vector3> InstantiateItemInScene;//在地面上生成物品
  public static void CallInstantiateItemInScene(int ID, Vector3 pos)
  {
    InstantiateItemInScene?.Invoke(ID, pos);
  }

  public static event Action<int, Vector3, ItemType> DropItemEvent;
  public static void CallDropItemEvent(int ID, Vector3 pos, ItemType itemType)
    {
        DropItemEvent?.Invoke(ID, pos, itemType);
    }

  public static event Action<ItemDetails, bool> ItemSelectedEvent;
  public static void CallItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails, isSelected);
    }

    public static event Action<int, int,int,Season> GameMinuteEvent;//时间切换
    public static void CallGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        GameMinuteEvent?.Invoke(minute, hour,day,season);
    }

    public static event Action<int, Season> GameDayEvent;
    public static void CallGameDayEvent(int day, Season season)
    {
        GameDayEvent?.Invoke(day, season);
    }

    public static event Action<int, int, int, int,  Season> GameDateEvent;//时间切换
    public static void CallGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        GameDateEvent?.Invoke(hour, day, month, year, season);
    }

    public static event Action<string, Vector3> TransitionEvent;//传送
    public static void CallTransitionEvent(string sceneName, Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
    }

    public static event Action BeforeSceneUnloadEvent;//在场景卸载之前执行event事件
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }

    public static event Action AfterSceneLoadedEvent;//加载场景后的event事件
    public static void CallAfterSceneLoadedEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
    }

    public static event Action<Vector3> MoveToPosition;//传送运动
    public static void CallMoveToPosition(Vector3 targetPosition)
    {
        MoveToPosition?.Invoke(targetPosition);
    }

    public static event Action<Vector3, ItemDetails> MouseClickedEvent;//鼠标点击事件
    public static void CallMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    {
        MouseClickedEvent?.Invoke(pos, itemDetails);
    }

    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimation;//实际事件在player动画之后
    public static void CallExecuteActionAfterAnimation(Vector3 pos, ItemDetails itemDetails)
    {
        ExecuteActionAfterAnimation?.Invoke(pos, itemDetails);
    }

    public static event Action<int, TileDetails> PlantSeedEvent;//种植
    public static void CallPlantSeedEvent(int ID, TileDetails tile)
    {
        PlantSeedEvent?.Invoke(ID, tile);
    }

    public static event Action<int> HarvestAtPlayerPosition;//在玩家附近生成物品
    public static void CallHarvestAtPlayerPosition(int ID)
    {
        HarvestAtPlayerPosition?.Invoke(ID);
    }

    public static event Action RefreshCurrentMap;
    public static void CallRefreshCurrentMap()
    {
        RefreshCurrentMap?.Invoke();
    }

     public static event Action<ParticleEffectType, Vector3> ParticleEffectEvent;//粒子效果
    public static void CallParticleEffectEvent(ParticleEffectType effectType, Vector3 pos)
    {
        ParticleEffectEvent?.Invoke(effectType, pos);
    }

    public static event Action GenerateCropEvent;
    public static void CallGenerateCropEvent()
    {
        GenerateCropEvent?.Invoke();
    }

    public static event Action<DialoguePiece> ShowDialogueEvent;//对话
    public static void CallShowDialogueEvent(DialoguePiece piece)
    {
        ShowDialogueEvent?.Invoke(piece);
    }

    //商店开启    
    public static event Action<SlotType, InventoryBag_SO> BaseBagOpenEvent;    
    public static void CallBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bag_SO)    
    {        
        BaseBagOpenEvent?.Invoke(slotType, bag_SO);    
    }
    
    public static event Action<SlotType, InventoryBag_SO> BaseBagCloseEvent;
    public static void CallBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bag_SO)
    {
        BaseBagCloseEvent?.Invoke(slotType, bag_SO);
    }
    
    public static event Action<GameState> UpdateGameStateEvent;
    public static void CallUpdateGameStateEvent(GameState gameState)
    {
        Debug.Log(gameState);
        UpdateGameStateEvent?.Invoke(gameState);
    }
    
    public static event Action<ItemDetails, bool> ShowTradeUI;
    public static void CallShowTradeUI(ItemDetails item, bool isSell)
    {
        ShowTradeUI?.Invoke(item, isSell);
    }
    
    //建造
    public static event Action<int, Vector3> BuildFurnitureEvent;
    public static void CallBuildFurnitureEvent(int ID, Vector3 pos)
    {
        BuildFurnitureEvent?.Invoke(ID, pos);
    }
    
    //灯光
    public static event Action<Season, LightShift, float> LightShiftChangeEvent;
    public static void CallLightShiftChangeEvent(Season season, LightShift lightShift, float timeDifference)
    {
        LightShiftChangeEvent?.Invoke(season, lightShift, timeDifference);
    }
    
    //音效
    public static event Action<SoundDetails> InitSoundEffect;
    public static void CallInitSoundEffect(SoundDetails soundDetails)
    {
        InitSoundEffect?.Invoke(soundDetails);
    }
    
    public static event Action<SoundName> PlaySoundEvent;
    public static void CallPlaySoundEvent(SoundName soundName)
    {
        PlaySoundEvent?.Invoke(soundName);
    }

    public static event Action<int> StartNewGameEvent;
    public static void CallStartNewGameEvent(int index)
    {
        StartNewGameEvent?.Invoke(index);
    }
    public static event Action EndGameEvent;
    public static void CallEndGameEvent()
    {
        EndGameEvent?.Invoke();
    }
}
