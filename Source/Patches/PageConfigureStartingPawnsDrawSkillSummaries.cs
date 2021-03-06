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
                TeamInfo info = GroupInfo.GetTeamInfo();
                if (info.Worst is null)
                {
                    Rect section = rect;
                    section.y += 75f;
                    section.yMax += 10f;
                    section.xMax += 300f;
                    Widgets.DrawMenuSection(section);

                    Rect label = section.ContractedBy(7f);
                    label = new Rect(label.min, new Vector2(label.width, 200f));
                    Widgets.Label(label, info.Message);
                }
                else
                {
                    __instance.SelectPawn(info.Worst);
                    StartingPawnUtility.RandomizeInPlace(info.Worst);
                }
            }
            catch (Exception e)
            {
                Log.Message($"My error {e} in {nameof(PageConfigureStartingPawnsDrawSkillSummaries)}!!");
                throw;
            }
        }
    }
}