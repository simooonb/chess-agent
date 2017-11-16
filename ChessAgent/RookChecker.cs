using System.Collections.Generic;
using System.Linq;

namespace ChessAgent
{
    public class RookChecker : MoveChecker
    {
        public override List<string> LegalMoves(string from, PieceColor color)
        {
            return GenerateOtherColumnPositions(from)
                .Union(GenerateOtherRowPositions(from))
                .ToList();
        }
        
        public override bool IsMoveLegal(string from, string to, PieceColor color)
        {
            return LegalMoves(from, color).Contains(to);
        }
    }
}