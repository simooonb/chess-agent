namespace ChessAgent
{
    public class Piece
    {
        public PieceType Type { get; private set; }
        public string Position { get; private set; }
        public PieceColor Color { get; private set; }

        public Piece(PieceType type, string pos, PieceColor color)
        {
            Type = type;
            Position = pos;
            Color = color;
        }
    }
}