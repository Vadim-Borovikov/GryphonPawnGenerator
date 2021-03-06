using System.Collections.Generic;
using System.Linq;
using Verse;

namespace GryphonPawnGenerator
{
    internal sealed class GroupInfo
    {
        private readonly IList<Pawn> _members;
        private readonly Dictionary<string, bool> _skills;
        private readonly string _names;
        private readonly decimal _competency;

        private GroupInfo(IList<Pawn> pawns)
        {
            _members = pawns;
            if (_members.Count < 2)
            {
                _competency = 0;
                return;
            }

            if (_members.Count == 2)
            {
                PawnInfo info1 = PawnInfo.GetOrCreate(_members[0]);
                PawnInfo info2 = PawnInfo.GetOrCreate(_members[1]);
                _skills = SkillsHelper.GetPairSkills(info1.Skills, info2.Skills);
            }
            else
            {
                List<GroupInfo> pairInfos = new List<GroupInfo>();
                for (int i = 0; i < (_members.Count - 1); ++i)
                {
                    for (int j = i + 1; j < _members.Count; ++j)
                    {
                        List<Pawn> pair = new List<Pawn>
                        {
                            _members[i],
                            _members[j]
                        };
                        GroupInfo info = GetOrCreate(pair);
                        pairInfos.Add(info);
                    }
                }

                _skills = new Dictionary<string, bool>();
                foreach (string skill in pairInfos[0]._skills.Keys)
                {
                    _skills[skill] = pairInfos.Any(p => p._skills[skill]);
                }
            }

            _names = string.Join(", ", _members.Select(p => p.Name.ToStringShort).OrderBy(n => n));
            _competency = _skills.Values.Count(o => o) * 1m / _skills.Count;
        }

        public static TeamInfo GetTeamInfo()
        {
            List<Pawn> team = Find.GameInitData.startingAndOptionalPawns;
            GroupInfo best = GetBestTeam(team);
            string message;
            if (best is null)
            {
                message = "No team found";
                return new TeamInfo(message, null);
            }

            if (best._competency < 1)
            {
                Pawn worst = GetWorstPawn(team);
                PawnInfo info = PawnInfo.GetOrCreate(worst);
                message = $"Best team with {best._competency:0%}: {best._names}. {info.Name} should go.";
                return new TeamInfo(message, worst);
            }

            message = $"Perfect team found! {best._names}";
            return new TeamInfo(message, null);
        }

        private static GroupInfo GetBestTeam(IReadOnlyCollection<Pawn> set)
        {
            if (set.Count < TeamSize)
            {
                return null;
            }

            GroupInfo result = GetOrCreate(set.Take(TeamSize).ToList());
            if (set.Count == TeamSize)
            {
                return result;
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Pawn candidate in set.Skip(TeamSize))
            {
                result = result.Try(candidate);
            }

            return result;
        }

        private static Pawn GetWorstPawn(ICollection<Pawn> set)
        {
            List<PawnInfo> infos = set.Select(PawnInfo.GetOrCreate).ToList();
            Pawn worst = null;
            int worstValue = -1;

            foreach (Pawn pawn in set)
            {
                PawnInfo info = PawnInfo.GetOrCreate(pawn);
                int value = SkillsHelper.GetCompatitiveValue(info.Skills, infos.Where(i => i != info)
                                                                               .Select(i => i.Skills)
                                                                               .ToList());
                if (worst is null || (value < worstValue))
                {
                    worst = pawn;
                    worstValue = value;
                }
            }

            return worst;
        }

        private GroupInfo Try(Pawn candidate)
        {
            GroupInfo best = this;

            foreach (Pawn member in _members)
            {
                List<Pawn> newGroup = new List<Pawn>(_members);
                newGroup.Remove(member);
                newGroup.Add(candidate);
                GroupInfo info = GetOrCreate(newGroup);
                if (info._competency > best._competency)
                {
                    best = info;
                }
            }

            return best;
        }

        private static GroupInfo GetOrCreate(IList<Pawn> pawns)
        {
            if (pawns is null)
            {
                return null;
            }

            string names = GetKey(pawns);
            if (!All.ContainsKey(names))
            {
                All[names] = new GroupInfo(pawns);
            }
            return All[names];
        }

        private static string GetKey(IEnumerable<Pawn> pawns)
        {
            return string.Join(" ", pawns.Select(p => p.ThingID).OrderBy(n => n));
        }

        public static void Remove(Pawn pawn)
        {
            if (pawn is null)
            {
                return;
            }

            All.RemoveAll(p => p.Value._members.Contains(pawn));
        }

        private static readonly Dictionary<string, GroupInfo> All = new Dictionary<string, GroupInfo>();
        private const int TeamSize = 3;
    }
}
