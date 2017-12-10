using System;
using System.Diagnostics;
using System.Linq;

namespace ChessAgent
{
    public static class Evaluation
    {       
        /**
         * Constants.
         */

        // Weights
        
        private const int MaterialWeight = 3;
        private const int PositionWeight = 1;
        private const int MobilityWeight = 2;
        
        private const int QueenDiffWeight = 9;
        private const int RookDiffWeight = 5;
        private const int KnightDiffWeight = 3;
        private const int BishopDiffWeight = 3;
        private const int PawnDiffWeight = 1;
        
        // Position bonus
        
        private static readonly int[] KnightPosition = 
        {
            -10,-8, -6, -6, -6, -6, -8,-10,
            -8, -4,  0,  1,  1,  0, -4, -8,
            -6,  0,  2,  3,  3,  2,  0, -6,
            -6,  1,  3,  4,  4,  3,  1, -6,
            -6,  1,  3,  4,  4,  3,  1, -6,
            -6,  0,  2,  3,  3,  2,  0, -6,
            -8, -4,  0,  1,  1,  0, -4, -8,
            -10,-8, -6, -6, -6, -6, -8,-10
        };
        
        private static readonly int[] QueenPosition =
        {
            -4, -2, -2, -1, -1, -2, -2, -4,
            -2,  0,  0,  0,  0,  0,  0, -2,
            -2,  0,  1,  1,  1,  1,  0, -2,
            -1,  0,  1,  1,  1,  1,  0, -1,
            -1,  0,  1,  1,  1,  1,  0, -1,
            -2,  0,  1,  1,  1,  1,  0, -2,
            -2,  0,  0,  0,  0,  0,  0, -2,
            -4, -2, -2, -1, -1, -2, -2, -4
        };

        private static readonly int[] WhitePawnPosition =
        {
            0,  0,  0,  0,  0,  0,  0,  0,
            10, 10, 10, 10, 10, 10, 10, 10,
            2,  2,  4,  6,  6,  4,  2,  2,
            1,  1,  2,  5,  5,  2,  1,  1,
            0,  0,  0,  4,  4,  0,  0,  0,
            1, -1, -2,  0,  0, -2, -1,  1,
            1,  2,  2, -4, -4,  2,  2,  2,
            0,  0,  0,  0,  0,  0,  0,  0
        };
        
        private static readonly int[] WhiteBishopPosition =
        {
            -4, -2, -2, -2, -2, -2, -2, -4,
            -2,  0,  0,  0,  0,  0,  0, -2,
            -2,  0,  1,  2,  2,  1,  0, -2,
            -2,  1,  1,  2,  2,  1,  1, -2,
            -2,  0,  2,  2,  2,  2,  0, -2,
            -2,  2,  2,  2,  2,  2,  2, -2,
            -2,  1,  0,  0,  0,  0,  1, -2,
            -4, -2, -2, -2, -2, -2, -2, -4
        };
        
        private static readonly int[] WhiteRookPosition =
        {
            0,  0,  0,  0,  0,  0,  0,  0,
            1,  2,  2,  2,  2,  2,  2,  1,
           -1,  0,  0,  0,  0,  0,  0, -1,
           -1,  0,  0,  0,  0,  0,  0, -1,
           -1,  0,  0,  0,  0,  0,  0, -1,
           -1,  0,  0,  0,  0,  0,  0, -1,
           -1,  0,  0,  0,  0,  0,  0, -1,
            0,  0,  0,  1,  1,  0,  0,  0
        };

        private static readonly int[] WhiteKingPosition =
        {
            -6, -8, -8,-10,-10, -8, -8, -6,
            -6, -8, -8,-10,-10, -8, -8, -6,
            -6, -8, -8,-10,-10, -8, -8, -6,
            -6, -8, -8,-10,-10, -8, -8, -6,
            -2, -6, -6, -8, -8, -6, -6, -2,
            -2, -4, -4, -4, -4, -4, -4, -2,
             4,  4,  0,  0,  0,  0,  4,  4,
             4,  6,  2,  0,  0,  2,  6,  4
        };

        private static readonly int[] BlackPawnPosition =
        {
            0,  0,  0,  0,  0,  0,  0,  0,
            1,  2,  2, -4, -4,  2,  2,  2,
            1, -1, -2,  0,  0, -2, -1,  1,
            0,  0,  0,  4,  4,  0,  0,  0,
            1,  1,  2,  5,  5,  2,  1,  1,
            2,  2,  4,  6,  6,  4,  2,  2,
            10, 10, 10, 10, 10, 10, 10, 10,
            0,  0,  0,  0,  0,  0,  0,  0
        };

        private static readonly int[] BlackBishopPosition =
        {
            -4, -2, -2, -2, -2, -2, -2, -4,
            -2,  1,  0,  0,  0,  0,  1, -2,
            -2,  2,  2,  2,  2,  2,  2, -2,
            -2,  0,  2,  2,  2,  2,  0, -2,
            -2,  1,  1,  2,  2,  1,  1, -2,
            -2,  0,  1,  2,  2,  1,  0, -2,
            -2,  0,  0,  0,  0,  0,  0, -2,
            -4, -2, -2, -2, -2, -2, -2, -4
        };

        private static readonly int[] BlackRookPosition = 
        {
            0,  0,  0,  1,  1,  0,  0,  0,
            -1,  0,  0,  0,  0,  0,  0, -1,
            -1,  0,  0,  0,  0,  0,  0, -1,
            -1,  0,  0,  0,  0,  0,  0, -1,
            -1,  0,  0,  0,  0,  0,  0, -1,
            -1,  0,  0,  0,  0,  0,  0, -1,
            0,  0,  0,  0,  0,  0,  0,  0,
            1,  2,  2,  2,  2,  2,  2,  1
        };

        private static readonly int[] BlackKingPosition =
        {
            4,  6,  2,  0,  0,  2,  6,  4,
            4,  4,  0,  0,  0,  0,  4,  4,
            -2, -4, -4, -4, -4, -4, -4, -2,
            -2, -6, -6, -8, -8, -6, -6, -2,
            -6, -8, -8,-10,-10, -8, -8, -6,
            -6, -8, -8,-10,-10, -8, -8, -6,
            -6, -8, -8,-10,-10, -8, -8, -6,
            -6, -8, -8,-10,-10, -8, -8, -6
        };
        
        /**
         * Intermediate scores.
         */

        private static int MaterialScore(Board b)
        {
            return (b.WhiteQueensCount - b.BlackQueensCount)   * QueenDiffWeight +
                   (b.WhiteRooksCount - b.BlackRooksCount)     * RookDiffWeight +
                   (b.WhiteBishopsCount - b.BlackBishopsCount) * BishopDiffWeight +
                   (b.WhiteKnightsCount - b.BlackKnightsCount) * KnightDiffWeight +
                   (b.WhitePawnsCount - b.BlackPawnsCount)     * PawnDiffWeight;
        }

        private static int PositionScore(Board b)
        {            
            var whitePawnList = b.BitboardToStringList(b.WhitePawns);
            var whiteKnightList = b.BitboardToStringList(b.WhiteKnights);
            var whiteBishopList = b.BitboardToStringList(b.WhiteBishops);
            var whiteRookList = b.BitboardToStringList(b.WhiteRooks);
            var whiteQueenList = b.BitboardToStringList(b.WhiteQueens);
            
            var blackPawnList = b.BitboardToStringList(b.BlackPawns);
            var blackKnightList = b.BitboardToStringList(b.BlackKnights);
            var blackBishopList = b.BitboardToStringList(b.BlackBishops);
            var blackRookList = b.BitboardToStringList(b.BlackRooks);
            var blackQueenList = b.BitboardToStringList(b.BlackQueens);

            var kingScore = 0;

            if (b.WhiteKingCount != 0)
                kingScore += WhiteKingPosition[Array.IndexOf(b.EnumSquare, b.BitboardToStringList(b.WhiteKing)[0])];
            else if (b.BlackKingCount != 0)
                kingScore -= BlackKingPosition[Array.IndexOf(b.EnumSquare, b.BitboardToStringList(b.BlackKing)[0])];
            
            var pawnScore = whitePawnList.Sum(pawn => WhitePawnPosition[Array.IndexOf(b.EnumSquare, pawn)]) -
                            blackPawnList.Sum(pawn => BlackPawnPosition[Array.IndexOf(b.EnumSquare, pawn)]);

            var knightScore = whiteKnightList.Sum(knight => KnightPosition[Array.IndexOf(b.EnumSquare, knight)]) -
                              blackKnightList.Sum(knight => KnightPosition[Array.IndexOf(b.EnumSquare, knight)]);

            var bishopScore = whiteBishopList.Sum(bishop => WhiteBishopPosition[Array.IndexOf(b.EnumSquare, bishop)]) -
                              blackBishopList.Sum(bishop => BlackBishopPosition[Array.IndexOf(b.EnumSquare, bishop)]);

            var rookScore = whiteRookList.Sum(rook => WhiteRookPosition[Array.IndexOf(b.EnumSquare, rook)]) -
                            blackRookList.Sum(rook => BlackRookPosition[Array.IndexOf(b.EnumSquare, rook)]);

            var queenScore = whiteQueenList.Sum(queen => QueenPosition[Array.IndexOf(b.EnumSquare, queen)]) -
                             blackQueenList.Sum(queen => QueenPosition[Array.IndexOf(b.EnumSquare, queen)]);

            return kingScore + pawnScore + knightScore + bishopScore + rookScore + queenScore;
        }

        private static int MobilityScore(Board b)
        {
            return b.WhitePiecesMobility - b.BlackPiecesMobility;
        }

        /// <summary>
        /// Evaluates the position, always according the the white.
        /// A positive score indicates the position is better for the white pieces,
        /// a negative one indicates the position is better for the black pieces.
        /// </summary>
        /// <returns>The position's score.</returns>
        public static int Evaluate(Board board)
        {
            return MaterialScore(board) * MaterialWeight +
                   PositionScore(board) * PositionWeight +
                   MobilityScore(board) * MobilityWeight;
        }
    }
}