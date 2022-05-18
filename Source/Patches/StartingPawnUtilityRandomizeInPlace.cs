using System;
using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace GryphonPawnGenerator.Patches
{
    // Token: 0x0200014C RID: 332
    [HarmonyPatch(typeof(StartingPawnUtility), "RandomizeInPlace")]
    public static class StartingPawnUtilityRandomizeInPlace
    {
        // Token: 0x06000931 RID: 2353 RVA: 0x0002AAA1 File Offset: 0x00028CA1
        // ReSharper disable once InconsistentNaming
        [HarmonyPrefix]
        [UsedImplicitly]
        public static void Patch(Pawn p)
        {
            try
            {
                PawnInfo.Remove(p);
                GroupInfo.Remove(p);
            }
            catch (Exception e)
            {
                Log.Message($"My error {e} in {nameof(StartingPawnUtilityRandomizeInPlace)}!!");
                throw;
            }
        }
    }
}