using System.Collections.Generic;
using System.Linq;

namespace ChessAgent
{
    public class QueenChecker : MoveChecker
    {
        public override List<string> LegalMoves(string @from, PieceColor color, List<string> opponentPieces)
        {
            return GenerateOtherColumnPositions(from)
                .Union(GenerateOtherRowPositions(from))
                .Union(GenerateOtherDiagPositions(from))
                .ToList();
        }
        
        public override bool IsMoveLegal(string from, string to, PieceColor color, List<string> opponentPieces)
        {
            return LegalMoves(@from, color, opponentPieces).Contains(to);
        }
    }
}