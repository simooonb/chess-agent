using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessAgent
{
    public class QueenChecker : MoveChecker
    {
        public List<string> LegalMoves(string from)
        {
            return GenerateOtherColumnPositions(from)
                .Union(GenerateOtherRowPositions(from))
                .Union(GenerateOtherDiagPositions(from))
                .ToList();
        }
        
        public override bool IsMoveLegal(string from, string to)
        {
            return LegalMoves(from).Contains(to);
        }
    }
}