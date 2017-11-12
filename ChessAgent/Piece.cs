using System;

namespace ChessAgent
{
    public class Piece
    {
        public MoveChecker RuleChecker { get; private set; }
        public string Position { get; private set; }
        public PieceColor Color { get; private set; }

        public Piece(MoveChecker moveChecker, string pos, PieceColor color)
        {
            RuleChecker = moveChecker;
            Position = pos;
            Color = color;
            
            if (!RuleChecker.IsInBoard(pos))
                throw new ArgumentException("Piece position is out of the board bounds.");
        }

        public void Move(string to)
        {
            
        }
    }
}