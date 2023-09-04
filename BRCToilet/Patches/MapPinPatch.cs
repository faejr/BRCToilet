using HarmonyLib;
using Reptile;
using UnityEngine;

namespace BRCToilet.Patches;

[HarmonyPatch(typeof(MapPin))]
public class MapPinPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("SetupMapPin")]
    private static void SetupMapPin(MapPin __instance)
    {
        var traverse = Traverse.Create(__instance);
        var m_pinType = traverse.Field<MapPin.PinType>("m_pinType").Value;

        if (m_pinType == MapPin.PinType.Pin)
        {
            Traverse.Create(__instance).Method("EnableMapPinGameObject");
            return;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch("RefreshMapPin")]
    private static void RefreshMapPin(MapPin __instance)
    {
        var traverse = Traverse.Create(__instance);
        var m_pinType = traverse.Field<MapPin.PinType>("m_pinType").Value;

        if (m_pinType == MapPin.PinType.Pin)
        {
            // Plugin.Log.LogInfo("Refreshing PublicToilet");
            var m_ObjectiveObject = traverse.Field<GameObject>("m_ObjectiveObject").Value;
            if (m_ObjectiveObject.GetComponent<PublicToilet>().CanSwap) {
                traverse.Method("EnableMapPinGameObject").GetValue();
            } else {
                traverse.Method("DisableMapPinGameObject").GetValue();
            }
            return;
        }
    }
}