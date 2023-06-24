using System.Linq;
using HarmonyLib;

namespace SuperNewRoles.Patches;

class Chat
{
    [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
    public static class SetBubbleName
    {
        public static void Postfix(ChatBubble __instance, [HarmonyArgument(0)] string playerName)
        {
            //チャット欄でImpostor陣営から見たSpyがばれないように
            PlayerControl sourcePlayer = CachedPlayer.AllPlayers.ToArray().ToList().FirstOrDefault(x => x.Data.PlayerName.Equals(playerName));
            if (sourcePlayer != null && CachedPlayer.LocalPlayer.PlayerControl.IsImpostor() && sourcePlayer.IsRole(RoleId.Egoist, RoleId.Spy))
            {
                __instance.NameText.color = Palette.ImpostorRed;
            }
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    private static class EnableChat
    {
        // 参考 => https://github.com/yukinogatari/TheOtherRoles-GM/blob/gm-main/TheOtherRoles/Modules/ChatCommands.cs
        private static void Postfix(HudManager __instance)
        {
            if (__instance?.Chat?.isActiveAndEnabled == false && CanUseChat())
                __instance?.Chat?.SetVisible(true);
        }

        private static bool CanUseChat()
        {
            if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay) return true;
            if (ModHelpers.IsDebugMode() && CustomOptionHolder.CanUseChatWhenTaskPhase.GetBool()) return true;
            return false;
        }
    }
}