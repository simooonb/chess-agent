using System.Collections.Generic;

namespace ChessAgent
{
    public class BishopChecker : MoveChecker
    {
        public override List<string> LegalMoves(string from, PieceColor color)
        {
            return GenerateOtherDiagPositions(from);
        }

        public override bool IsMoveLegal(string from, string to, PieceColor color)
        {
            return LegalMoves(from, color).Contains(to);
        }
    }
}