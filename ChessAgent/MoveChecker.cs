using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessAgent
{
    public abstract class MoveChecker
    {
        public readonly string[] LegalColumns = {"a", "b", "c", "d", "e", "f", "g", "h" };
        public readonly string[] LegalRows = {"1", "2", "3", "4", "5", "6", "7", "8"};
        
        /// <summary>
        /// Check if the move is legal, based on the piece type.
        /// </summary>
        /// <param name="from">Source position.</param>
        /// <param name="to">Destination position.</param>
        /// <returns>True if the move is in the board, false otherwise.</returns>
        public abstract bool IsMoveLegal(string from, string to);

        /// <summary>
        /// Check if a position is in the board.
        /// </summary>
        /// <param name="pos">A position.</param>
        /// <returns>True if the position is in the board, false otherwise.</returns>
        public bool IsInBoard(string pos)
        {
            return LegalRows.Contains(pos[1].ToString()) && LegalColumns.Contains(pos[0].ToString());
        }

        public List<string> GenerateOtherRowPositions(string position)
        {
            var pos = new List<string>();
            var temp = new[] {"", position[1].ToString()};

            foreach (var col in LegalColumns)
            {
                if (position[0].ToString() == col) continue;
                
                temp[0] = col;
                pos.Add(string.Concat(temp[0], temp[1]));
            }

            return pos;
        }
        
        public List<string> GenerateOtherColumnPositions(string position)
        {
            var pos = new List<string>();
            var temp = new[] {position[0].ToString(), ""};

            foreach (var row in LegalRows)
            {
                if (position[1].ToString() == row) continue;
                
                temp[1] = row;
                pos.Add(string.Concat(temp[0], temp[1]));
            }

            return pos;
        }

        public List<string> GenerateOtherDiagPositions(string position)
        {
            var pos = new List<string>();
            var colIndex = Array.IndexOf(LegalColumns, position[0].ToString());
            var rowIndex = Array.IndexOf(LegalRows, position[1].ToString());
            
            // TODO: One position is missing sometimes 
            
            for (var i = 1; i <= 8; i++)
            {
                if (position[1].ToString() == LegalRows[i-1] || position[0].ToString() == LegalColumns[i-1])
                    continue;

                if (colIndex + i < 8 && rowIndex - i >= 0)
                {
                    pos.Add(string.Concat(LegalColumns[colIndex + i], LegalRows[rowIndex - i]));
                    //Console.Write(string.Concat(LegalColumns[colIndex + i], LegalRows[rowIndex - i]) + " ");
                }

                if (colIndex + i < 8 && rowIndex + i < 8)
                {
                    pos.Add(string.Concat(LegalColumns[colIndex + i], LegalRows[rowIndex + i]));
                    //Console.Write(string.Concat(LegalColumns[colIndex + i], LegalRows[rowIndex + i]) + " ");
                }

                if (colIndex - i >= 0 && rowIndex - i >= 0)
                {
                    pos.Add(string.Concat(LegalColumns[colIndex - i], LegalRows[rowIndex - i]));
                    //Console.Write(string.Concat(LegalColumns[colIndex - i], LegalRows[rowIndex - i]) + " ");
                }

                if (colIndex - i >= 0 && rowIndex + i < 8)
                {
                    pos.Add(string.Concat(LegalColumns[colIndex - i], LegalRows[rowIndex + i]));
                    //Console.Write(string.Concat(LegalColumns[colIndex - i], LegalRows[rowIndex + i]) + " ");
                }
            }
            //Console.WriteLine();
            return pos;
        }
    }
}