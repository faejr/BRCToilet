using HarmonyLib;
using Reptile;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace BRCToilet.Patches;

[HarmonyPatch(typeof(Mapcontroller))]
public class MapControllerPatch
{

    [HarmonyPostfix]
    [HarmonyPatch("Awake")]
    private static void Awake(Mapcontroller __instance)
    {
        var traverse = Traverse.Create(__instance);
        GameObject publicToiletPrefab;

        // Create base material
        var publicToiletMaterial = new Material(Shader.Find("Standard"))
        {
            color = new Color(0f, 1f, 0f)
        };

        if (Plugin.DisplayToiletRadar.Value)
        {
            var m_StoryObjectivePin = traverse.Field<GameObject>("m_StoryObjectivePin").Value;
            publicToiletPrefab = GameObject.Instantiate(m_StoryObjectivePin);
            publicToiletPrefab.name = "PublicToiletPin";

            // Destroy things we don't need
            Object.Destroy(publicToiletPrefab.GetComponent<StoryObjectivePin>());
            Object.Destroy(publicToiletPrefab.transform.Find("OutOfViewVisualization").gameObject);

            var mapPin = publicToiletPrefab.AddComponent<MapPin>();
            mapPin.InitMapPin(MapPin.PinType.Pin);

            var inView = publicToiletPrefab.transform.Find("InViewVisualization");
            inView.localScale = new Vector3(6, 6, 6);

            inView.GetComponentInChildren<MeshRenderer>().material = publicToiletMaterial;
            var particleSystemRenderer = inView.GetComponentInChildren<ParticleSystemRenderer>();
            Material publicToiletParticleMaterial = CreateParticleMaterial(particleSystemRenderer);
            particleSystemRenderer.material = publicToiletParticleMaterial;

            var particleSystem = inView.GetComponentInChildren<ParticleSystem>();
            particleSystem.startColor = new Color(0f, 1f, 0f);
        }
        else
        {
            var m_GraffitiPinPrefab = traverse.Field<GameObject>("m_GraffitiPinPrefab").Value;
            publicToiletPrefab = GameObject.Instantiate(m_GraffitiPinPrefab);
            publicToiletPrefab.name = "PublicToiletPin";
            publicToiletPrefab.GetComponentInChildren<MeshRenderer>().material = publicToiletMaterial;
            publicToiletPrefab.GetComponent<LineRenderer>().sharedMaterial = publicToiletMaterial;
            var mapPinComp = publicToiletPrefab.GetComponent<MapPin>();
            Traverse.Create(mapPinComp).Field<MapPin.PinType>("m_pinType").Value = MapPin.PinType.Pin;
        }
        publicToiletPrefab.SetActive(value: false);

        var m_PinPrefabs = traverse.Field<Dictionary<MapPin.PinType, GameObject>>("m_PinPrefabs").Value;
        m_PinPrefabs[MapPin.PinType.Pin] = publicToiletPrefab;
        Plugin.Log.LogInfo("Set up new prefab :)");
    }

    private static Material CreateParticleMaterial(ParticleSystemRenderer particleSystemRenderer)
    {
        var publicToiletParticleMaterial = new Material(particleSystemRenderer.material.shader);
        publicToiletParticleMaterial.CopyPropertiesFromMaterial(particleSystemRenderer.material);
        Texture2D texture2D = new Texture2D(2, 2, GraphicsFormat.R8G8B8A8_UNorm, 1, TextureCreationFlags.None);
        texture2D.LoadImage(Plugin.GetImage("BRCToilet.assets.PublicToiletRadar.png"));
        publicToiletParticleMaterial.mainTexture = texture2D;
        return publicToiletParticleMaterial;
    }

    [HarmonyPrefix]
    [HarmonyPatch("CreatePin")]
    private static bool CreatePin(Mapcontroller __instance, MapPin.PinType pinType, ref MapPin __result)
    {
        if (pinType == MapPin.PinType.Pin)
        {
            Plugin.Log.LogInfo("Creating toilet pin");
            var traverse = Traverse.Create(__instance);
            var m_PinPrefabs = traverse.Field<Dictionary<MapPin.PinType, GameObject>>("m_PinPrefabs").Value;
            var m_PinsGroup = traverse.Field<Transform>("m_PinsGroup").Value;

            GameObject obj = Object.Instantiate(m_PinPrefabs[MapPin.PinType.Pin], m_PinsGroup.transform);
            obj.layer = __instance.gameObject.layer;

            MapPin component = obj.GetComponent<MapPin>();
            component.gameObject.SetActive(value: true);
            component.SetMapController(__instance);

            var m_MapPins = traverse.Field<List<MapPin>>("m_MapPins").Value;
            m_MapPins.Add(component);
            __result = component;
            return false;
        }
        return true;
    }
}