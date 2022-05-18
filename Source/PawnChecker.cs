using System;
using System.Collections.Generic;
using Verse;

namespace GryphonPawnGenerator
{
    internal static class PawnChecker
    {
        public static bool Check(Pawn pawn, out string error)
        {
            List<Func<Pawn, string>> checks = new List<Func<Pawn, string>>
            {
                CheckStory,
                CheckWork,
                CheckTraits,
                CheckSkills,
                CheckHealth
            };

            foreach (Func<Pawn, string> check in checks)
            {
                error = check(pawn);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return false;
                }
            }

            error = null;
            return true;
        }

        private static string CheckStory(Pawn pawn) => pawn.story.adulthood is null ? "story (no adulthood)" : null;

        private static string CheckWork(Pawn pawn)
        {
            foreach (WorkTypeDef disabledWorkType in pawn.GetDisabledWorkTypes(true))
            {
                foreach (WorkTags neededWorkTag in NeededWorkTags)
                {
                    if (AreOverlapping(disabledWorkType.workTags, neededWorkTag))
                    {
                        return $"work (disabled {neededWorkTag})";
                    }
                }
            }

            return null;
        }

        private static string CheckTraits(Pawn pawn)
        {
            PawnInfo info = PawnInfo.GetOrCreate(pawn);
            return info.TraitError;
        }

        private static string CheckSkills(Pawn pawn)
        {
            PawnInfo info = PawnInfo.GetOrCreate(pawn);

            if ((info.Skills["Shooting"] <= SkillsHelper.State.None) &&
                (info.Skills["Melee"] <= SkillsHelper.State.None))
            {
                return "skills (can't fight properly)";
            }

            return null;
        }

        private static string CheckHealth(Pawn pawn)
        {
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                HediffWithComps hediffWithComps = hediff as HediffWithComps;
                HediffComp_SeverityPerDay severityPerDay = hediffWithComps?.TryGetComp<HediffComp_SeverityPerDay>();
                if (severityPerDay is null || (severityPerDay.SeverityChangePerDay() >= 0.0f))
                {
                    return $"health ({hediff.def.defName})";
                }
            }

            return null;
        }

        private static bool AreOverlapping(WorkTags tag1, WorkTags tag2) => (tag1 & tag2) != 0;

        private static readonly List<WorkTags> NeededWorkTags = new List<WorkTags>
        {
            WorkTags.Firefighting,
            WorkTags.Hauling
        };

        // private const int MinProficiencies = 8;
        // private const int MinPassions = 4;
    }
}
