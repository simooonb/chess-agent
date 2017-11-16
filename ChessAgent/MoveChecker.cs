using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessAgent
{
    public abstract class MoveChecker
    {
        public readonly string[] LegalColumns = {"a", "b", "c", "d", "e", "f", "g", "h" };
        public readonly string[] LegalRows = {"1", "2", "3", "4", "5", "6", "7", "8"};
        public List<string> OpponentPieces;
        public List<string> OwnPieces;

        /// <summary>
        /// Generate legal moves for a piece.
        /// </summary>
        /// <param name="from">Source position.</param>
        /// <param name="color">Color of the piece.</param>
        /// <returns>A list of legal moves for this piece.</returns>
        public abstract List<string> LegalMoves(string from, PieceColor color);

        /// <summary>
        /// Check if the move is legal, based on the piece type.
        /// </summary>
        /// <param name="from">Source position.</param>
        /// <param name="to">Destination position.</param>
        /// <param name="color">The piece color</param>
        /// <returns>True if the move is in the board, false otherwise.</returns>
        public abstract bool IsMoveLegal(string from, string to, PieceColor color);
        
        /// <summary>
        /// Check if a position is in the board.
        /// </summary>
        /// <param name="pos">A position.</param>
        /// <returns>True if the position is in the board, false otherwise.</returns>
        public bool IsInBoard(string pos)
        {
            return LegalRows.Contains(pos[1].ToString()) && LegalColumns.Contains(pos[0].ToString());
        }

        /// <summary>
        /// Generates a list with others positions on the same row, limited by the opponent's pieces.
        /// </summary>
        /// <param name="position">Origin's position.</param>
        /// <returns>A list containing positions of other positions.</returns>
        public List<string> GenerateOtherRowPositions(string position)
        {
            var pos = new List<string>();
            int oppNegMax = -8, oppPosMin = 8;  // Default values
            int ownNegMax = -8, ownPosMin = 8;

            var ownPiecesOnSameRow = OwnPieces.Where(piece => position[1].Equals(piece[1])).ToList();
            var opponentPiecesOnSameRow = OpponentPieces.Where(piece => position[1].Equals(piece[1])).ToList();
            
            // Find the closest pieces (own and opponent) to the position
            
            foreach (var piece in opponentPiecesOnSameRow)
            {
                var substractCols = piece[0] - position[0];
                
                if (substractCols > 0 && oppPosMin > substractCols)
                    oppPosMin = substractCols;
                else if (substractCols < 0 && oppNegMax < substractCols)
                    oppNegMax = substractCols;
            }

            foreach (var piece in ownPiecesOnSameRow)
            {
                // Same position
                if (piece.Equals(position)) continue;
                
                var substractCols = piece[0] - position[0];

                if (substractCols > 0 && ownPosMin > substractCols)
                    ownPosMin = substractCols;
                else if (substractCols < 0 && ownNegMax < substractCols)
                    ownNegMax = substractCols;
            }

            // +1 or -1 in order to not include our own piece's position
            var closestNeg = Math.Max(ownNegMax + 1, oppNegMax);
            var closestPos = Math.Min(ownPosMin - 1, oppPosMin);
            var positionColIndex = Array.IndexOf(LegalColumns, position[0].ToString());

            for (var colIndexOffset = closestNeg; colIndexOffset <= closestPos; colIndexOffset++)
            {
                var colIdx = positionColIndex + colIndexOffset;
                
                // Not in board
                if (colIdx < 0 || colIdx > 7) continue;
                
                // Same position
                if (colIdx.Equals(positionColIndex)) continue;
                
                var tempPos = string.Concat(LegalColumns[colIdx], position[1]);
                pos.Add(tempPos);
            }

            return pos;
        }
        
        /// <summary>
        /// Generates a list with others positions on the same column, limited by the opponent's pieces.
        /// </summary>
        /// <param name="position">Origin's position.</param>
        /// <returns>A list containing positions of other positions.</returns>
        public List<string> GenerateOtherColumnPositions(string position)
        {
            var pos = new List<string>();
            int oppNegMax = -8, oppPosMin = 8;  // Default values
            int ownNegMax = -8, ownPosMin = 8;

            var ownPiecesOnSameCol = OwnPieces.Where(piece => position[0].Equals(piece[0])).ToList();
            var opponentPiecesOnSameCol = OpponentPieces.Where(piece => position[0].Equals(piece[0])).ToList();
            
            // Find the closest pieces (own and opponent) to the position
            
            foreach (var piece in opponentPiecesOnSameCol)
            {
                var substractRows = piece[1] - position[1];
                
                if (substractRows > 0 && oppPosMin > substractRows)
                    oppPosMin = substractRows;
                else if (substractRows < 0 && oppNegMax < substractRows)
                    oppNegMax = substractRows;
            }

            foreach (var piece in ownPiecesOnSameCol)
            {
                // Same position
                if (piece.Equals(position)) continue;
                
                var substractRows = piece[1] - position[1];

                if (substractRows > 0 && ownPosMin > substractRows)
                    ownPosMin = substractRows;
                else if (substractRows < 0 && ownNegMax < substractRows)
                    ownNegMax = substractRows;
            }

            // +1 or -1 in order to not include our own piece's position
            var closestNeg = Math.Max(ownNegMax + 1, oppNegMax);
            var closestPos = Math.Min(ownPosMin - 1, oppPosMin);
            var positionRowIndex = Array.IndexOf(LegalRows, position[1].ToString());

            for (var rowIndexOffset = closestNeg; rowIndexOffset <= closestPos; rowIndexOffset++)
            {
                var rowIdx = positionRowIndex + rowIndexOffset;
                
                // Not in board
                if (rowIdx < 0 || rowIdx > 7) continue;
                
                // Same position
                if (rowIdx.Equals(positionRowIndex)) continue;
                
                var tempPos = string.Concat(position[0], LegalRows[rowIdx]);
                pos.Add(tempPos);
            }

            return pos;
        }

        /// <summary>
        /// Generates a list with others positions of the two diagonals, limited by the opponent's pieces.
        /// </summary>
        /// <param name="position">Origin's position.</param>
        /// <returns>A list containing positions of other positions.</returns>
        public List<string> GenerateOtherDiagPositions(string position)
        {
            var pos = new List<string>();
            int oppNegMax = -8, oppPosMin = 8;  // Default values
            int ownNegMax = -8, ownPosMin = 8;

            var ownPiecesOnSameDiag = OwnPieces.Where(piece =>
                Math.Abs(int.Parse((piece[1] - position[1]).ToString())) == 
                Math.Abs(int.Parse((piece[0] - position[0]).ToString()))).ToList();

            var opponentPiecesOnSameDiag = OpponentPieces.Where(piece => 
                Math.Abs(int.Parse((piece[1] - position[1]).ToString())) == 
                Math.Abs(int.Parse((piece[0] - position[0]).ToString()))).ToList();
            
            // Find the closest pieces (own and opponent) to the position
            
            foreach (var piece in opponentPiecesOnSameDiag)
            {
                var offset = piece[1] - position[1];
                
                if (offset > 0 && oppPosMin > offset)
                    oppPosMin = offset;
                else if (offset < 0 && oppNegMax < offset)
                    oppNegMax = offset;
            }

            foreach (var piece in ownPiecesOnSameDiag)
            {
                // Same position
                if (piece.Equals(position)) continue;
                
                var offset = piece[1] - position[1];

                if (offset > 0 && ownPosMin > offset)
                    ownPosMin = offset;
                else if (offset < 0 && ownNegMax < offset)
                    ownNegMax = offset;
            }

            // +1 or -1 in order to not include our own piece's position
            var closestNeg = Math.Max(ownNegMax + 1, oppNegMax);
            var closestPos = Math.Min(ownPosMin - 1, oppPosMin);
            var positionRowIndex = Array.IndexOf(LegalRows, position[1].ToString());
            var positionColIndex = Array.IndexOf(LegalColumns, position[0].ToString());

            for (var offset = closestNeg; offset <= closestPos; offset++)
            {
                var rowIdx = positionRowIndex + offset;
                var colIdx = positionColIndex + offset;
                
                // Not in board
                if (rowIdx < 0 || rowIdx > 7 || colIdx < 0 || colIdx > 7) continue;
                
                // Same position
                if (rowIdx.Equals(positionRowIndex) && colIdx.Equals(positionColIndex)) continue;
                
                var tempPos = string.Concat(LegalColumns[colIdx], LegalRows[rowIdx]);
                pos.Add(tempPos);
            }

            return pos;
        }
    }
}