namespace ChessAgent
{
    public class TranspositionTable
    {
        private TranspositionTableElement[] _table;
        private Zobrist _zobrist = new Zobrist();
        private const int TableSize = 100000;

        public TranspositionTable()
        {
            _table = new TranspositionTableElement[TableSize];
        }

        public int? ProbeHash(Board board, int depth, int alpha, int beta)
        {
            var boardHash = _zobrist.FindBoardHash(board);
            var element = _table[boardHash % TableSize];

            if (element == null)
                return null;

            if (element.Key == boardHash)
            {
                if (element.Depth >= depth)
                {
                    if (element.Flags == TranspositionTableElement.Exact)
                        return element.Evaluation;

                    if (element.Flags == TranspositionTableElement.Alpha && element.Evaluation <= alpha)
                        return alpha;

                    if (element.Flags == TranspositionTableElement.Beta && element.Evaluation >= beta)
                        return beta;
                }
                
                // remember best move?
            }

            return null;
        }

        public void RecordHash(Board board, Move bestMove, int depth, int eval, int hashf)
        {
            var boardHash = _zobrist.FindBoardHash(board);
            var idx = boardHash % TableSize;

            if (_table[idx] == null)
                _table[idx] = new TranspositionTableElement(boardHash, depth, hashf, eval, bestMove);
            else
            {
                _table[idx].Key = boardHash;
                _table[idx].BestMove = bestMove;
                _table[idx].Evaluation = eval;
                _table[idx].Flags = hashf;
                _table[idx].Depth = depth;
            }
        }
    }
}