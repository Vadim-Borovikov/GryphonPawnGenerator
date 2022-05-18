using System;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace GryphonPawnGenerator.Patches
{
    // Token: 0x02001378 RID: 4984
    [HarmonyPatch(typeof(Page_ConfigureStartingPawns), "DrawSkillSummaries")]
    public static class PageConfigureStartingPawnsDrawSkillSummaries
    {
        // Token: 0x06007A06 RID: 31238 RVA: 0x002AB124 File Offset: 0x002A9324
        // ReSharper disable once InconsistentNaming
        [HarmonyPostfix]
        [UsedImplicitly]
        public static void Patch(Page_ConfigureStartingPawns __instance, Rect rect)
        {
            try
            {
                Rect section = rect;
                section.y += 75f;
                section.yMax += 10f;
                section.xMax += 445f;
                Widgets.DrawMenuSection(section);

                Rect label = section.ContractedBy(7f);
                label = new Rect(label.min, new Vector2(label.width, 400f));
                string info = GroupInfo.GetTeamInfo();
                Widgets.Label(label, info);
            }
            catch (Exception e)
            {
                Log.Message($"My error {e} in {nameof(PageConfigureStartingPawnsDrawSkillSummaries)}!!");
                throw;
            }
        }
    }
}