namespace ChessAgent
{
    public class BoardState
    {
        public Player Player { get; private set; }

        public BoardState(Player player)
        {
            Player = player;
        }
    }
}