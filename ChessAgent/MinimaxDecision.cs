using System;
using System.Timers;

namespace ChessAgent
{
    public class MinimaxDecision<T>
    {
        private DirectedWeightedGraph<T> _graph;
        private static bool _keepComputing = true;
        
        private readonly Timer _timer = new Timer();
        private readonly Func<T, int> _heuristic;

        private const int PositiveInf = int.MaxValue;
        private const int NegativeInf = int.MinValue;
        private const int MaxComputationTimeInMilliseconds = 200;
        
        public MinimaxDecision(Func<T, int> heuristicCostEstimate)
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

        /// <summary>
        /// Compute the next best move, based on the game's graph.
        /// </summary>
        /// <param name="graph">The graph of the game.</param>
        /// <param name="source">The actual game state.</param>
        /// <returns></returns>
        public void IterativeDeepeningAlphaBeta(DirectedWeightedGraph<T> graph, T source)
        {
            _graph = graph;
            var depth = 1;

            _timer.Enabled = true;
            
            // TODO: Implement iterative deepening
            // TODO: Implement transposition table
            // TODO: Return a move

            do
            {
                AlphaBeta(source, depth, NegativeInf, PositiveInf, true);
            } while (_keepComputing);

            _timer.Enabled = false;
        }

        private int AlphaBeta(T node, int depth, int alpha, int beta, bool maximizingPlayer)
        {
            // Stop condition for recursion
            if (depth <= 0 || _graph.Neighbors(node).Count <= 0)
                return _heuristic(node);

            int bestValue;

            if (maximizingPlayer)  // Evaluating as the maximizing player
            {
                bestValue = NegativeInf;

                foreach (var child in _graph.Neighbors(node))
                {
                    bestValue = Math.Max(bestValue, AlphaBeta(child, depth - 1, alpha, beta, false));
                    alpha = Math.Max(alpha, bestValue);

                    if (beta <= alpha)
                        break; // Beta cut-off
                }

                return bestValue;
            }
            else  // Evaluating as the minimizing player
            {
                bestValue = PositiveInf;

                foreach (var child in _graph.Neighbors(node))
                {
                    bestValue = Math.Min(bestValue, AlphaBeta(child, depth - 1, alpha, beta, true));
                    beta = Math.Min(beta, bestValue);

                    if (beta <= alpha)
                        break; // Alpha cut-off
                }

                return bestValue;
            }
        }
    }
}