using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace GryphonPawnGenerator
{
    internal sealed class PawnInfo
    {
        public readonly string Name;
        public readonly Dictionary<string, SkillsHelper.State> Skills;
        public readonly string TraitError;

        private PawnInfo(string name, Dictionary<string, SkillsHelper.State> skills, string traitError)
        {
            Name = name;
            Skills = skills;
            TraitError = traitError;
        }

        public static PawnInfo GetOrCreate(Pawn pawn)
        {
            if (pawn is null)
            {
                return null;
            }

            if (!All.ContainsKey(pawn.ThingID))
            {
                Dictionary<string, SkillsHelper.State> skills = SkillsHelper.GetSkills(pawn.skills.skills);
                string traitError = ScanTraits(pawn, skills);
                All[pawn.ThingID] = new PawnInfo(pawn.Name.ToStringShort, skills, traitError);
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


        private static string ScanTraits(Pawn pawn, Dictionary<string, SkillsHelper.State> skills)
        {
            foreach (Trait trait in pawn.story.traits.allTraits)
            {
                TraitDegreeData degreeData = trait.Degree == 0
                    ? trait.def.degreeDatas.FirstOrDefault()
                    : trait.def.degreeDatas.FirstOrDefault(d => d.degree == trait.Degree);
                if (degreeData is null)
                {
                    return $"no degree data in trait ({trait.def.defName})!";
                }

                string traitName = degreeData.untranslatedLabel;
                if (BlockedTraits.Contains(traitName))
                {
                    return trait.def.degreeDatas.Count < 2
                        ? trait.def.defName
                        : $"{trait.def.defName} - {traitName}";
                }

                SkillsHelper.RegisterSkillIfBlocked(skills, traitName);
            }
            return null;
        }

        private static readonly List<string> BlockedTraits = new List<string>
        {
            "bloodlust",
            "psychopath",
            "cannibal",
            "slow learner",
            "body purist",
            "misandrist",
            "misogynist",
            "slowpoke",
            "chemical fascination",
            "chemical interest",
            "depressive",
            "lazy",
            "slothful",
        };

        private static readonly Dictionary<string, PawnInfo> All = new Dictionary<string, PawnInfo>();
    }
}
