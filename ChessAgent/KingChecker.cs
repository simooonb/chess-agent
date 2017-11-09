using System;
using System.Collections.Generic;

namespace ChessAgent
{
    public class KingChecker : MoveChecker
    {
        public List<string> LegalMoves(string from)
        {
            // TODO: Implement legal moves of king
            // TODO: Add castle
            return null;
        }
        
        public override bool IsMoveLegal(string from, string to)
        {
            // Checking column legality
            if (Math.Abs(from[1] - to[1]) > 1)
                return false;
            
            // Checking row legality
            if (Math.Abs(from[0] - to[1]) > 1)
                return false;

            return true;
        }
    }
}