using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using HarmonyLib;
using SuperNewRoles.Modules;
using SuperNewRoles.Modules.Events.Bases;
using UnityEngine;

namespace SuperNewRoles.Events;

public class WrapUpEventData : IEventData
{
    public NetworkedPlayerInfo exiled { get; }
    public WrapUpEventData(NetworkedPlayerInfo exiled)
    {
        this.exiled = exiled;
    }
}

public class WrapUpEvent : EventTargetBase<WrapUpEvent, WrapUpEventData>
{
    public static void Invoke(NetworkedPlayerInfo exiled)
    {
        var data = new WrapUpEventData(exiled);
        Instance.Awake(data);
    }
}

[HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
public static class WrapUpPatch
{
    public static void Postfix(ExileController __instance)
    {
        WrapUpEvent.Invoke(__instance.initData.networkedPlayer);
        CheckGameEndPatch.CouldCheckEndGame = false;
        new LateTask(() =>
        {
            CheckGameEndPatch.CouldCheckEndGame = true;
        }, 0.5f);
    }
}

[HarmonyCoroutinePatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
public static class AirshipWrapUpPatch
{
    // スレッドセーフなHashSetを使用して処理済みインスタンスを管理
    private static readonly HashSet<int> _processedInstances = new();

    // 古いエントリを定期的にクリアするためのタイマー
    private static DateTime _lastCleanup = DateTime.UtcNow;
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromMinutes(5);

    public static void Postfix(object __instance)
    {
        AirshipExileController airshipExileController = HarmonyCoroutinePatchProcessor.GetParentFromCoroutine<AirshipExileController>(__instance);
        if (airshipExileController == null) return;

        var instanceId = airshipExileController.GetInstanceID();

        // より効率的な重複チェック
        if (!_processedInstances.Add(instanceId))
        {
            return; // 既に処理済み
        }

        Logger.Info("AirshipWrapUpPatch 開始");
        WrapUpEvent.Invoke(airshipExileController.initData.networkedPlayer);
        CheckGameEndPatch.CouldCheckEndGame = false;
        new LateTask(() =>
        {
            CheckGameEndPatch.CouldCheckEndGame = true;
        }, 0.5f);
    }
}