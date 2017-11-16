using System;
using System.Collections.Generic;

namespace ChessAgent
{
    public class PawnChecker : MoveChecker
    {
        public override List<string> LegalMoves(string from, PieceColor color)
        {
            var legalMoves = new List<string>();
            int nextPlay;
            string temp;
            var colIndex = Array.IndexOf(LegalColumns, from[0].ToString());
            
            // Prise en passant missing (not so dramatic)

            if (color.Equals(PieceColor.White))
            {
                // Next row
                nextPlay = int.Parse(from[1].ToString()) + 1;
                
                if (nextPlay <= 8)
                {
                    temp = string.Concat(from[0], nextPlay);
                    
                    // Check if the square is available
                    if (!OpponentPieces.Contains(temp))
                        legalMoves.Add(temp);

                    // Attacking pawn
                    if (colIndex < 7)
                    {
                        temp = string.Concat(LegalColumns[colIndex + 1], nextPlay);
                        
                        if (OpponentPieces.Contains(temp))
                            legalMoves.Add(temp);
                    }

                    if (colIndex > 0)
                    {
                        temp = string.Concat(LegalColumns[colIndex - 1], nextPlay);

                        if (OpponentPieces.Contains(temp))
                            legalMoves.Add(temp);
                    }
                }
                
                // First pawn move
                temp = string.Concat(from[0], 4);
                
                if (int.Parse(from[1].ToString()).Equals(2) && !OpponentPieces.Contains(temp))
                    legalMoves.Add(temp);
            }
            else if (color.Equals(PieceColor.Black))
            {
                // Next row
                nextPlay = int.Parse(from[1].ToString()) - 1;

                if (nextPlay > 0)
                {
                    temp = string.Concat(from[0], nextPlay);
                    
                    // Check if the square is available
                    if (!OpponentPieces.Contains(temp))
                        legalMoves.Add(temp);

                    // Attacking pawn
                    if (colIndex < 7)
                    {
                        temp = string.Concat(LegalColumns[colIndex + 1], nextPlay);
                        
                        if (OpponentPieces.Contains(temp))
                            legalMoves.Add(temp);
                    }

                    if (colIndex > 0)
                    {
                        temp = string.Concat(LegalColumns[colIndex - 1], nextPlay);
                        
                        if (OpponentPieces.Contains(temp))
                            legalMoves.Add(temp);
                    }
                }
                
                // First pawn move
                temp = string.Concat(from[0], 5);
                
                if (int.Parse(from[1].ToString()).Equals(7) && !OpponentPieces.Contains(temp))
                    legalMoves.Add(temp);
            }

            return legalMoves;
        }
        
        public override bool IsMoveLegal(string from, string to, PieceColor color)
        {
            return LegalMoves(from, color).Contains(to);
        }
    }
}