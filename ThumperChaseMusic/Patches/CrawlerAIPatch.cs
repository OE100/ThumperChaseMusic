using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ThumperChaseMusic.Patches
{
    [HarmonyPatch(typeof(CrawlerAI))]
    internal class CrawlerAIPatch
    {
        private static AudioClip thumperSound = null;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void patchThumperChaseSoundStart(CrawlerAI __instance)
        {
            Plugin.log.LogInfo("Patching CrawlerAI...");
            if (thumperSound == null)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(Paths.PluginPath + "\\OE_Tweaks\\Sounds\\thumpersounds");
                if (ab == null)
                {
                    Plugin.log.LogError("Failed to load thumpersounds asset bundle");
                    return;
                }
                thumperSound = ab.LoadAsset<AudioClip>("thumper_chase.mp3");
            }
            __instance.GetComponent<AudioSource>().clip = thumperSound;
            __instance.GetComponent<AudioSource>().volume = 0.8f;
            __instance.GetComponent<AudioSource>().loop = true;
            __instance.GetComponent<AudioSource>().dopplerLevel = 0.0f;
            __instance.GetComponent<AudioSource>().spatialBlend = 0.7f;
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void patchTumperChaseSoundUpdate(CrawlerAI __instance)
        {
            if (__instance.isEnemyDead)
            {
                __instance.GetComponent<AudioSource>().Stop();
            }
            if (__instance.currentBehaviourStateIndex == 0 && __instance.GetComponent<AudioSource>().isPlaying)
            {
                __instance.GetComponent<AudioSource>().Pause();
            }
            else if (__instance.currentBehaviourStateIndex != 0 && !__instance.GetComponent<AudioSource>().isPlaying)
            {
                if (__instance.GetComponent<AudioSource>().time > 0.0f && __instance.GetComponent<AudioSource>().time <= thumperSound.length - 0.1)
                    __instance.GetComponent<AudioSource>().UnPause();
                else
                    __instance.GetComponent<AudioSource>().Play();
            }
        }
    }
}
