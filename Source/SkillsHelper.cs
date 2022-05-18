using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace GryphonPawnGenerator
{
    internal static class SkillsHelper
    {
        public enum State
        {
            Blocked = -1,
            None = 0,
            Proficiency = 1,
            Passion = 2
        }

        public static void RegisterSkillIfBlocked(PawnInfo info, string trait)
        {
            if (SkillBlockingTraits.ContainsKey(trait))
            {
                string skill = SkillBlockingTraits[trait];
                info.Skills[skill] = State.Blocked;
            }
        }

        public static void FillSkills(Pawn pawn, PawnInfo info)
        {
            List<SkillRecord> proficientIn = pawn.skills.skills
                                                 .Where(s => info.Skills[s.def.defName] != State.Blocked)
                                                 .Where(s => s.Level >= MinProficiencySkillLevel).ToList();
            foreach (SkillRecord skill in proficientIn)
            {
                string name = skill.def.defName;
                info.Skills[name] = State.Proficiency;
            }

            List<SkillRecord> passionateAbout = proficientIn.Where(s => s.passion != Passion.None).ToList();
            foreach (SkillRecord skill in passionateAbout)
            {
                string name = skill.def.defName;
                info.Skills[name] = State.Passion;
            }
        }

        private static readonly Dictionary<string, string> SkillBlockingTraits = new Dictionary<string, string>
        {
            { "brawler", "Shooting" },
            { "abrasive", "Social" },
            { "annoying voice", "Social" },
            { "creepy breathing", "Social" },
            { "ugly", "Social" },
            { "staggeringly ugly", "Social" }
        };

        private const int MinProficiencySkillLevel = 3;
    }
}
