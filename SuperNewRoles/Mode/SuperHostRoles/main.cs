using System.Collections.Generic;

namespace SuperNewRoles.Mode.SuperHostRoles;

class Main
{
    public static void ClearAndReloads()
    {
        RealExiled = null;
        Chat.WinCond = null;
        // FixedUpdate.UpdateTime = new Dictionary<byte, float>();
        Patches.OnGameEndPatch.EndData = null;
    }
    public static PlayerControl RealExiled;
}