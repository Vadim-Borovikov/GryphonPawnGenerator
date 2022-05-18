using Verse;

namespace GryphonPawnGenerator
{
    internal sealed class TeamInfo
    {
        public readonly string Message;
        public readonly Pawn Worst;

        public TeamInfo(string message, Pawn worst)
        {
            Message = message;
            Worst = worst;
        }
    }
}
