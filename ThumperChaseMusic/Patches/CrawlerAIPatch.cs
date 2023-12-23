using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ThumperChaseMusic.Patches
{
    [HarmonyPatch(typeof(CrawlerAI))]
    internal class CrawlerAIPatch
    {
        private static AudioClip thumperSound = null;

        internal static Dictionary<CrawlerAI, AudioSource> crawlerSources = new Dictionary<CrawlerAI, AudioSource>();
        
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void patchThumperChaseSoundStart(CrawlerAI __instance)
        {
            Plugin.log.LogInfo("Patching CrawlerAI...");
            if (thumperSound == null)
            {
                AssetBundle ab = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames()[0]));
                if (ab == null)
                {
                    Plugin.log.LogError("Failed to load thumpersounds asset bundle");
                    return;
                }
                thumperSound = ab.LoadAsset<AudioClip>("thumper_chase.mp3");
            }

            AudioSource newSource = __instance.gameObject.AddComponent<AudioSource>();
            newSource.clip = thumperSound;
            newSource.volume = 0.8f;
            newSource.loop = true;
            newSource.dopplerLevel = 0.0f;
            newSource.spatialBlend = 0.7f;
            newSource.maxDistance = 150f;
            crawlerSources[__instance] = newSource;
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void patchTumperChaseSoundUpdate(CrawlerAI __instance)
        {
            AudioSource source = crawlerSources[__instance];
            if (__instance.isEnemyDead)
            {
                source.Stop();
            }
            if (__instance.currentBehaviourStateIndex == 0 && source.isPlaying)
            {
                source.Pause();
            }
            else if (__instance.currentBehaviourStateIndex != 0 && !source.isPlaying)
            {
                if (source.time > 0.0f && source.time <= thumperSound.length - 0.1)
                    source.UnPause();
                else
                    source.Play();
            }
        }
    }
}
