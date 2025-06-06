using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using SuperNewRoles.CustomOptions.Categories;
using SuperNewRoles.Modules;

namespace SuperNewRoles.MapCustoms;
public static class FungleAdditionalElectrical
{
    public static void CreateElectrical()
    {
        if (!MapCustomHandler.IsMapCustom(MapCustomHandler.MapCustomId.TheFungle) ||
            !MapEditSettingsOptions.TheFunglePowerOutageSabotage)
            return;

        FungleShipStatus fungleShipStatus = ShipStatus.Instance.TryCast<FungleShipStatus>();
        if (fungleShipStatus == null)
            return;
        SwitchSystem system = new();
        fungleShipStatus.Systems[SystemTypes.Electrical] = system.TryCast<ISystemType>();
        fungleShipStatus.Systems[SystemTypes.Sabotage].TryCast<SabotageSystemType>().specials.Add(system.TryCast<IActivatable>());

        MapLoader.LoadMap(MapNames.Airship, (ship) =>
        {
            if (ship == null)
                return;

            List<PlayerTask> Tasks = ShipStatus.Instance.SpecialTasks.ToList();
            Tasks.Add(ship.SpecialTasks.FirstOrDefault(x => x.TaskType == TaskTypes.FixLights));
            ShipStatus.Instance.SpecialTasks = new(Tasks.ToArray());

            Console console1 = GameObject.Instantiate(ship.transform.FindChild("Storage/task_lightssabotage (cargo)"), fungleShipStatus.transform).GetComponent<Console>();
            console1.transform.localPosition = new(-16.2f, 7.67f, 0);
            console1.ConsoleId = 0;

            Console console2 = GameObject.Instantiate(console1, fungleShipStatus.transform);//skeld.transform.FindChild("Electrical/Ground/electric_frontset/SwitchConsole"), fungleShipStatus.transform).GetComponent<Console>();
            console2.transform.localPosition = new(-5.7f, -7.7f, -1.008f);
            console2.ConsoleId = 1;

            Console console3 = GameObject.Instantiate(console1, fungleShipStatus.transform);
            console3.transform.localPosition = new(21.48f, 4.27f, 0f);
            console3.ConsoleId = 2;

            List<Console> Consoles = ShipStatus.Instance.AllConsoles.ToList();
            Consoles.Add(console1);
            Consoles.Add(console2);
            Consoles.Add(console3);
            ShipStatus.Instance.AllConsoles = Consoles.ToArray();
        });
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Awake))]
    class MapBehaviourAwakePatch
    {
        public static void Postfix(MapBehaviour __instance)
        {
            if (!MapCustomHandler.IsMapCustom(MapCustomHandler.MapCustomId.TheFungle) ||
                !MapEditSettingsOptions.TheFunglePowerOutageSabotage)
                return;

            MapLoader.LoadMap(MapNames.Airship, (ship) =>
            {
                if (ship == null)
                    return;

                MapRoom mapRoom = GameObject.Instantiate(ship.MapPrefab.infectedOverlay.rooms.FirstOrDefault(x => x.room == SystemTypes.Electrical), __instance.infectedOverlay.transform);
                mapRoom.Parent = __instance.infectedOverlay;
                mapRoom.transform.localPosition = new(-0.83f, -1.8f, -1f);
                var buttons = __instance.infectedOverlay.allButtons.ToList();
                buttons.Add(mapRoom.GetComponentInChildren<ButtonBehavior>());
                __instance.infectedOverlay.allButtons = buttons.ToArray();
                var buttons2 = __instance.infectedOverlay.rooms.ToList();
                buttons2.Add(mapRoom);
                __instance.infectedOverlay.rooms = buttons2.ToArray();
            });
        }
    }
}