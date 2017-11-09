using System;
using System.Collections.Generic;

namespace ChessAgent
{
    public class Agent
    {
        BoardState board = new BoardState();
        
        public void ObserveEnvironmentAndUpdateState(List<string> myPieces, List<string> opponenetPieces)
        {
            board.Update(myPieces, opponenetPieces);
        }

        public string[] ChooseMove()
        {
            var move = new[] { "", "", "" };
            var empty = board.EmptySquares;
            var rnd = new Random();
            
            move[0] = board.OwnPieces[rnd.Next(board.OwnPieces.Count)];
            move[1] = empty[rnd.Next(empty.Length)];

            return move;
        }
    }
}