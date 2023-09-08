using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Linq;

namespace TrickMultiplier
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log;
        private void Awake()
        {
            Log = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            SetupHarmony();
        }

        private void SetupHarmony()
        {
            var Harmony = new Harmony("TrickMultiplier.Harmony");

            var patches = typeof(Plugin).Assembly.GetTypes()
                                        .Where(m => m.GetCustomAttributes(typeof(HarmonyPatch), false).Length > 0)
                                        .ToArray();

            foreach (var patch in patches)
            {
                Harmony.PatchAll(patch);
            }
        }
    }
}
