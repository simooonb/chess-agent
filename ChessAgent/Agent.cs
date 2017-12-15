namespace ChessAgent
{
    public class Agent
    {
        private Board _board;
        public PieceColor Color { get; private set; }

        public Agent(PieceColor color)
        {
            Color = color;
        }

        /// <summary>
        /// Update inner state based on environment's input.
        /// </summary>
        /// <param name="pieces">The piecse of the board.</param>
        public void ObserveEnvironmentAndUpdateState(int[] pieces)
        {
            _board = new Board(pieces, Color);
        }

        /// <summary>
        /// Choose a move.
        /// </summary>
        /// <returns>The move chosen as a string array.</returns>
        public string[] ChooseMove()
        {
            var moveArray = new[] { "", "", "D" };  // Queen promotion is almost always the best choice (good enough here)
            var eval = new Evaluation(Color);

            var minimax = new MinimaxDecision(eval.Evaluate);
            var move = minimax.MinimaxIterativeDeepening(_board, true);

            moveArray[0] = move.From;  // From
            moveArray[1] = move.To;  // To

            return moveArray;
        }
    }
}