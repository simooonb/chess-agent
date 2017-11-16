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
        
        /// <summary>
        /// Update inner state based on environment's input.
        /// </summary>
        /// <param name="ownPieces">Our pieces' positions.</param>
        /// <param name="opponentPieces">Opponent pieces' positions.</param>
        public void ObserveEnvironmentAndUpdateState(Dictionary<string, int> ownPieces, Dictionary<string, int> opponentPieces)
        {
            _board.Update(ownPieces, opponentPieces);
        }

        /// <summary>
        /// Choose a move.
        /// </summary>
        /// <returns>The move chosen as a string array.</returns>
        public string[] ChooseMove()
        {
            var move = new[] { "", "", "D" };  // Queen promotion is almost always the best choice (good enough here)
            var empty = _board.EmptySquares;
            var rnd = new Random();
            var legalMoves = ComputeLegalMovesAvailable();
            
            Console.WriteLine("legal moves count: " + legalMoves.Count);
            
            // TODO: Add minimax algorithm
            
            move[0] = legalMoves[0][0];  // From
            move[1] = legalMoves[0][1];  // To

            return move;
        }
        
        private List<string[]> ComputeLegalMovesAvailable()
        {
            var legalMoves = new List<string[]>();
            var ownPiecesPos = _board.OwnPieces.Select(pieces => pieces.Position).ToList();

            foreach (var piece in _board.OwnPieces)
            {
                // Compute every legal move
                var subLegalMoves = piece.RuleChecker.LegalMoves(piece.Position, Color);

                // Remove our pieces from legal moves
                subLegalMoves.RemoveAll(move => ownPiecesPos.Contains(move));
                
                // TODO: Verify if king is in check before adding to list

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