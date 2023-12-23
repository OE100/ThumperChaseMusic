using HarmonyLib;

namespace ThumperChaseMusic.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch
    {
        [HarmonyPatch("openingDoorsSequence")]
        [HarmonyPostfix]
        private static void PatchOpeningDoorsSequence()
        {
            CrawlerAIPatch.crawlerSources.Clear();
        }
    }
}