using System;
using System.Collections.Generic;
using System.Linq;

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
            _board = new Board(pieces);
        }

        /// <summary>
        /// Choose a move.
        /// </summary>
        /// <returns>The move chosen as a string array.</returns>
        public string[] ChooseMove()
        {
            var move = new[] { "", "", "D" };  // Queen promotion is almost always the best choice (good enough here)
            var legalMoves = _board.GenerateMovesFor(Color);
            
            var minimax = new MinimaxDecision<Board>(Evaluation.Evaluate);
            
            var rnd = new Random();
            var index = rnd.Next(legalMoves.Count);
            
            Console.WriteLine("position's score: " + Evaluation.Evaluate(_board));
            Console.WriteLine("legal moves count: " + legalMoves.Count);
            
            move[0] = legalMoves[index].From;  // From
            move[1] = legalMoves[index].To;  // To

            return move;
        }
    }
}