using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using SuperNewRoles.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SuperNewRoles.RequestInGame;

public class CreateButtons
{
    public static AspectPosition button;
    public static GameObject notificationBadge;
    public static void GenerateButtons(Transform parent, Vector3 position, AspectPosition.EdgeAlignments alignment)
    {
        ModManagerLateUpdatePatch.ForceNotificationCheck = true;
        GameObject bugReportButton = GameObject.Instantiate(AssetManager.GetAsset<GameObject>("bugReport"), parent);
        bugReportButton.transform.localPosition = position;
        bugReportButton.transform.localScale = Vector3.one * 0.15f;
        PassiveButton passiveButton = bugReportButton.AddComponent<PassiveButton>();
        passiveButton.Colliders = new Collider2D[] { bugReportButton.GetComponent<CircleCollider2D>() };
        passiveButton.OnClick = new();
        passiveButton.OnClick.AddListener((UnityAction)(() =>
        {
            SelectButtonsMenu.GenerateButtonsMenu();
        }));
        passiveButton.OnMouseOut = new();
        passiveButton.OnMouseOver = new();
        AspectPosition aspectPosition = bugReportButton.AddComponent<AspectPosition>();
        aspectPosition.Alignment = alignment;
        aspectPosition.DistanceFromEdge = position;
        aspectPosition.OnEnable();
        button = aspectPosition;
        notificationBadge = bugReportButton.transform.Find("badge").gameObject;
    }

    [HarmonyPatch(typeof(ModManager), nameof(ModManager.LateUpdate))]
    public class ModManagerLateUpdatePatch
    {
        private const float UpdateInterval = 10f;
        private static float updateTimer = 0f;
        public static bool ForceNotificationCheck = false;
        public static void Postfix(ModManager __instance)
        {
            if (updateTimer >= UpdateInterval || ForceNotificationCheck)
            {
                if (notificationBadge != null)
                {
                    __instance.StartCoroutine(RequestInGameManager.hasNotifications(hasNotifications =>
                    {
                        notificationBadge.SetActive(hasNotifications);
                    }).WrapToIl2Cpp());
                }
                updateTimer = 0f;
                ForceNotificationCheck = false;
            }
            updateTimer += Time.deltaTime;
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public class MainMenuManagerStartPatch
    {
        public static void Postfix(MainMenuManager __instance)
        {
            GenerateButtons(__instance.transform, new(-0.9f, 1.3f, 0f), AspectPosition.EdgeAlignments.Top);
            TextMeshPro text = GameObject.Instantiate(AssetManager.GetAsset<GameObject>("bugReportText"), __instance.transform).GetComponent<TextMeshPro>();
            text.transform.localPosition = new(0.88f, 1.715f, 0f);
            text.transform.localScale = Vector3.one * 0.15f;
            text.alignment = TextAlignmentOptions.Left;
            text.text = ModTranslation.GetString("RequestInGameButtonText");
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public class HudManagerStartPatch
    {
        public static void Postfix(HudManager __instance)
        {
            GenerateButtons(__instance.transform, new(2.7f, 1.3f, 0f), AspectPosition.EdgeAlignments.RightTop);
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGame))]
    public class AmongUsClientStartPatch
    {
        public static void Postfix(AmongUsClient __instance)
        {
            if (button != null)
            {
                button.DistanceFromEdge = new(0.23f, 1.1f, 0f);
                button.OnEnable();
            }
        }
    }
}