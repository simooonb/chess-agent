using System;
using System.Collections.Generic;
using System.Timers;

namespace ChessAgent
{
    public class MinimaxDecision
    {
        private DirectedWeightedGraph<Board> _graph;
        private static bool _keepComputing = true;
        
        private readonly Timer _timer = new Timer();
        private readonly Func<Board, int> _heuristic;

        private const int PositiveInf = int.MaxValue;
        private const int NegativeInf = int.MinValue;
        private const int MaxComputationTimeInMilliseconds = 225;
        
        public MinimaxDecision(Func<Board, int> heuristicCostEstimate)
        {
            if (heuristicCostEstimate == null)
                throw new ArgumentException("The heuristic function is null");
            
            _heuristic = heuristicCostEstimate;
            _timer.Interval = MaxComputationTimeInMilliseconds;
            _timer.Elapsed += OnTimedEvent;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _keepComputing = false;
        }

//        public void InitialiseGraph(Board source)
//        {
//            _graph = new DirectedWeightedGraph<Board>();
//            _graph.AddNode(source);
//
//            var nodes = source.GenerateBoardsFromMoves(source.GenerateNextMoves());
//            _graph.AddNodes(nodes);
//            _outerFrontier.AddRange(nodes);
//            
//            foreach (var node in _graph.Nodes)
//            {
//                if (node.Equals(source)) continue;
//
//                _graph.AddEdge(source, node, 1);
//            }
//        }

//        /// <summary>
//        /// Compute the next best move, based on the game's graph.
//        /// </summary>
//        /// <param name="graph">The graph of the game.</param>
//        /// <param name="source">The actual game state.</param>
//        /// <returns></returns>
//        public void IterativeDeepeningAlphaBeta(DirectedWeightedGraph<Board> graph, Board source)
//        {
//            _graph = graph;
//            var depth = 1;
//
//            _timer.Enabled = true;
//            
//            // TODO: Implement iterative deepening
//            // TODO: Implement transposition table
//            // TODO: Return a move
//
//            do
//            {
//                Minimax(source, depth, NegativeInf, PositiveInf, true);
//            } while (_keepComputing);
//
//            _timer.Enabled = false;
//        }

        public Move MinimaxRoot(int depth, Board game, bool maximizingPlayer)
        {
            var newMoves = game.GenerateNextMoves();
            var bestMoveValue = NegativeInf;
            Move bestMoveFound = null;

            foreach (var newMove in newMoves)
            {
                game.PlayMove(newMove);
                var value = Minimax(game, depth - 1, NegativeInf, PositiveInf, !maximizingPlayer);
                game.Undo();

                if (value > bestMoveValue)
                {
                    bestMoveValue = value;
                    bestMoveFound = newMove;
                }
            }

            return bestMoveFound;
        }

        private int Minimax(Board node, int depth, int alpha, int beta, bool maximizingPlayer)
        {
            // Stop condition for recursion
            if (depth <= 0)
                return _heuristic(node);

            int bestValue;
            var newMoves = node.GenerateNextMoves();

            if (maximizingPlayer)  // Evaluating as the maximizing player
            {
                bestValue = NegativeInf;

                foreach (var newMove in newMoves)
                {
                    node.PlayMove(newMove);
                    bestValue = Math.Max(bestValue, Minimax(node, depth - 1, alpha, beta, false));
                    node.Undo();
                    
                    alpha = Math.Max(alpha, bestValue);

                    if (beta <= alpha)
                        break; // Beta cut-off
                }

                return bestValue;
            }
            else  // Evaluating as the minimizing player
            {
                bestValue = PositiveInf;

                foreach (var newMove in newMoves)
                {
                    node.PlayMove(newMove);
                    bestValue = Math.Min(bestValue, Minimax(node, depth - 1, alpha, beta, true));
                    node.Undo();
                    
                    beta = Math.Min(beta, bestValue);

                    if (beta <= alpha)
                        break; // Alpha cut-off
                }

                return bestValue;
            }
        }        
    }
}