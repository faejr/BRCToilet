using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using BepInEx.Configuration;
using System.IO;
using System.Drawing;

namespace BRCToilet
{
    [BepInPlugin("gay.faejr.plugin.brctoilet", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static string PLUGIN_GUID = "gay.faejr.plugin.brctoilet";
        public static ManualLogSource Log = null!;

        public static string DisplayToiletRadarKey = "Display toilet radar";

        public static ConfigEntry<bool> DisplayToiletRadar;

        public static byte[] GetImage(string filename)
        {
            var assembly = typeof(Plugin).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(filename))
            {
                if (stream == null) return null;
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        private void Awake()
        {
            Log = Logger;

            DisplayToiletRadar = Config.Bind("General", DisplayToiletRadarKey, true);

            Application.runInBackground = true;
            SetupHarmony();

            new ToiletManager();
        }

        private void SetupHarmony()
        {
            var Harmony = new Harmony("BRCToilet.Harmony");

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
