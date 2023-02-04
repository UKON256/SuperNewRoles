using SuperNewRoles.Buttons;

namespace SuperNewRoles.Roles;

public static class DoubleKiller
{
    public static void ResetMainCooldown()
    {
        HudManagerStartPatch.DoubleKillerMainKillButton.MaxTimer = CustomOptionHolder.MainKillCoolTime.GetFloat();
        HudManagerStartPatch.DoubleKillerMainKillButton.Timer = HudManagerStartPatch.DoubleKillerMainKillButton.MaxTimer;
    }
    public static void ResetSubCooldown()
    {
        HudManagerStartPatch.DoubleKillerSubKillButton.MaxTimer = CustomOptionHolder.SubKillCoolTime.GetFloat();
        HudManagerStartPatch.DoubleKillerSubKillButton.Timer = HudManagerStartPatch.DoubleKillerSubKillButton.MaxTimer;
    }
    public static void EndMeeting()
    {
        ResetSubCooldown();
        ResetMainCooldown();
    }
    public class DoubleKillerFixedPatch
    {
        public static void SetOutline()
        {
            if (PlayerControl.LocalPlayer.IsRole(RoleId.DoubleKiller))
            {
                Patches.PlayerControlFixedUpdatePatch.SetPlayerOutline(Patches.PlayerControlFixedUpdatePatch.SetTarget(), RoleClass.DoubleKiller.color);
            }
        }
    }
}