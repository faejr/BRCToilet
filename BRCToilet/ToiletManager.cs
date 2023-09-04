// StageManager.OnStageInitialized += this.StageInit;

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Reptile;
using UnityEngine;

namespace BRCToilet;

public class ToiletManager
{
    public ToiletManager()
    {
        StageManager.OnStagePostInitialization += this.StageInit;
    }

    private void StageInit()
    {
        Plugin.Log.LogInfo("Stage init uwu");

        var publicToilets = GetPublicToilets();

        Plugin.Log.LogInfo("Found " + publicToilets.Length + " toilets!");

        foreach (var toilet in publicToilets) {
            createToiletPin(toilet);
        }
    }

    private MapPin createToiletPin(PublicToilet toilet)
    {
        var mapController = Mapcontroller.Instance;
        var pin = Traverse.Create(mapController)
                    .Method("CreatePin", MapPin.PinType.Pin)
                    .GetValue<MapPin>();

        pin.AssignGameplayEvent(toilet.gameObject);
        pin.InitMapPin(MapPin.PinType.Pin);
        pin.OnPinEnable();

        return pin;
    }

    private PublicToilet[] GetPublicToilets()
    {
        return UnityEngine.Object.FindObjectsOfType<PublicToilet>();
    }
}