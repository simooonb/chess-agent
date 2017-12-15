using System;
using System.Timers;

namespace ChessAgent
{
    public class MinimaxDecision
    {
        private readonly TranspositionTable _tt = new TranspositionTable();
        private static bool _keepComputing = true;
        private Move _oldBestMove;
        
        private readonly Timer _timer = new Timer();
        private readonly Func<Board, int> _heuristic;

        private const int PositiveInf = int.MaxValue;
        private const int NegativeInf = int.MinValue;
        private const int MaxComputationTimeInMilliseconds = 200;
        
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

        public Move MinimaxIterativeDeepening(Board game, bool maximizingPlayer)
        {
            _timer.Start();
            
            for (var depth = 1;; depth++)
            {
                var move = MinimaxRoot(depth, game, maximizingPlayer);

                if (move != null)
                    _oldBestMove = move;

                if (!_keepComputing)
                {
                    Console.WriteLine(depth);
                    break;
                }
            }
            
            _timer.Stop();
            _keepComputing = true;

            return _oldBestMove;
        }

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

                if (!_keepComputing)
                    return null;
            }

            return bestMoveFound;
        }    

        private int Minimax(Board node, int depth, int alpha, int beta, bool maximizingPlayer)
        {
            int? val;

            if ((val = _tt.ProbeHash(node, depth, alpha, beta)) != null)
                return (int) val;
            
            // Stop condition for recursion
            if (depth <= 0 || !_keepComputing)
            {
                var eval = _heuristic(node);
                _tt.RecordHash(node, null, depth, eval, TranspositionTableElement.Exact);
                return eval;
            }

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
                    {
                        _tt.RecordHash(node, null, depth, alpha, TranspositionTableElement.Alpha);
                        break; // Beta cut-off
                    }
                }

                _tt.RecordHash(node, null, depth, alpha, TranspositionTableElement.Exact);
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
                    {
                        _tt.RecordHash(node, null, depth, beta, TranspositionTableElement.Beta);
                        break; // Alpha cut-off
                    }
                }
                
                _tt.RecordHash(node, null, depth, alpha, TranspositionTableElement.Exact);
                return bestValue;
            }
        }
    }
}