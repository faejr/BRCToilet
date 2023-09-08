using HarmonyLib;
using Reptile;
using TrickMultiplier;
using static Reptile.Player;

namespace BRCToilet.Patches;

[HarmonyPatch(typeof(Player))]
public class PlayerPatch
{
    private static TrickType lastType = TrickType.NONE;

    [HarmonyPrefix]
    [HarmonyPatch("DoTrick")]
    private static void DoTrick(Player __instance, ref TrickType type)
    {
        var traverse = Traverse.Create(__instance);
        var addScoreMultiplier = traverse.Method("AddScoreMultiplier");

        switch (type)
        {
            case TrickType.GRIND_SWITCH_MOVESTYLE:
            case TrickType.GRIND_START:
            case TrickType.GRIND:
            case TrickType.AIR:
            case TrickType.AIR_VERT:
            case TrickType.AIR_BOOST:
            case TrickType.SPECIAL_AIR:
            case TrickType.HANDPLANT:
            case TrickType.POLE_FLIP:
                addScoreMultiplier.GetValue();
                break;
            case TrickType.SLIDE:
                if (lastType != TrickType.SLIDE)
                {
                    addScoreMultiplier.GetValue();
                }
                break;
        }
        lastType = type;
    }
}