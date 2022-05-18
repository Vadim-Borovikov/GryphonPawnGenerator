using System.Collections.Generic;
using System.Linq;
using Verse;

namespace GryphonPawnGenerator
{
    internal sealed class GroupInfo
    {
        public readonly string Names;
        public readonly decimal Competency;

        private readonly IList<Pawn> _members;
        private readonly Dictionary<string, bool> _skills = new Dictionary<string, bool>();

        private GroupInfo(IList<Pawn> pawns)
        {
            _members = pawns;
            if (_members.Count < 2)
            {
                Competency = 0;
                return;
            }

            if (_members.Count == 2)
            {
                PawnInfo info1 = PawnInfo.GetOrCreate(_members[0]);
                PawnInfo info2 = PawnInfo.GetOrCreate(_members[1]);
                foreach (string skill in info1.Skills.Keys)
                {
                    _skills[skill] = (info1.Skills[skill] > SkillsHelper.State.None)
                                    && (info2.Skills[skill] > SkillsHelper.State.None)
                                    && (info1.Skills[skill] is SkillsHelper.State.Passion || info2.Skills[skill] is SkillsHelper.State.Passion);
                }
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

                foreach (string skill in pairInfos[0]._skills.Keys)
                {
                    _skills[skill] = pairInfos.Any(p => p._skills[skill]);
                }
            }

            Names = string.Join(", ", _members.Select(p => p.Name.ToStringShort).OrderBy(n => n));
            Competency = _skills.Values.Count(o => o) * 1m / _skills.Count;
        }

        public GroupInfo Try(Pawn candidate)
        {
            GroupInfo best = this;

            foreach (Pawn member in _members)
            {
                List<Pawn> newGroup = new List<Pawn>(_members);
                newGroup.Remove(member);
                newGroup.Add(candidate);
                GroupInfo info = GetOrCreate(newGroup);
                if (info.Competency > best.Competency)
                {
                    best = info;
                }
            }

            return best;
        }

        public static GroupInfo GetOrCreate(IList<Pawn> pawns)
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
    }
}
