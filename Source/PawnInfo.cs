using System.Collections.Generic;
using System.Linq;
using Verse;

namespace GryphonPawnGenerator
{
    internal sealed class PawnInfo
    {
        public readonly Dictionary<string, SkillsHelper.State> Skills;

        private PawnInfo(Dictionary<string, SkillsHelper.State> skills) => Skills = skills;

        public static PawnInfo GetOrCreate(Pawn pawn)
        {
            if (pawn is null)
            {
                return null;
            }

            if (!All.ContainsKey(pawn.ThingID))
            {
                Dictionary<string, SkillsHelper.State> skills =
                    pawn.skills.skills.ToDictionary(r => r.def.defName, r => default(SkillsHelper.State));
                All[pawn.ThingID] = new PawnInfo(skills);
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
