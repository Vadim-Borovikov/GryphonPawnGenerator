using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace GryphonPawnGenerator
{
    internal static class PawnChecker
    {
        public static bool Check(Pawn pawn, out string error)
        {
            ScanTraits(pawn);

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

        private static void ScanTraits(Pawn pawn)
        {
            PawnInfo info = PawnInfo.GetOrCreate(pawn);
            _traitError = null;

            foreach (Trait trait in pawn.story.traits.allTraits)
            {
                TraitDegreeData degreeData = trait.Degree == 0
                    ? trait.def.degreeDatas.FirstOrDefault()
                    : trait.def.degreeDatas.FirstOrDefault(d => d.degree == trait.Degree);
                if (degreeData is null)
                {
                    _traitError = $"no degree data in trait ({trait.def.defName})!";
                    return;
                }

                string traitName = degreeData.untranslatedLabel;
                if (BlockedTraits.Contains(traitName))
                {
                    _traitError = trait.def.degreeDatas.Count < 2
                        ? trait.def.defName
                        : $"{trait.def.defName} - {traitName}";
                    return;
                }

                SkillsHelper.RegisterSkillIfBlocked(info, traitName);
            }
        }

        private static string CheckTraits(Pawn pawn) => _traitError;

        private static string CheckSkills(Pawn pawn)
        {
            PawnInfo info = PawnInfo.GetOrCreate(pawn);
            SkillsHelper.FillSkills(pawn, info);

            int proficiencies = info.Skills.Values.Count(s => s != SkillsHelper.State.None);
            if (proficiencies < MinProficiencies)
            {
                return $"skills ({proficiencies} proficiencies only)";
            }

            int passions = info.Skills.Values.Count(s => s == SkillsHelper.State.Passion);
            if (passions < MinPassions)
            {
                return $"skills ({passions} passions only)";
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

        private const int MinProficiencies = 8;
        private const int MinPassions = 4;

        private static string _traitError;
    }
}
