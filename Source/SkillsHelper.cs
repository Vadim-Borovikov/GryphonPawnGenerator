using System.Collections.Generic;
using System.Linq;
using RimWorld;

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

        public static void RegisterSkillIfBlocked(Dictionary<string, State> skills, string trait)
        {
            if (SkillBlockingTraits.ContainsKey(trait))
            {
                string skill = SkillBlockingTraits[trait];
                skills[skill] = State.Blocked;
            }
        }

        public static void FillSkills(IEnumerable<SkillRecord> source, Dictionary<string, State> target)
        {
            List<SkillRecord> proficientIn = source.Where(s => target[s.def.defName] != State.Blocked)
                                                   .Where(s => s.Level >= MinProficiencySkillLevel).ToList();
            foreach (SkillRecord skill in proficientIn)
            {
                string name = skill.def.defName;
                target[name] = State.Proficiency;
            }

            List<SkillRecord> passionateAbout = proficientIn.Where(s => s.passion != Passion.None).ToList();
            foreach (SkillRecord skill in passionateAbout)
            {
                string name = skill.def.defName;
                target[name] = State.Passion;
            }
        }

        public static int GetProficienciesAmount(Dictionary<string, State> skills)
        {
            return skills.Values.Count(s => s > State.None);
        }

        public static int GetPassionsAmount(Dictionary<string, State> skills)
        {
            return skills.Values.Count(s => s == State.Passion);
        }

        public static Dictionary<string, bool> GetPairSkills(Dictionary<string, State> skills1,
            Dictionary<string, State> skills2)
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();
            foreach (string skill in skills1.Keys)
            {
                result[skill] = (skills1[skill] > State.None) &&
                                (skills2[skill] > State.None) &&
                                (skills1[skill] is State.Passion || skills2[skill] is State.Passion);
            }
            return result;
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
