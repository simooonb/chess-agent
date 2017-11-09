using System.Collections.Generic;

namespace ChessAgent
{
    public class PawnChecker : MoveChecker
    {
        public List<string> LegalMoves(string from)
        {
            // TODO: Implement legal moves
            // TODO: Check for color
            return null;
        }
        
        public override bool IsMoveLegal(string from, string to)
        {
            throw new System.NotImplementedException();
        }
    }
}