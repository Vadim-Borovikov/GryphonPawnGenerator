using System;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace GryphonPawnGenerator
{
    // Token: 0x020002EF RID: 751
    [HarmonyPatch(typeof(PawnGenerator), "TryGenerateNewPawnInternal")]
    public static class PatchPawnGeneratorTryGenerateNewPawnInternal
    {
        // Token: 0x06001514 RID: 5396 RVA: 0x0007B1DC File Offset: 0x000793DC
        // ReSharper disable once InconsistentNaming
        [HarmonyPostfix]
        [UsedImplicitly]
        public static void Patch(ref PawnGenerationRequest request, ref Pawn __result)
        {
            try
            {
                if (__result is null)
                {
                    return;
                }

                if (request.Context != PawnGenerationContext.PlayerStarter)
                {
                    return;
                }

                if (PawnChecker.Check(__result, out string _))
                {
                    return;
                }

                PawnInfo.All.Remove(__result.ThingID);
                // string name = __result.Name.ToStringFull ?? "<Empty>";
                // Log.Message($"{name} rejected: {error}");
                __result = null;
            }
            catch (Exception e)
            {
                Log.Message($"My error {e}!!");
                throw;
            }
        }
    }
}
