using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessAgent
{
    public class Agent
    {
        private BoardState _board;
        public PieceColor Color { get; private set; }

        public Agent(PieceColor color)
        {
            Color = color;
            _board = new BoardState(color);
        }
        
        public void ObserveEnvironmentAndUpdateState(Dictionary<string, int> ownPieces, Dictionary<string, int> opponentPieces)
        {
            _board.Update(ownPieces, opponentPieces);
        }

        public string[] ChooseMove()
        {
            var move = new[] { "", "", "D" };  // Queen promotion is almost always the best choice (good enough here)
            var empty = _board.EmptySquares;
            var rnd = new Random();
            var legalMoves = ComputeLegalMovesAvailable();
            
            move[0] = legalMoves[1][0];  // From
            move[1] = legalMoves[1][1];  // To

            return move;
        }

        public List<string[]> ComputeLegalMovesAvailable()
        {
            var legalMoves = new List<string[]>();
            var ownPiecesPos = _board.OwnPieces.Select(pieces => pieces.Position).ToList();
            var oppPiecesPos = _board.OpponentPieces.Select(pieces => pieces.Position).ToList();

            foreach (var piece in _board.OwnPieces)
            {
                // Compute every legal move
                var subLegalMoves = piece.RuleChecker.LegalMoves(piece.Position, Color, oppPiecesPos);
                
                // Remove positions where we have a piece
                subLegalMoves.RemoveAll(s => ownPiecesPos.Contains(s));

                // Add to list
                foreach (var move in subLegalMoves)
                {
                    legalMoves.Add(new[] { piece.Position, move });
                }
            }
            
            return legalMoves;
        }
    }
}