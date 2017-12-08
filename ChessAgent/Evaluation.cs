namespace ChessAgent
{
    public class Evaluation
    {       
        /**
         * Constants (such as weights).
         */

        private const int MaterialWeight = 1;
        private const int QueenDiffWeight = 9;
        private const int RookDiffWeight = 5;
        private const int KnightDiffWeight = 3;
        private const int BishopDiffWeight = 3;
        private const int PawnDiffWeight = 1;
        
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

        /// <summary>
        /// Evaluates the position, always according the the white.
        /// A positive score indicates the position is better for the white pieces,
        /// a negative one indicates the position is better for the black pieces.
        /// </summary>
        /// <returns>The position's score.</returns>
        public static int Evaluate(Board board)
        {
            return MaterialScore(board) * MaterialWeight;
        }
    }
}