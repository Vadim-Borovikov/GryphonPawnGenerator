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

        public static Dictionary<string, State> GetSkills(ICollection<SkillRecord> source)
        {
            Dictionary<string, State> result = new Dictionary<string, State>();

            foreach (string skill in source.Select(s => s.def.defName))
            {
                result[skill] = State.None;
            }

            List<SkillRecord> proficientIn = source.Where(s => s.Level >= MinProficiencySkillLevel).ToList();
            foreach (string skill in proficientIn.Select(s => s.def.defName))
            {
                result[skill] = State.Proficiency;
            }

            IEnumerable<SkillRecord> passionateAbout = proficientIn.Where(s => s.passion != Passion.None);
            foreach (string skill in passionateAbout.Select(s => s.def.defName))
            {
                result[skill] = State.Passion;
            }

            return result;
        }

        public static Dictionary<string, bool> GetPairSkills(Dictionary<string, State> skills1,
            Dictionary<string, State> skills2)
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();
            foreach (string skill in skills1.Keys)
            {
                result[skill] = skills1[skill] is State.Passion || skills2[skill] is State.Passion;
                if (skill != "Melee")
                {
                    result[skill] &= (skills1[skill] > State.None) && (skills2[skill] > State.None);
                }
            }
            return result;
        }

        public static int GetCompatitiveValue(Dictionary<string, State> skills, List<Dictionary<string, State>> others)
        {
            int result = 0;
            foreach (string skill in skills.Keys)
            {
                switch (skills[skill])
                {
                    case State.Blocked:
                    case State.None:
                        continue;
                    case State.Proficiency:
                        foreach (Dictionary<string, State> set in others)
                        {
                            if ((skill != "Melee") && (set[skill] < State.Proficiency))
                            {
                                ++result;
                            }
                        }
                        break;
                    case State.Passion:
                        foreach (Dictionary<string, State> set in others)
                        {
                            if (((skill == "Melee") && (set[skill] < State.Passion))
                                || (set[skill] < State.Proficiency))
                            {
                                result += 2;
                            }
                            else if (set[skill] == State.Proficiency)
                            {
                                ++result;
                            }
                        }
                        break;
                }
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
