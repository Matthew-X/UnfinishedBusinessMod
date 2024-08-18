using Assets.Scripts.LevelObjectives;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx;
using HarmonyLib;
using Assets.Scripts.LevelObjectives.Handlers;
using DG.Tweening.Plugins.Core;
using BepInEx.Configuration;
using UnityEngine;

namespace UnfinishedBusinessMod
{
    [BepInPlugin("alittlelonger.UnfinishedBusinessMod", "Unfinished Business Mod", "1.0.0")]
    [BepInProcess("DRG Survivor.exe")]
    public class DRGSMod : BasePlugin
    {

        internal static ConfigEntry<int> TimeOutDuration { get; private set; }
        internal static ManualLogSource Logger { get; set; }
        private readonly Harmony harmony = new Harmony("alittlelonger.UnfinishedBusinessMod");
        public override void Load()
        {
            TimeOutDuration = Config.Bind("01. Timeout", "Minutes", 60, new ConfigDescription("Set how many minutes you will have at the end of each stage"));
            TimeOutDuration.SettingChanged += (_, _) =>
            {
                Patches.UpdateDropPodTimer();
            };
            harmony.PatchAll();
            Logger = Log;
        }
    }


    [Harmony]
    public static class Patches
    {
        private static DropPod DropPodInstance { get; set; }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DropPod), nameof(DropPod.Awake))]
        public static void DropPod_Awake(DropPod __instance)
        {
            DropPodInstance = __instance;
            UpdateDropPodTimer();    
        }
        internal static void UpdateDropPodTimer()
        {
            if (!DropPodInstance) return;

            DRGSMod.Logger.LogInfo($"Drop Pod Time: {DRGSMod.TimeOutDuration.Value}");
            DropPodInstance.secondsToTimeOut = (int)(DRGSMod.TimeOutDuration.Value * 60);

        }
    }
}