using System.Collections.Generic;
using RimWorld;
using Verse;

namespace GryphonPawnGenerator
{
    internal sealed class PawnInfo
    {
        public readonly Dictionary<string, SkillsHelper.State> Skills = new Dictionary<string, SkillsHelper.State>();

        private PawnInfo(List<SkillRecord> skills)
        {
            foreach (SkillRecord record in skills)
            {
                string skill = record.def.defName;
                Skills[skill] = SkillsHelper.State.None;
            }
        }

        public static PawnInfo GetOrCreate(Pawn pawn)
        {
            if (pawn is null)
            {
                return null;
            }

            if (!All.ContainsKey(pawn.ThingID))
            {
                All[pawn.ThingID] = new PawnInfo(pawn.skills.skills);
            }
            return All[pawn.ThingID];
        }

        public static void Remove(Pawn pawn)
        {
            if (pawn is null)
            {
                return;
            }

            if (All.ContainsKey(pawn.ThingID))
            {
                All.Remove(pawn.ThingID);
            }
        }

        private static readonly Dictionary<string, PawnInfo> All = new Dictionary<string, PawnInfo>();
    }
}
