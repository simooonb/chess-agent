namespace ChessAgent
{
    public class Move
    {
        public string From { get; private set; }
        public string To { get; private set; }
        public PieceType Type { get; private set; }
        public PieceColor Color { get; private set; }

        public Move(string from, string to, PieceType type, PieceColor color)
        {
            From = from;
            To = to;
            Type = type;
            Color = color;
        }

        public override string ToString()
        {
            return Color + " " + Type + ": " + From + "-" + To;
        }
    }
}
