using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ChessAgent
{
    public class Board
    {       
        // Pieces consts
        private const int PassingPawn = 10;
        private const int Pawn = 1;
        private const int LeftRook = 21;
        private const int RightRook = 22;
        private const int LeftKnight = 31;
        private const int RightKnight = 32;
        private const int Bishop = 4;
        private const int Queen = 5;
        private const int King = 6;
        
        // Pieces bitboards
        public ulong[][] Pieces;

        public PieceColor ColorPlaying = PieceColor.White;
        public Stack<Move> MovesPlayed = new Stack<Move>();
        
        // Columns and rows bitboards
        private const ulong ACol = 0x0101010101010101;
        private const ulong BCol = ACol << 1;
        private const ulong CCol = ACol << 2;
        private const ulong DCol = ACol << 3;
        private const ulong ECol = ACol << 4;
        private const ulong FCol = ACol << 5;
        private const ulong GCol = ACol << 6;
        private const ulong HCol = ACol << 7;

        private const ulong Row8 = 0xff;
        private const ulong Row7 = Row8 << (8 * 1);
        private const ulong Row6 = Row8 << (8 * 2);
        private const ulong Row5 = Row8 << (8 * 3);
        private const ulong Row4 = Row8 << (8 * 4);
        private const ulong Row3 = Row8 << (8 * 5);
        private const ulong Row2 = Row8 << (8 * 6);
        private const ulong Row1 = Row8 << (8 * 7);

        private const ulong DarkSquares = 0xaa55aa55aa55aa55;

        private static readonly int[] Index64 =
        {
            0, 47,  1, 56, 48, 27,  2, 60,
            57, 49, 41, 37, 28, 16,  3, 61,
            54, 58, 35, 52, 50, 42, 21, 44,
            38, 32, 29, 23, 17, 11,  4, 62,
            46, 55, 26, 59, 40, 36, 15, 53,
            34, 51, 20, 43, 31, 22, 10, 45,
            25, 39, 14, 33, 19, 30,  9, 24,
            13, 18,  8, 12,  7,  6,  5, 63
        };

        public readonly string[] EnumSquare =
        {
            "a8", "b8", "c8", "d8", "e8", "f8", "g8", "h8",
            "a7", "b7", "c7", "d7", "e7", "f7", "g7", "h7",
            "a6", "b6", "c6", "d6", "e6", "f6", "g6", "h6",
            "a5", "b5", "c5", "d5", "e5", "f5", "g5", "h5",
            "a4", "b4", "c4", "d4", "e4", "f4", "g4", "h4",
            "a3", "b3", "c3", "d3", "e3", "f3", "g3", "h3",
            "a2", "b2", "c2", "d2", "e2", "f2", "g2", "h2",
            "a1", "b1", "c1", "d1", "e1", "f1", "g1", "h1"
        };

        public Board(int[] pieces)
        {
            InitialiseBoard(pieces);
        }

        public Board(ulong[][] pieces)
        {
            Pieces = new ulong[2][];
            Pieces[(int) PieceColor.White] = new ulong[6];
            Pieces[(int) PieceColor.Black] = new ulong[6];
            
            pieces[(int) PieceColor.White].CopyTo(Pieces[(int) PieceColor.White], 0);
            pieces[(int) PieceColor.Black].CopyTo(Pieces[(int) PieceColor.Black], 0);
        }

        private void InitialiseBoard(int[] pieces)
        {
            Pieces = new ulong[2][];
            
            // Indexing by pieces' color
            Pieces[(int) PieceColor.White] = new ulong[6];
            Pieces[(int) PieceColor.Black] = new ulong[6];
            
            // Indexing by pieces' type
            Pieces[(int) PieceColor.White][(int) PieceType.Pawn] = 0;
            Pieces[(int) PieceColor.White][(int) PieceType.Knight] = 0;
            Pieces[(int) PieceColor.White][(int) PieceType.Bishop] = 0;
            Pieces[(int) PieceColor.White][(int) PieceType.Rook] = 0;
            Pieces[(int) PieceColor.White][(int) PieceType.Queen] = 0;
            Pieces[(int) PieceColor.White][(int) PieceType.King] = 0;
            
            Pieces[(int) PieceColor.Black][(int) PieceType.Pawn] = 0;
            Pieces[(int) PieceColor.Black][(int) PieceType.Knight] = 0;
            Pieces[(int) PieceColor.Black][(int) PieceType.Bishop] = 0;
            Pieces[(int) PieceColor.Black][(int) PieceType.Rook] = 0;
            Pieces[(int) PieceColor.Black][(int) PieceType.Queen] = 0;
            Pieces[(int) PieceColor.Black][(int) PieceType.King] = 0;

            for (short i = 0; i < 64; i++)
            {
                // White pieces
                if (pieces[i] == King)
                    Pieces[(int) PieceColor.White][(int) PieceType.King] |= (ulong) 1 << i;
                else if (pieces[i] == Queen)
                    Pieces[(int) PieceColor.White][(int) PieceType.Queen] |= (ulong) 1 << i;
                else if (pieces[i] == Bishop)
                    Pieces[(int) PieceColor.White][(int) PieceType.Bishop] |= (ulong) 1 << i;
                else if (pieces[i] == LeftKnight || pieces[i] == RightKnight)
                    Pieces[(int) PieceColor.White][(int) PieceType.Knight] |= (ulong) 1 << i;
                else if (pieces[i] == LeftRook || pieces[i] == RightRook)
                    Pieces[(int) PieceColor.White][(int) PieceType.Rook] |= (ulong) 1 << i;
                else if (pieces[i] == Pawn || pieces[i] == PassingPawn)
                    Pieces[(int) PieceColor.White][(int) PieceType.Pawn] |= (ulong) 1 << i;
                
                // Black pieces
                else if (pieces[i] == King * -1)
                    Pieces[(int) PieceColor.Black][(int) PieceType.King] |= (ulong) 1 << i;
                else if (pieces[i] == Queen * -1)
                    Pieces[(int) PieceColor.Black][(int) PieceType.Queen] |= (ulong) 1 << i;
                else if (pieces[i] == Bishop * -1)
                    Pieces[(int) PieceColor.Black][(int) PieceType.Bishop] |= (ulong) 1 << i;
                else if (pieces[i] == LeftKnight * -1 || pieces[i] == RightKnight * -1)
                    Pieces[(int) PieceColor.Black][(int) PieceType.Knight] |= (ulong) 1 << i;
                else if (pieces[i] == LeftRook * -1 || pieces[i] == RightRook * -1)
                    Pieces[(int) PieceColor.Black][(int) PieceType.Rook] |= (ulong) 1 << i;
                else if (pieces[i] == Pawn * -1 || pieces[i] == PassingPawn * -1)
                    Pieces[(int) PieceColor.Black][(int) PieceType.Pawn] |= (ulong) 1 << i;
            }
        }
        
        /**
         * Properties.
         */

        public ulong WhiteKing
        {
            get { return Pieces[(int) PieceColor.White][(int) PieceType.King]; }
        }

        public ulong WhiteQueens
        {
            get { return Pieces[(int) PieceColor.White][(int) PieceType.Queen]; }
        }

        public ulong WhiteRooks
        {
            get { return Pieces[(int) PieceColor.White][(int) PieceType.Rook]; }
        }

        public ulong WhiteBishops
        {
            get { return Pieces[(int) PieceColor.White][(int) PieceType.Bishop]; }
        }

        public ulong WhiteKnights
        {
            get { return Pieces[(int) PieceColor.White][(int) PieceType.Knight]; }
        }

        public ulong WhitePawns
        {
            get { return Pieces[(int) PieceColor.White][(int) PieceType.Pawn]; }
        }

        public ulong WhitePieces
        {
            get
            {
                return WhitePawns   |
                       WhiteRooks   |
                       WhiteKnights |
                       WhiteBishops |
                       WhiteQueens  |
                       WhiteKing;
            }
        }
        
        public ulong BlackKing
        {
            get { return Pieces[(int) PieceColor.Black][(int) PieceType.King]; }
        }

        public ulong BlackQueens
        {
            get { return Pieces[(int) PieceColor.Black][(int) PieceType.Queen]; }
        }

        public ulong BlackRooks
        {
            get { return Pieces[(int) PieceColor.Black][(int) PieceType.Rook]; }
        }

        public ulong BlackBishops
        {
            get { return Pieces[(int) PieceColor.Black][(int) PieceType.Bishop]; }
        }

        public ulong BlackKnights
        {
            get { return Pieces[(int) PieceColor.Black][(int) PieceType.Knight]; }
        }

        public ulong BlackPawns
        {
            get { return Pieces[(int) PieceColor.Black][(int) PieceType.Pawn]; }
        }
        
        public ulong BlackPieces
        {
            get
            {
                return BlackPawns   |
                       BlackRooks   |
                       BlackKnights |
                       BlackBishops |
                       BlackQueens  |
                       BlackKing;
            }
        }
        
        public ulong OccupiedSpace
        {
            get { return WhitePieces | BlackPieces; }
        }
        
        public ulong EmptySpace
        {
            get { return ~OccupiedSpace; }
        }
        
        public ulong WhiteAttacksPattern
        {
            get
            {
                return KingPattern(PieceColor.White)    |
                       QueensPattern(PieceColor.White)  |
                       RooksPattern(PieceColor.White)   |
                       BishopsPattern(PieceColor.White) |
                       KnightsPattern(PieceColor.White) |
                       WhitePawnsAnyAttacks();
            }
        }

        public ulong WhiteAttackAndDefendPattern
        {
            get
            {
                return KingAttackPattern(WhiteKing)       |
                       QueensAttackPattern(WhiteQueens)   |
                       RooksAttackPattern(WhiteRooks)     |
                       BishopsAttackPattern(WhiteBishops) |
                       KnightsAttackPattern(WhiteKnights) |
                       WhitePawnsAnyAttacks();
            }
        }
        
        public ulong BlackAttacksPattern
        {
            get
            {
                return KingPattern(PieceColor.Black)    |
                       QueensPattern(PieceColor.Black)  |
                       RooksPattern(PieceColor.Black)   |
                       BishopsPattern(PieceColor.Black) |
                       KnightsPattern(PieceColor.Black) |
                       BlackPawnsAnyAttacks();
            }
        }
        
        public ulong BlackAttackAndDefendPattern
        {
            get
            {
                return KingAttackPattern(BlackKing)      |
                       QueensAttackPattern(BlackQueens)   |
                       RooksAttackPattern(BlackRooks)     |
                       BishopsAttackPattern(BlackBishops) |
                       KnightsAttackPattern(BlackKnights) |
                       BlackPawnsAnyAttacks();
            }
        }

        public int WhiteKingMobility
        {
            get { return Count(KingAttackPattern(WhiteKing) & ~WhitePieces); }
        }

        public int WhiteQueensMobility
        {
            get { return Count(QueensAttackPattern(WhiteQueens) & ~WhitePieces); }
        }
        
        public int WhiteRooksMobility
        {
            get { return Count(RooksAttackPattern(WhiteRooks) & ~WhitePieces); }
        }
        
        public int WhiteBishopsMobility
        {
            get { return Count(BishopsAttackPattern(WhiteBishops) & ~WhitePieces); }
        }
        
        public int WhiteKnightsMobility
        {
            get { return Count(KnightsAttackPattern(WhiteKnights) & ~WhitePieces); }
        }
        
        public int WhitePawnsMobility
        {
            get { return Count(WhitePawnsPattern()); }
        }

        public int WhitePiecesMobility
        {
            get
            {
                return WhiteKingMobility + WhiteQueensMobility + WhiteRooksMobility + WhiteBishopsMobility +
                       WhiteKnightsMobility + WhitePawnsMobility;
            }
        }
        
        public int BlackKingMobility
        {
            get { return Count(KingAttackPattern(BlackKing) & ~BlackPieces); }
        }

        public int BlackQueensMobility
        {
            get { return Count(QueensAttackPattern(BlackQueens) & ~BlackPieces); }
        }
        
        public int BlackRooksMobility
        {
            get { return Count(RooksAttackPattern(BlackRooks) & ~BlackPieces); }
        }
        
        public int BlackBishopsMobility
        {
            get { return Count(BishopsAttackPattern(BlackBishops) & ~BlackPieces); }
        }
        
        public int BlackKnightsMobility
        {
            get { return Count(KnightsAttackPattern(BlackKnights) & ~BlackPieces); }
        }
        
        public int BlackPawnsMobility
        {
            get { return Count(BlackPawnsPattern()); }
        }

        public int BlackPiecesMobility
        {
            get
            {
                return BlackKingMobility + BlackQueensMobility + BlackRooksMobility + BlackBishopsMobility +
                       BlackKnightsMobility + BlackPawnsMobility;
            }
        }

        public bool IsWhiteKingInCheck
        {
            get { return (BlackAttacksPattern & WhiteKing) != 0; }
        }

        public bool IsBlackKingInCheck
        {
            get { return (WhiteAttacksPattern & BlackKing) != 0; }
        }

        public ulong WhitePawnsAbleToPush
        {
            get { return SouthOne(EmptySpace) & WhitePawns; }
        }

        public ulong WhitePawnsAbleToDoublePush
        {
            get
            {
                ulong emptyRow3 = SouthOne(EmptySpace & Row4) & EmptySpace;
                return SouthOne(emptyRow3) & WhitePawns;
            }
        }
        
        public ulong BlackPawnsAbleToPush
        {
            get { return NorthOne(EmptySpace) & BlackPawns; }
        }

        public ulong BlackPawnsAbleToDoublePush
        {
            get
            {
                ulong emptyRow6 = NorthOne(EmptySpace & Row5) & EmptySpace;
                return NorthOne(emptyRow6) & BlackPawns;
            }
        }

        private ulong SafeSquaresByWhitePawns {
            get
            {
                return WhitePawnsDoubleAttacks() | ~BlackPawnsAnyAttacks() |
                       (WhitePawnsSingleAttacks() & ~BlackPawnsDoubleAttacks());
            }
        }
        
        private ulong SafeSquaresByBlackPawns {
            get
            {
                return BlackPawnsDoubleAttacks() | ~WhitePawnsAnyAttacks() |
                       (BlackPawnsSingleAttacks() & ~WhitePawnsDoubleAttacks());
            }
        }

        public int WhiteKingCount
        {
            get { return Count(WhiteKing); }
        }
        
        public int WhiteQueensCount
        {
            get { return Count(WhiteQueens); }
        }

        public int WhiteRooksCount
        {
            get { return Count(WhiteRooks); }
        }
        
        public int WhiteKnightsCount
        {
            get { return Count(WhiteKnights); }
        }
        
        public int WhiteBishopsCount
        {
            get { return Count(WhiteBishops); }
        }
        
        public int WhitePawnsCount
        {
            get { return Count(WhitePawns); }
        }

        public int BlackKingCount
        {
            get { return Count(BlackKing); }
        }
        
        public int BlackQueensCount
        {
            get { return Count(BlackQueens); }
        }

        public int BlackRooksCount
        {
            get { return Count(BlackRooks); }
        }

        public int BlackKnightsCount
        {
            get { return Count(BlackKnights); }
        }

        public int BlackBishopsCount
        {
            get { return Count(BlackBishops); }
        }

        public int BlackPawnsCount
        {
            get { return Count(BlackPawns); }
        }
        
        /**
         * Methods.
         */

        /// <summary>
        /// Pseudo-legal move generation.
        /// </summary>
        /// <param name="color">Color for which we generate moves.</param>
        /// <returns>A List of Move instances.</returns>
        public List<Move> GenerateMovesFor(PieceColor color)
        {
            var moves = new List<Move>();

            moves.AddRange(GenerateKingMovesFor(color));
            moves.AddRange(GenerateQueenMovesFor(color));
            moves.AddRange(GenerateRookMovesFor(color));
            moves.AddRange(GenerateKnightMovesFor(color));
            moves.AddRange(GenerateBishopMovesFor(color));
            moves.AddRange(GeneratePawnMovesFor(color));

            return moves;
        }

        public List<Move> GenerateNextMoves()
        {
            return GenerateMovesFor(ColorPlaying);
        }

        public Board[] GenerateBoardsFromMoves(IEnumerable<Move> moves)
        {
            var boards = new List<Board>();

            foreach (var move in moves)
            {
                var board = new Board(Pieces);
                board.PlayMove(move);
                
                // If player just played and still is in check
                if (move.Color == PieceColor.White && !board.IsWhiteKingInCheck)
                    boards.Add(board);
                else if (move.Color == PieceColor.Black && !board.IsBlackKingInCheck)
                    boards.Add(board);
            }

            return boards.ToArray();
        }

        public void PlayMove(Move move)
        {
            Debug.Assert(move.Color == ColorPlaying);
            if (move.Color != ColorPlaying)
                return;
            
            var fromIdx = Array.IndexOf(EnumSquare, move.From);

            // Special cases first
            
            // Left castle
            if (move.To == "grand roque")
            {
                if (move.Color == PieceColor.White)
                {
                    Pieces[(int) move.Color][(int) PieceType.King] |= (ulong) 1 << Array.IndexOf(EnumSquare, "c1");
                    Pieces[(int) move.Color][(int) PieceType.King] &= ~((ulong) 1 << Array.IndexOf(EnumSquare, "e1"));
                    Pieces[(int) move.Color][(int) PieceType.Rook] |= (ulong) 1 << Array.IndexOf(EnumSquare, "d1");
                    Pieces[(int) move.Color][(int) PieceType.Rook] &= ~((ulong) 1 << Array.IndexOf(EnumSquare, "a1"));
                }
                else if (move.Color == PieceColor.Black)
                {
                    Pieces[(int) move.Color][(int) PieceType.King] |= (ulong) 1 << Array.IndexOf(EnumSquare, "c8");
                    Pieces[(int) move.Color][(int) PieceType.King] &= ~((ulong) 1 << Array.IndexOf(EnumSquare, "e8"));
                    Pieces[(int) move.Color][(int) PieceType.Rook] |= (ulong) 1 << Array.IndexOf(EnumSquare, "d8");
                    Pieces[(int) move.Color][(int) PieceType.Rook] &= ~((ulong) 1 << Array.IndexOf(EnumSquare, "a8"));
                }
                
                ColorPlaying = ColorPlaying == PieceColor.White ? PieceColor.Black : PieceColor.White;
                MovesPlayed.Push(move);
                return;
            }
            // Right castle
            if (move.To == "petit roque")
            {
                if (move.Color == PieceColor.White)
                {
                    Pieces[(int) move.Color][(int) PieceType.King] |= (ulong) 1 << Array.IndexOf(EnumSquare, "g1");
                    Pieces[(int) move.Color][(int) PieceType.King] &= ~((ulong) 1 << Array.IndexOf(EnumSquare, "e1"));
                    Pieces[(int) move.Color][(int) PieceType.Rook] |= (ulong) 1 << Array.IndexOf(EnumSquare, "f1");
                    Pieces[(int) move.Color][(int) PieceType.Rook] &= ~((ulong) 1 << Array.IndexOf(EnumSquare, "h1"));
                }
                else if (move.Color == PieceColor.Black)
                {
                    Pieces[(int) move.Color][(int) PieceType.King] |= (ulong) 1 << Array.IndexOf(EnumSquare, "g8");
                    Pieces[(int) move.Color][(int) PieceType.King] &= ~((ulong) 1 << Array.IndexOf(EnumSquare, "e8"));
                    Pieces[(int) move.Color][(int) PieceType.Rook] |= (ulong) 1 << Array.IndexOf(EnumSquare, "f8");
                    Pieces[(int) move.Color][(int) PieceType.Rook] &= ~((ulong) 1 << Array.IndexOf(EnumSquare, "h8"));
                }

                ColorPlaying = ColorPlaying == PieceColor.White ? PieceColor.Black : PieceColor.White;
                MovesPlayed.Push(move);
                return;  // No capture here
            }
            
            var toIdx = Array.IndexOf(EnumSquare, move.To);
            
            Pieces[(int) move.Color][(int) move.Type] |= (ulong) 1 << toIdx;     // Set the new position
            Pieces[(int) move.Color][(int) move.Type] &= ~((ulong) 1 << fromIdx);  // Remove the old position

            // Is it a capture?
            
            PieceType? type;

            if (((BlackPawns >> toIdx) & 1) == 1)
                type = PieceType.Pawn;
            else if (((BlackKnights >> toIdx) & 1) == 1)
                type = PieceType.Knight;
            else if (((BlackBishops >> toIdx) & 1) == 1)
                type = PieceType.Bishop;
            else if (((BlackRooks >> toIdx) & 1) == 1)
                type = PieceType.Rook;
            else if (((BlackQueens >> toIdx) & 1) == 1)
                type = PieceType.Queen;
            else if (((BlackKing >> toIdx) & 1) == 1)
                type = PieceType.King;
            else
                type = null;
                
            // If there was a black piece there... (as a white piece)
            if (move.Color == PieceColor.White && type != null)
            {
                Pieces[(int) PieceColor.Black][(int) type] &= ~((ulong) 1 << toIdx); // Delete the captured piece
                move.PieceCaptured = type;                                           // And remember it
            }
            else
            {
                if (((WhitePawns >> toIdx) & 1) == 1)
                    type = PieceType.Pawn;
                else if (((WhiteKnights >> toIdx) & 1) == 1)
                    type = PieceType.Knight;
                else if (((WhiteBishops >> toIdx) & 1) == 1)
                    type = PieceType.Bishop;
                else if (((WhiteRooks >> toIdx) & 1) == 1)
                    type = PieceType.Rook;
                else if (((WhiteQueens >> toIdx) & 1) == 1)
                    type = PieceType.Queen;
                else if (((WhiteKing >> toIdx) & 1) == 1)
                    type = PieceType.King;
                else
                    type = null;

                // If there was a white piece there... (as a black piece)
                if (move.Color == PieceColor.Black && type != null)
                {
                    Pieces[(int) PieceColor.White][(int) type] &= ~((ulong) 1 << toIdx); // Delete the captured piece
                    move.PieceCaptured = type;                                           // And remember it
                }
            }

            // Opponent's turn now
            ColorPlaying = ColorPlaying == PieceColor.White ? PieceColor.Black : PieceColor.White;
            
            // Push to the history of moves
            MovesPlayed.Push(move);
        }

        /// <summary>
        /// Undo last move played.
        /// </summary>
        public void Undo()
        {
            var move = MovesPlayed.Pop();

            // Inverse positions
            var fromIdx = Array.IndexOf(EnumSquare, move.To);
            var toIdx = Array.IndexOf(EnumSquare, move.From);
            
            Pieces[(int) move.Color][(int) move.Type] |= (ulong) 1 << toIdx;     // Set the new position
            Pieces[(int) move.Color][(int) move.Type] &= ~((ulong) 1 << fromIdx);  // Remove the old position

            if (move.PieceCaptured != null)
            {
                if (move.Color == PieceColor.White)
                    Pieces[(int) PieceColor.Black][(int) move.PieceCaptured] |= (ulong) 1 << fromIdx;
                else
                    Pieces[(int) PieceColor.White][(int) move.PieceCaptured] |= (ulong) 1 << fromIdx;
            }

            ColorPlaying = ColorPlaying == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }
                
        public List<string> BitboardToStringList(ulong bb)
        {
            var squares = new List<string>();

            if (bb != 0)
            {
                do
                {
                    int index = BitScanForward(bb);
                    squares.Add(EnumSquare[index]);
                } while ((bb &= bb - 1) != 0);  // Reset LS1B
            }
            
            return squares;
        }

        private List<Move> GenerateKingMovesFor(PieceColor color)
        {
            var moves = new List<Move>();
            var king = Pieces[(int) color][(int) PieceType.King];

            if (king == 0)
                return moves;
            
            var origin = BitboardToStringList(king);
            var pattern = BitboardToStringList(KingPattern(color));

            if (origin.Count != 1)
                return moves;

            moves.AddRange(pattern.Select(move => new Move(origin[0], move, PieceType.King, color)));
            
            if (IsLeftCastleAllowedFor(color))
                moves.Add(new Move("grand roque", "grand roque", PieceType.King, color));
            
            if (IsRightCastleAllowedFor(color))
                moves.Add(new Move("petit roque", "petit roque", PieceType.King, color));

            return moves;
        }

        private List<Move> GenerateQueenMovesFor(PieceColor color)
        {
            var moves = new List<Move>();
            var queens = Pieces[(int) color][(int) PieceType.Queen];

            if (queens == 0)
                return moves;
            
            var movesDict = new Dictionary<string, List<string>>();

            do
            {
                ulong index = (ulong) BitScanForward(queens);
                ulong mask = (ulong) Math.Pow(2, index);
                ulong oneQueenPattern = QueensPattern(queens & mask, color);
                
                movesDict.Add(EnumSquare[index], BitboardToStringList(oneQueenPattern));
            } while ((queens &= queens - 1) != 0);  // For each queen, generate the pattern associated

            foreach (var kvp in movesDict)
            {
                // Add each move for queen kvp.Key with pattern (list) kvp.Value
                moves.AddRange(kvp.Value.Select(s => new Move(kvp.Key, s, PieceType.Queen, color)));
            }
            
            return moves;
        }
        
        private List<Move> GenerateRookMovesFor(PieceColor color)
        {
            var moves = new List<Move>();
            var rooks = Pieces[(int) color][(int) PieceType.Rook];

            if (rooks == 0)
                return moves;
            
            var movesDict = new Dictionary<string, List<string>>();

            do
            {
                ulong index = (ulong) BitScanForward(rooks);
                ulong mask = (ulong) Math.Pow(2, index);
                ulong oneRookPattern = RooksPattern(rooks & mask, color);
                
                movesDict.Add(EnumSquare[index], BitboardToStringList(oneRookPattern));
            } while ((rooks &= rooks - 1) != 0);  // For each rook, generate the pattern associated

            foreach (var kvp in movesDict)
            {
                // Add each move for rook kvp.Key with pattern (list) kvp.Value
                moves.AddRange(kvp.Value.Select(s => new Move(kvp.Key, s, PieceType.Rook, color)));
            }
            
            return moves;
        }
        
        private List<Move> GenerateKnightMovesFor(PieceColor color)
        {
            var moves = new List<Move>();
            var knights = Pieces[(int) color][(int) PieceType.Knight];

            if (knights == 0)
                return moves;
            
            var movesDict = new Dictionary<string, List<string>>();

            do
            {
                ulong index = (ulong) BitScanForward(knights);
                ulong mask = (ulong) Math.Pow(2, index);
                ulong oneKnightPattern = KnightsPattern(knights & mask, color);
                
                movesDict.Add(EnumSquare[index], BitboardToStringList(oneKnightPattern));
            } while ((knights &= knights - 1) != 0);  // For each knight, generate the pattern associated
            
            foreach (var kvp in movesDict)
            {
                // Add each move for knight kvp.Key with pattern (list) kvp.Value
                moves.AddRange(kvp.Value.Select(s => new Move(kvp.Key, s, PieceType.Knight, color)));
            }
            
            return moves;
        }
        
        private List<Move> GenerateBishopMovesFor(PieceColor color)
        {
            var moves = new List<Move>();
            var bishops = Pieces[(int) color][(int) PieceType.Bishop];

            if (bishops == 0)
                return moves;
            
            var movesDict = new Dictionary<string, List<string>>();

            do
            {
                ulong index = (ulong) BitScanForward(bishops);
                ulong mask = (ulong) Math.Pow(2, index);
                ulong oneBishopPattern = BishopsPattern(bishops & mask, color);
                
                movesDict.Add(EnumSquare[index], BitboardToStringList(oneBishopPattern));
            } while ((bishops &= bishops - 1) != 0);  // For each bishop, generate the pattern associated

            foreach (var kvp in movesDict)
            {
                // Add each move for bishop kvp.Key with pattern (list) kvp.Value
                moves.AddRange(kvp.Value.Select(s => new Move(kvp.Key, s, PieceType.Bishop, color)));
            }
            
            return moves;
        }
        
        private List<Move> GeneratePawnMovesFor(PieceColor color)
        {
            var moves = new List<Move>();
            var pawns = Pieces[(int) color][(int) PieceType.Pawn];

            if (pawns == 0)
                return moves;
            
            var movesDict = new Dictionary<string, List<string>>();

            do
            {
                ulong index = (ulong) BitScanForward(pawns);
                ulong mask = (ulong) Math.Pow(2, index);
                ulong onePawnPattern = PawnPattern(pawns & mask, color);           
                
                movesDict.Add(EnumSquare[index], BitboardToStringList(onePawnPattern));
            } while ((pawns &= pawns - 1) != 0);  // For each pawn, generate the pattern associated

            foreach (var kvp in movesDict)
            {
                // Add each move for pawn kvp.Key with pattern (list) kvp.Value
                moves.AddRange(kvp.Value.Select(s => new Move(kvp.Key, s, PieceType.Pawn, color)));
            }
            
            return moves;
        }

        public bool IsRightCastleAllowedFor(PieceColor color)
        {
            ulong king = Pieces[(int) color][(int) PieceType.King];
            ulong rooks = Pieces[(int) color][(int) PieceType.Rook];
            ulong colorPieces = color == PieceColor.White ? WhitePieces : BlackPieces;

            var kingSquares = BitboardToStringList(king);
            var rooksSquares = BitboardToStringList(rooks);
            var colorSquares = BitboardToStringList(colorPieces);

            // At least 1 rook (and 1 king) needed
            if (rooksSquares.Count == 0 || kingSquares.Count != 1)
                return false;

            if (color == PieceColor.White)
            {
                // The white king need to be at his original position, and the rook as well
                if (kingSquares[0] != "e1" || !rooksSquares.Contains("h1"))
                    return false;

                // In order to castle, you can't have any pieces in between king and rook
                return !colorSquares.Contains("g1") && !colorSquares.Contains("f1");
            }
            
            if (color == PieceColor.Black)
            {
                // The black king need to be at his original position, and at least one of the rook as well
                if (kingSquares[0] != "e8" || !rooksSquares.Contains("h8"))
                    return false;

                // In order to castle, you can't have any pieces in between king and rook
                return !colorSquares.Contains("g8") && !colorSquares.Contains("f8");
            }

            return false;
        }
        
        public bool IsLeftCastleAllowedFor(PieceColor color)
        {
            ulong king = Pieces[(int) color][(int) PieceType.King];
            ulong rooks = Pieces[(int) color][(int) PieceType.Rook];
            ulong colorPieces = color == PieceColor.White ? WhitePieces : BlackPieces;

            var kingSquares = BitboardToStringList(king);
            var rooksSquares = BitboardToStringList(rooks);
            var colorSquares = BitboardToStringList(colorPieces);

            // At least 1 rook (and 1 king) needed
            if (rooksSquares.Count == 0 || kingSquares.Count != 1)
                return false;

            if (color == PieceColor.White)
            {
                // The white king need to be at his original position, and the rook as well
                if (kingSquares[0] != "e1" || !rooksSquares.Contains("a1"))
                    return false;

                // In order to castle, you can't have any pieces in between king and rook
                return !colorSquares.Contains("b1") && !colorSquares.Contains("c1") && !colorSquares.Contains("d1");
            }
            
            if (color == PieceColor.Black)
            {
                // The black king need to be at his original position, and at least one of the rook as well
                if (kingSquares[0] != "e8" || !rooksSquares.Contains("a8"))
                    return false;

                // In order to castle, you can't have any pieces in between king and rook
                return !colorSquares.Contains("b8") && !colorSquares.Contains("c8") && !colorSquares.Contains("d8");
            }

            return false;
        }
        
        /**
         * Moves patterns
         */
        
        // King
        
        public ulong KingPattern(PieceColor color)
        {
            return KingPattern(Pieces[(int) color][(int) PieceType.King], color);
        }

        private ulong KingPattern(ulong king, PieceColor color)
        {
            ulong colorPieces = color == PieceColor.White ? WhitePieces : BlackPieces;
            return KingAttackPattern(king) & ~colorPieces;
        }

        private ulong KingAttackPattern(ulong king)
        {
            ulong pattern = EastOne(king) | WestOne(king);
            king |= pattern;
            pattern |= NorthOne(king) | SouthOne(king);

            return pattern;
        }
        
        // Knights

        public ulong KnightsPattern(PieceColor color)
        {
            return KnightsPattern(Pieces[(int) color][(int) PieceType.Knight], color);
        }

        private ulong KnightsPattern(ulong knights, PieceColor color)
        {
            ulong colorPieces = color == PieceColor.White ? WhitePieces : BlackPieces;
            return KnightsAttackPattern(knights) & ~colorPieces;
        }

        private ulong KnightsAttackPattern(ulong knights)
        {
            var east = EastOne(knights);
            var west = WestOne(knights);
            var pattern = (east | west) << 16;
            pattern |= (east | west) >> 16;

            east = EastOne(east);
            west = WestOne(west);
            pattern |= (east | west) << 8;
            pattern |= (east | west) >> 8;

            return pattern;
        }
        
        // Rooks

        public ulong RooksPattern(PieceColor color)
        {
            return RooksPattern(Pieces[(int) color][(int) PieceType.Rook], color);
        }

        private ulong RooksPattern(ulong rooks, PieceColor color)
        {
            ulong colorPieces = color == PieceColor.White ? WhitePieces : BlackPieces;
            return RooksAttackPattern(rooks) & ~colorPieces;
        }

        private ulong RooksAttackPattern(ulong rooks)
        {
            return NorthAttacks(rooks) | SouthAttacks(rooks) | EastAttacks(rooks) | WestAttacks(rooks);
        }
        
        // Bishops

        public ulong BishopsPattern(PieceColor color)
        {
            return BishopsPattern(Pieces[(int) color][(int) PieceType.Bishop], color);
        }

        private ulong BishopsPattern(ulong bishops, PieceColor color)
        {
            ulong colorPieces = color == PieceColor.White ? WhitePieces : BlackPieces;
            return BishopsAttackPattern(bishops) & ~colorPieces;
        }

        private ulong BishopsAttackPattern(ulong bishops)
        {
            return NorthEastAttacks(bishops) | NorthWestAttacks(bishops) |
                   SouthEastAttacks(bishops) | SouthWestAttacks(bishops);
        }
        
        // Queens

        public ulong QueensPattern(PieceColor color)
        {
            return QueensPattern(Pieces[(int) color][(int) PieceType.Queen], color);
        }

        private ulong QueensPattern(ulong queens, PieceColor color)
        {
            ulong colorPieces = color == PieceColor.White ? WhitePieces : BlackPieces;
            return QueensAttackPattern(queens) & ~colorPieces;
        }

        private ulong QueensAttackPattern(ulong queens)
        {
            return NorthAttacks(queens) | SouthAttacks(queens)         |
                   EastAttacks(queens) | WestAttacks(queens)           |
                   NorthEastAttacks(queens) | NorthWestAttacks(queens) |
                   SouthEastAttacks(queens) | SouthWestAttacks(queens);
        }
        
        // Pawns

        public ulong WhitePawnsPattern()
        {
            return WhitePawnsSinglePush() | WhitePawnsDoublePush() |
                   (WhitePawnsSingleAttacks() & BlackPawns);
        }

        public ulong BlackPawnsPattern()
        {
            return BlackPawnsSinglePush() | BlackPawnsDoublePush() |
                   (BlackPawnsSingleAttacks() & WhitePawns);
        }

        private ulong PawnPattern(ulong pawn, PieceColor color)
        {
            var oppPieces = color == PieceColor.White ? BlackPieces : WhitePieces;
            return PawnSinglePush(pawn, color) | PawnDoublePush(pawn, color) |
                   (PawnSingleAttacks(pawn, color) & oppPieces);
        }

        private ulong PawnSinglePush(ulong pawn, PieceColor color)
        {
            if (color == PieceColor.White)
                return NorthOne(pawn) & EmptySpace;
            else
                return SouthOne(pawn) & EmptySpace;
        }

        private ulong PawnDoublePush(ulong pawn, PieceColor color)
        {
            if (color == PieceColor.White)
                return NorthOne(PawnSinglePush(pawn, color)) & EmptySpace & Row4;
            else
                return SouthOne(PawnSinglePush(pawn, color)) & EmptySpace & Row5;
        }

        private ulong PawnSingleAttacks(ulong pawn, PieceColor color)
        {
            if (color == PieceColor.White)
                return NorthEastOne(pawn) ^ NorthWestOne(pawn);
            else
                return SouthEastOne(pawn) ^ SouthWestOne(pawn);
        }
        
        private ulong WhitePawnsSinglePush()
        {
            return NorthOne(WhitePawns) & EmptySpace;
        }

        private ulong WhitePawnsDoublePush()
        {
            return NorthOne(WhitePawnsSinglePush()) & EmptySpace & Row4;
        }

        private ulong BlackPawnsSinglePush()
        {
            return SouthOne(BlackPawns) & EmptySpace;
        }

        private ulong BlackPawnsDoublePush()
        {
            return SouthOne(BlackPawnsSinglePush()) & EmptySpace & Row5;
        }
        
        private ulong WhitePawnsAnyAttacks()
        {
            return WhitePawnsWestAttacks() | WhitePawnsEastAttacks();
        }

        private ulong WhitePawnsDoubleAttacks()
        {
            return WhitePawnsWestAttacks() & WhitePawnsEastAttacks();
        }

        private ulong WhitePawnsSingleAttacks()
        {
            return WhitePawnsWestAttacks() ^ WhitePawnsEastAttacks();
        }

        private ulong BlackPawnsAnyAttacks()
        {
            return BlackPawnsWestAttacks() | BlackPawnsEastAttacks();
        }

        private ulong BlackPawnsDoubleAttacks()
        {
            return BlackPawnsWestAttacks() & BlackPawnsEastAttacks();
        }

        private ulong BlackPawnsSingleAttacks()
        {
            return BlackPawnsWestAttacks() ^ BlackPawnsEastAttacks();
        }

        private ulong WhitePawnsEastAttacks()
        {
            return NorthEastOne(WhitePawns);
        }

        private ulong WhitePawnsWestAttacks()
        {
            return NorthWestOne(WhitePawns);
        }

        private ulong BlackPawnsEastAttacks()
        {
            return SouthEastOne(BlackPawns);
        }

        private ulong BlackPawnsWestAttacks()
        {
            return SouthWestOne(BlackPawns);
        }
        
        /**
         * Patterns helper functions.
         */
                
        // Sliding pieces

        private ulong SouthOccl(ulong gen)
        {
            for (var cycle = 0; cycle < 7; cycle++)
                gen |= EmptySpace & (gen << 8);

            return gen;
        }

        private ulong SouthAttacks(ulong gen)
        {
            return SouthOne(SouthOccl(gen));
        }
        
        private ulong NorthOccl(ulong gen)
        {
            for (var cycle = 0; cycle < 7; cycle++)
                gen |= EmptySpace & (gen >> 8);

            return gen;
        }

        private ulong NorthAttacks(ulong gen)
        {
            return NorthOne(NorthOccl(gen));
        }
        
        private ulong EastOccl(ulong gen)
        {
            ulong empty = EmptySpace & ~ACol;
            
            for (var cycle = 0; cycle < 7; cycle++)
                gen |= empty & (gen << 1);

            return gen;
        }

        private ulong EastAttacks(ulong gen)
        {
            return EastOne(EastOccl(gen));
        }

        private ulong WestOccl(ulong gen)
        {
            ulong empty = EmptySpace & ~HCol;
            
            for (var cycle = 0; cycle < 7; cycle++)
                gen |= empty & (gen >> 1);

            return gen;
        }
        
        private ulong WestAttacks(ulong gen)
        {
            return WestOne(WestOccl(gen));
        }
        
        private ulong NorthEastOccl(ulong gen)
        {
            ulong empty = EmptySpace & ~ACol;
            
            for (var cycle = 0; cycle < 7; cycle++)
                gen |= empty & (gen >> 7);

            return gen;
        }

        private ulong NorthEastAttacks(ulong gen)
        {
            return NorthEastOne(NorthEastOccl(gen));
        }
        
        private ulong NorthWestOccl(ulong gen)
        {
            ulong empty = EmptySpace & ~HCol;
            
            for (var cycle = 0; cycle < 7; cycle++)
                gen |= empty & (gen >> 9);

            return gen;
        }
        
        private ulong NorthWestAttacks(ulong gen)
        {
            return NorthWestOne(NorthWestOccl(gen));
        }
        
        private ulong SouthEastOccl(ulong gen)
        {
            ulong empty = EmptySpace & ~ACol;
            
            for (var cycle = 0; cycle < 7; cycle++)
                gen |= empty & (gen << 9);

            return gen;
        }
        
        private ulong SouthEastAttacks(ulong gen)
        {
            return SouthEastOne(SouthEastOccl(gen));
        }
        
        private ulong SouthWestOccl(ulong gen)
        {
            ulong empty = EmptySpace & ~HCol;
            
            for (var cycle = 0; cycle < 7; cycle++)
                gen |= empty & (gen << 7);

            return gen;
        }
        
        private ulong SouthWestAttacks(ulong gen)
        {
            return SouthWestOne(SouthWestOccl(gen));
        }
        
        // Only one step moves
        
        private ulong SouthOne(ulong bb)
        {
            return bb << 8;
        }
        
        private ulong NorthOne(ulong bb)
        {
            return bb >> 8;
        }
        
        private ulong EastOne(ulong bb)
        {
            return (bb << 1) & ~ACol;
        }

        private ulong WestOne(ulong bb)
        {
            return (bb >> 1) & ~HCol;
        }

        private ulong NorthEastOne(ulong bb)
        {
            return (bb >> 7) & ~ACol;
        }

        private ulong SouthEastOne(ulong bb)
        {
            return (bb << 9) & ~ACol;
        }

        private ulong NorthWestOne(ulong bb)
        {
            return (bb >> 9) & ~HCol;
        }

        private ulong SouthWestOne(ulong bb)
        {
            return (bb << 7) & ~HCol;
        }
        
        /**
         * Attcks-related functions.
         */
        
        private bool IsAttacked(ulong targetSquares, bool whiteAttacking)
        {
            ulong remainingTargetSquares = targetSquares;
            ulong targetSquareBb, slidingAttackers;
            int targetSquare;

            if (whiteAttacking)
            {
                while (remainingTargetSquares != 0)
                {
                    targetSquare = BitScanForward(remainingTargetSquares);
                    targetSquareBb = (ulong) 1 << targetSquare;

                    // Is it attacked by a pawn, knight or king?

                    if ((WhitePawns & BlackPawnsPossiblyAttacking(targetSquare)) != 0)
                        return true;

                    if ((WhiteKnights & KnightsPossiblyAttacking(targetSquare)) != 0)
                        return true;

                    if ((WhiteKing & KingsPossiblyAttacking(targetSquare)) != 0)
                        return true;

                    // Is it attacked by a queen or a rook?

                    slidingAttackers = WhiteQueens | WhiteRooks;

                    if (slidingAttackers != 0)
                    {
                        if (((EastAttacks(targetSquareBb) | WestAttacks(targetSquareBb)) & slidingAttackers) != 0)
                            return true;

                        if (((NorthAttacks(targetSquareBb) | SouthAttacks(targetSquareBb)) & slidingAttackers) != 0)
                            return true;
                    }

                    // Is it attacked by a queen or a bishop?

                    slidingAttackers = WhiteQueens | WhiteBishops;

                    if (slidingAttackers != 0)
                    {
                        if (((NorthEastAttacks(targetSquareBb) | SouthWestAttacks(targetSquareBb)) &
                             slidingAttackers) != 0)
                            return true;

                        if (((NorthWestAttacks(targetSquareBb) | SouthWestAttacks(targetSquareBb)) &
                             slidingAttackers) != 0)
                            return true;
                    }

                    remainingTargetSquares ^= targetSquareBb;
                }
            }
            else
            {
                while (remainingTargetSquares != 0)
                {
                    targetSquare = BitScanForward(remainingTargetSquares);
                    targetSquareBb = (ulong) 1 << targetSquare;
                    
                    // Is it attacked by a pawn, knight or king?

                    if ((BlackPawns & WhitePawnsPossiblyAttacking(targetSquare)) != 0)
                        return true;

                    if ((BlackKnights & KnightsPossiblyAttacking(targetSquare)) != 0)
                        return true;

                    if ((BlackKing & KingsPossiblyAttacking(targetSquare)) != 0)
                        return true;

                    // Is it attacked by a queen or a rook?
                    
                    slidingAttackers = BlackQueens | BlackRooks;

                    if (slidingAttackers != 0)
                    {
                        if (((EastAttacks(targetSquareBb) | WestAttacks(targetSquareBb)) & slidingAttackers) != 0)
                            return true;

                        if (((NorthAttacks(targetSquareBb) | SouthAttacks(targetSquareBb)) & slidingAttackers) != 0)
                            return true;
                    }
                    
                    // Is it attacked by a queen or a bishop?
                    
                    slidingAttackers = BlackQueens | BlackBishops;

                    if (slidingAttackers != 0)
                    {
                        if (((NorthEastAttacks(targetSquareBb) | SouthWestAttacks(targetSquareBb)) &
                             slidingAttackers) != 0)
                            return true;

                        if (((NorthWestAttacks(targetSquareBb) | SouthWestAttacks(targetSquareBb)) &
                             slidingAttackers) != 0)
                            return true;
                    }

                    remainingTargetSquares ^= targetSquareBb;
                }
            }

            return false;
        }

        private ulong WhitePawnsPossiblyAttacking(int squareIndex)
        {
            ulong targetSquareBB = (ulong) 1 << squareIndex;
            ulong whitePawns = 0;
            
            whitePawns |= SouthEastOne(targetSquareBB);
            whitePawns |= SouthWestOne(targetSquareBB);

            return whitePawns;
        }
        
        private ulong BlackPawnsPossiblyAttacking(int squareIndex)
        {
            ulong targetSquareBB = (ulong) 1 << squareIndex;
            ulong blackPawns = 0;
            
            blackPawns |= NorthEastOne(targetSquareBB);
            blackPawns |= NorthWestOne(targetSquareBB);

            return blackPawns;
        }

        private ulong KnightsPossiblyAttacking(int squareIndex)
        {
            return KnightsAttackPattern((ulong) 1 << squareIndex);
         }

        private ulong KingsPossiblyAttacking(int squareIndex)
        {
            return KingAttackPattern((ulong) 1 << squareIndex);
        }
        
        /**
         * Helper functions.
         */

        private int Count(ulong bb)
        {
            var count = 0;

            while (bb != 0)
            {
                count++;
                bb &= bb - 1;  // Reset LS1B (least significant 1 bit)
            }
            
            return count;
        }

        private int BitScanForward(ulong bb)
        {
            if (bb == 0)
                return -1;
            
            const ulong magic = 0x03f79d71b4cb0a89;
            return Index64[((bb ^ (bb-1)) * magic) >> 58];
        }

        private static string Trace(ulong bb)
        {
            var sb = new StringBuilder();
            
            for (short i = 0; i < 64; i++)
            {
                if (i % 8 == 0)
                    sb.Append("\n");
                
                sb.Append((bb >> i) & 1);
                sb.Append(" ");
            }
            
             return sb.ToString();
        }

        public override string ToString()
        {
            return Trace(OccupiedSpace);
        }

        public static void Main2(string[] args)
        {
            int[] pieces = {
                -LeftRook, -LeftKnight,-Bishop,     -Queen, -King, -Bishop,     -RightKnight, -RightRook,
                -Pawn,     -Pawn,      -Pawn,       -Pawn,  -Pawn, -Pawn,       -Pawn,        -Pawn,
                0,         0,           0,          0,      0,     0,           0,            0,
                0,         0,           0,          0,      0,     0,           0,            0,
                0,         0,           0,          0,      0,     0,           0,            0,
                0,         0,           0,          0,      0,     0,           0,            0,
                Pawn,      Pawn,        Pawn,       Pawn,   Pawn,  Pawn,        Pawn,         Pawn,
                LeftRook,  LeftKnight,  Bishop,     Queen,  King,  Bishop,      RightKnight,  RightRook
            };
            
            var board = new Board(pieces);
            var whiteMoves = board.GenerateMovesFor(PieceColor.White);

            board.PlayMove(whiteMoves[0]);

            Console.WriteLine(board);

            board.Undo();
            
            Console.WriteLine(board);
        }
    }
}