namespace ChessAgent
{
    public class TranspositionTableElement
    {
        public const int Exact = 0;
        public const int Alpha = 1;
        public const int Beta = 2;
        
        public ulong Key;
        public int Depth;
        public int Flags;
        public int Evaluation;
        public Move BestMove;

        public TranspositionTableElement(ulong key, int depth, int hashf, int eval, Move move)
        {
            Key = key;
            Depth = depth;
            Flags = hashf;
            Evaluation = eval;
            BestMove = move;
        }
    }
}