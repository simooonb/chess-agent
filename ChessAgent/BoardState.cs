using System.Collections.Generic;
using System.Linq;

namespace ChessAgent
{
    public class BoardState
    {
        public PieceColor Color { get; private set; }
        public List<Piece> OwnPieces { get; private set; }
        public List<Piece> OpponentPieces { get; private set; }
        public readonly string[] Board = { "a8","b8","c8","d8","e8","f8","g8","h8",
                                           "a7","b7","c7","d7","e7","f7","g7","h7",
                                           "a6","b6","c6","d6","e6","f6","g6","h6",
                                           "a5","b5","c5","d5","e5","f5","g5","h5",
                                           "a4","b4","c4","d4","e4","f4","g4","h4",
                                           "a3","b3","c3","d3","e3","f3","g3","h3",
                                           "a2","b2","c2","d2","e2","f2","g2","h2",
                                           "a1","b1","c1","d1","e1","f1","g1","h1" };
        
        // Consts
        private const int PassingPawn = 10;
        private const int Pawn = 1;
        private const int LeftRook = 21;
        private const int RightRook = 22;
        private const int LeftKnight = 31;
        private const int RightKnight = 32;
        private const int Bishop = 4;
        private const int Queen = 5;
        private const int King = 6;

        public BoardState(PieceColor color)
        {
            Color = color;
            OwnPieces = new List<Piece>();
            OpponentPieces = new List<Piece>();
        }
        
        public string[] EmptySquares
        {
            get
            {
                var ownPiecesPos = OwnPieces.Select(pieces => pieces.Position).ToList();

                return Board.Where(s => !ownPiecesPos.Contains(s)).ToArray();
            }
        }

        public void Update(Dictionary<string, int> ownPieces, Dictionary<string, int> opponentPieces)
        {
            OwnPieces.Clear();
            OpponentPieces.Clear();
            
            foreach (var kvp in ownPieces)
            {
                AddToPieces(kvp.Value, kvp.Key);
            }
            
            foreach (var kvp in opponentPieces)
            {
                AddToPieces(kvp.Value, kvp.Key);
            }
        }

        private void AddToPieces(int value, string pos)
        {
            if (Color.Equals(PieceColor.White))
            {
                switch (value)
                {
                    // Own pieces
                    case PassingPawn:
                    case Pawn:
                        OwnPieces.Add(new Piece(new PawnChecker(), pos, Color));
                        break;
                    
                    case LeftRook:
                    case RightRook:
                        OwnPieces.Add(new Piece(new RookChecker(), pos, Color));
                        break;
                        
                    case LeftKnight:
                    case RightKnight:
                        OwnPieces.Add(new Piece(new KnightChecker(), pos, Color));
                        break;
                        
                    case Bishop:
                        OwnPieces.Add(new Piece(new BishopChecker(), pos, Color));
                        break;
                        
                    case Queen:
                        OwnPieces.Add(new Piece(new QueenChecker(), pos, Color));
                        break;
                        
                    case King:
                        OwnPieces.Add(new Piece(new KingChecker(), pos, Color));
                        break;
                    
                    // Opponent pieces
                    case PassingPawn * -1:
                    case Pawn * -1:
                        OpponentPieces.Add(new Piece(new PawnChecker(), pos, Color));
                        break;
                    
                    case LeftRook * -1:
                    case RightRook * -1:
                        OpponentPieces.Add(new Piece(new RookChecker(), pos, Color));
                        break;
                        
                    case LeftKnight * -1:
                    case RightKnight * -1:
                        OpponentPieces.Add(new Piece(new KnightChecker(), pos, Color));
                        break;
                        
                    case Bishop * -1:
                        OpponentPieces.Add(new Piece(new BishopChecker(), pos, Color));
                        break;
                        
                    case Queen * -1:
                        OpponentPieces.Add(new Piece(new QueenChecker(), pos, Color));
                        break;
                        
                    case King * -1:
                        OpponentPieces.Add(new Piece(new KingChecker(), pos, Color));
                        break;
                }
            }
            else if (Color.Equals(PieceColor.White))
            {
                switch (value)
                {
                    // Opponent pieces
                    case PassingPawn:
                    case Pawn:
                        OpponentPieces.Add(new Piece(new PawnChecker(), pos, Color));
                        break;
                    
                    case LeftRook:
                    case RightRook:
                        OpponentPieces.Add(new Piece(new RookChecker(), pos, Color));
                        break;
                        
                    case LeftKnight:
                    case RightKnight:
                        OpponentPieces.Add(new Piece(new KnightChecker(), pos, Color));
                        break;
                        
                    case Bishop:
                        OpponentPieces.Add(new Piece(new BishopChecker(), pos, Color));
                        break;
                        
                    case Queen:
                        OpponentPieces.Add(new Piece(new QueenChecker(), pos, Color));
                        break;
                        
                    case King:
                        OpponentPieces.Add(new Piece(new KingChecker(), pos, Color));
                        break;
                    
                    // Own pieces
                    case PassingPawn * -1:
                    case Pawn * -1:
                        OwnPieces.Add(new Piece(new PawnChecker(), pos, Color));
                        break;
                    
                    case LeftRook * -1:
                    case RightRook * -1:
                        OwnPieces.Add(new Piece(new RookChecker(), pos, Color));
                        break;
                        
                    case LeftKnight * -1:
                    case RightKnight * -1:
                        OwnPieces.Add(new Piece(new KnightChecker(), pos, Color));
                        break;
                        
                    case Bishop * -1:
                        OwnPieces.Add(new Piece(new BishopChecker(), pos, Color));
                        break;
                        
                    case Queen * -1:
                        OwnPieces.Add(new Piece(new QueenChecker(), pos, Color));
                        break;
                        
                    case King * -1:
                        OwnPieces.Add(new Piece(new KingChecker(), pos, Color));
                        break;
                }
            }
        }
    }
}