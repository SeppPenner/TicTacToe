namespace TicTacToe
{
    /// <summary>
    ///     This class represents a static evaluation function for Tic-Tac-Toe
    ///     The value is computed by summing number of game pieces in the rows, columns, and diagonals
    ///     for those rows, columns and diagonals that are still winnable
    /// </summary>
    public class EvaluationFunction
    {
        /// <summary>
        ///     Gets the number of times the evaluation function has been called.
        /// </summary>
        private int FunctionCalls { get; set; }

        /// <summary>
        ///     Evaluates the favorability of the current board configuration for maxPiece.  Higher values
        ///     indicate better configuration for maxPiece
        /// </summary>
        /// <param name="b">the game board to evaluate</param>
        /// <param name="maxPiece">the piece representing MAX</param>
        /// <returns></returns>
        public double Evaluate(Board b, Pieces maxPiece)
        {
            FunctionCalls++;
            if (b.HasWinner())
                return b.WinningPiece == maxPiece ? double.MaxValue : double.MinValue;
            var maxValue = EvaluatePiece(b, maxPiece);
            var minValue = EvaluatePiece(b, Board.GetOponentPiece(maxPiece));
            return maxValue - minValue;
        }

        // Sums up all the possible ways to win for the specified board piece
        private double EvaluatePiece(Board b, Pieces p)
        {
            return EvaluateRows(b, p) + EvaluateColumns(b, p) + EvaluateDiagonals(b, p);
        }

        // Over all rows sums the number of pieces in the row if 
        // the specified piece can still win in that row i.e. the row
        // does not contain an opponent's piece
        private double EvaluateRows(Board b, Pieces p)
        {
            var score = 0.0;
            // Check the rows
            for (var i = 0; i < Board.Rows; i++)
            {
                var count = 0;
                var rowClean = true;
                for (var j = 0; j < Board.Columns; j++)
                {
                    var boardPiece = b.GetPieceAtPoint(i, j);
                    if (boardPiece == p)
                    {
                        count++;
                    }
                    else if (boardPiece == Board.GetOponentPiece(p))
                    {
                        rowClean = false;
                        break;
                    }
                }
                // If we get here then the row is clean (an open row)
                if (rowClean && count != 0)
                    score += count;
            }
            return score;
        }

        // Over all rows sums the number of pieces in the row if 
        // the specified piece can still win in that row i.e. the row
        // does not contain an opponent's piece
        private double EvaluateColumns(Board b, Pieces p)
        {
            var score = 0.0;
            // Check the rows
            for (var j = 0; j < Board.Columns; j++)
            {
                var count = 0;
                var rowClean = true;
                for (var i = 0; i < Board.Columns; i++)
                {
                    var boardPiece = b.GetPieceAtPoint(i, j);
                    if (boardPiece == p)
                    {
                        count++;
                    }
                    else if (boardPiece == Board.GetOponentPiece(p))
                    {
                        rowClean = false;
                        break;
                    }
                }
                // If we get here then the row is clean (an open row)
                if (rowClean && count != 0)
                    score += count; //Math.Pow(count, count);
            }
            return score;
        }


        // Over both diagonals sums the number of pieces in the diagonal if 
        // the specified piece can still win in that diagonal i.e. the diagonal
        // does not contain an opponent's piece
        private double EvaluateDiagonals(Board b, Pieces p)
        {
            // Go down and to the right diagonal first
            var count = 0;
            var diagonalClean = true;
            var score = 0.0;
            for (var i = 0; i < Board.Columns; i++)
            {
                var boardPiece = b.GetPieceAtPoint(i, i);
                if (boardPiece == p)
                    count++;
                if (boardPiece != Board.GetOponentPiece(p)) continue;
                diagonalClean = false;
                break;
            }
            if (diagonalClean && count > 0)
                score += count; // Math.Pow(count, count);
            // Now try the other way
            var row = 0;
            var col = 2;
            count = 0;
            diagonalClean = true;
            while (row < Board.Rows && col >= 0)
            {
                var boardPiece = b.GetPieceAtPoint(row, col);
                if (boardPiece == p)
                    count++;
                if (boardPiece == Board.GetOponentPiece(p))
                {
                    diagonalClean = false;
                    break;
                }
                row++;
                col--;
            }
            if (count > 0 && diagonalClean)
                score += count;
            return score;
        }
    }
}