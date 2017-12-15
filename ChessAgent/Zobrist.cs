using System;

namespace ChessAgent
{
    public class Zobrist
    {
        // Indexing by piece type, color, and square
        private readonly ulong[][][] _zobrist;
        
        // Side
        private readonly ulong _side;

        public Zobrist()
        {
            var rand = new Random();
            _zobrist = new ulong[6][][];

            for (var type = 0; type < 6; type++)
            {
                _zobrist[type] = new ulong[2][];

                for (var color = 0; color < 2; color++)
                {
                    _zobrist[type][color] = new ulong[64];

                    for (var square = 0; square < 64; square++)
                    {
                        // Random ulong
                        _zobrist[type][color][square] = (ulong) rand.Next() ^ ((ulong) rand.Next() << 31);
                    }
                }
            }

            _side = (ulong) rand.Next() ^ ((ulong) rand.Next() << 31);
        }

        public ulong FindBoardHash(Board board)
        {
            var pieces = board.AllPieces;
            var hash = board.ColorPlaying == PieceColor.Black ? _side : 0;

            foreach (var piece in pieces)
            {
                hash ^= _zobrist[(int) piece.Type][(int) piece.Color][Board.PositionToIndex(piece.Position) - 1];
            }

            return hash;
        }
    }
}