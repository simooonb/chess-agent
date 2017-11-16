using System;
using System.Collections.Generic;

namespace ChessAgent
{
    public class KingChecker : MoveChecker
    {
        public bool HasMoved = false;
       
        public override List<string> LegalMoves(string from, PieceColor color)
        {
            // TODO: Add castle
            // TODO: Check opponent's king position
            
            var legalMoves = new List<string>();
            var rowIndex = Array.IndexOf(LegalRows, from[1].ToString());
            var colIndex = Array.IndexOf(LegalColumns, from[0].ToString());
            var row = int.Parse(LegalRows[rowIndex]);
            
            // Iterating through each possibility

            if (row < 8)
                legalMoves.Add(string.Concat(from[0], row + 1));
            
            if (row > 1)
                legalMoves.Add(string.Concat(from[0], row - 1));
            
            if (colIndex > 0)
                legalMoves.Add(string.Concat(LegalColumns[colIndex - 1], row));
            
            if (colIndex < 7)
                legalMoves.Add(string.Concat(LegalColumns[colIndex + 1], row));
            
            if (row < 8 && colIndex > 0)
                legalMoves.Add(string.Concat(LegalColumns[colIndex - 1], row + 1));
            
            if (row < 8 && colIndex < 7)
                legalMoves.Add(string.Concat(LegalColumns[colIndex + 1], row + 1));
            
            if (row > 1 && colIndex > 0)
                legalMoves.Add(string.Concat(LegalColumns[colIndex - 1], row - 1));
            
            if (row > 1 && colIndex < 7)
                legalMoves.Add(string.Concat(LegalColumns[colIndex + 1], row - 1));
            
            return legalMoves;
        }
        
        public override bool IsMoveLegal(string from, string to, PieceColor color)
        {
            return Math.Abs(from[1] - to[1]) <= 1 && Math.Abs(from[0] - to[1]) <= 1;
        }
    }
}