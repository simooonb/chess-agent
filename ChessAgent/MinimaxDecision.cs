using System;

namespace ChessAgent
{
    public class MinimaxDecision<T>
    {
        private DirectedWeightedGraph<T> _graph;
        private readonly Func<T, int> _heuristic;

        private const int PositiveInf = int.MaxValue;
        private const int NegativeInf = int.MinValue;
        
        public MinimaxDecision(Func<T, int> heuristicCostEstimate)
        {
            if (heuristicCostEstimate == null)
                throw new ArgumentException("The heuristic function is null");
            
            _heuristic = heuristicCostEstimate;
        }

        public int IterativeDeepeningAlphaBeta(DirectedWeightedGraph<T> graph, T source)
        {
            _graph = graph;
            var maxDepth = 5;
            
            // TODO: Implement iterative deepening

            return AlphaBeta(source, maxDepth, NegativeInf, PositiveInf, true);
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