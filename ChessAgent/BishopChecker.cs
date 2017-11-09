using System.Collections.Generic;
using System.Linq;

namespace ChessAgent
{
    public class BishopChecker : MoveChecker
    {
        public List<string> LegalMoves(string from)
        {
            return GenerateOtherDiagPositions(from);
        }

        public override bool IsMoveLegal(string from, string to)
        {
            return LegalMoves(from).Contains(to);
        }
    }
}