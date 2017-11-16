using System;
using System.Collections.Generic;

namespace ChessAgent
{
    public class KnightChecker : MoveChecker
    {
        public override List<string> LegalMoves(string from, PieceColor color)
        {
            var legalMoves = new List<string>();
            var rowIndex = Array.IndexOf(LegalRows, from[1].ToString());
            var colIndex = Array.IndexOf(LegalColumns, from[0].ToString());
            var row = int.Parse(LegalRows[rowIndex]);

            // Iterating through each quarter
            
            if (row < 7)
            {
                if (colIndex > 0)
                    legalMoves.Add(string.Concat(LegalColumns[colIndex - 1], row + 2));
                
                if (colIndex < 7)
                    legalMoves.Add(string.Concat(LegalColumns[colIndex + 1], row + 2));
            }
            
            if (row < 8)
            {
                if (colIndex > 1)
                    legalMoves.Add(string.Concat(LegalColumns[colIndex - 2], row + 1));
                
                if (colIndex < 6)
                    legalMoves.Add(string.Concat(LegalColumns[colIndex + 2], row + 1));
            }

            if (row > 1)
            {
                if (colIndex > 1)
                    legalMoves.Add(string.Concat(LegalColumns[colIndex - 2], row - 1));
                
                if (colIndex < 6)
                    legalMoves.Add(string.Concat(LegalColumns[colIndex + 2], row - 1));
            }
            
            if (row > 2)
            {
                if (colIndex > 0)
                    legalMoves.Add(string.Concat(LegalColumns[colIndex - 1], row - 2));
                
                if (colIndex < 7)
                    legalMoves.Add(string.Concat(LegalColumns[colIndex + 1], row - 2));
            }
            
            return legalMoves;
        }
        
        public override bool IsMoveLegal(string from, string to, PieceColor color)
        {
            return LegalMoves(from, color).Contains(to);
        }
    }
}