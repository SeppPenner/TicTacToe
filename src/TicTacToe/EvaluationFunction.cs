// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationFunction.cs" company="H�mmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   This class represents a static evaluation function for Tic Tac Toe.
//   The value is computed by summing number of game pieces in the rows, columns, and diagonals
//   for those rows, columns and diagonals that are still winnable.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe;

/// <summary>
///     This class represents a static evaluation function for Tic Tac Toe.
///     The value is computed by summing number of game pieces in the rows, columns, and diagonals
///     for those rows, columns and diagonals that are still winnable.
/// </summary>
public class EvaluationFunction
{
    /// <summary>
    ///     Gets or sets the number of times the evaluation function has been called.
    /// </summary>
    private int FunctionCalls { get; set; }

    /// <summary>
    ///     Evaluates the favorability of the current board configuration for maxPiece. Higher values
    ///     indicate better configuration for maxPiece.
    /// </summary>
    /// <param name="b">The game board to evaluate.</param>
    /// <param name="maxPiece">The piece representing the maximum.</param>
    /// <returns>A <see cref="double"/> that evaluates the result.</returns>
    public double Evaluate(Board b, Pieces maxPiece)
    {
        this.FunctionCalls++;

        if (b.HasWinner())
        {
            return b.WinningPiece == maxPiece ? double.MaxValue : double.MinValue;
        }

        var maxValue = EvaluatePiece(b, maxPiece);
        var minValue = EvaluatePiece(b, OpponentPieceHelper.GetOpponentPiece(maxPiece));
        return maxValue - minValue;
    }

    /// <summary>
    /// Over all rows sums the number of pieces in the row if 
    /// the specified piece can still win in that row i.e. the row
    /// does not contain an opponent's piece.
    /// </summary>
    /// <param name="board">The board.</param>
    /// <param name="piece">The piece.</param>
    /// <returns>A value that evaluates the rows.</returns>
    private static double EvaluateRows(Board board, Pieces piece)
    {
        var score = 0.0;

        // Check the rows
        for (var i = 0; i < Board.Rows; i++)
        {
            var count = 0;
            var rowClean = true;

            for (var j = 0; j < Board.Columns; j++)
            {
                var boardPiece = board.GetPieceAtPoint(i, j);
                if (boardPiece == piece)
                {
                    count++;
                }
                else if (boardPiece == OpponentPieceHelper.GetOpponentPiece(piece))
                {
                    rowClean = false;
                    break;
                }
            }

            // If we get here then the row is clean (an open row)
            if (rowClean && count != 0)
            {
                score += count;
            }
        }

        return score;
    }

    /// <summary>
    /// Over all rows sums the number of pieces in the row if 
    /// the specified piece can still win in that row i.e. the row
    /// does not contain an opponent's piece.
    /// </summary>
    /// <param name="board">The board.</param>
    /// <param name="piece">The piece.</param>
    /// <returns>A value that evaluates the columns.</returns>
    private static double EvaluateColumns(Board board, Pieces piece)
    {
        var score = 0.0;

        // Check the rows
        for (var j = 0; j < Board.Columns; j++)
        {
            var count = 0;
            var rowClean = true;

            for (var i = 0; i < Board.Columns; i++)
            {
                var boardPiece = board.GetPieceAtPoint(i, j);

                if (boardPiece == piece)
                {
                    count++;
                }
                else if (boardPiece == OpponentPieceHelper.GetOpponentPiece(piece))
                {
                    rowClean = false;
                    break;
                }
            }

            // If we get here then the row is clean (an open row)
            if (rowClean && count != 0)
            {
                // Math.Pow(count, count);
                score += count;
            }
        }

        return score;
    }

    /// <summary>
    /// Over both diagonals sums the number of pieces in the diagonal if 
    /// the specified piece can still win in that diagonal i.e. the diagonal
    /// does not contain an opponent's piece.
    /// </summary>
    /// <param name="board">The board.</param>
    /// <param name="piece">The piece.</param>
    /// <returns>A value that evaluates the diagonals.</returns>
    private static double EvaluateDiagonals(Board board, Pieces piece)
    {
        // Go down and to the right diagonal first
        var count = 0;
        var diagonalClean = true;
        var score = 0.0;

        for (var i = 0; i < Board.Columns; i++)
        {
            var boardPiece = board.GetPieceAtPoint(i, i);

            if (boardPiece == piece)
            {
                count++;
            }

            if (boardPiece != OpponentPieceHelper.GetOpponentPiece(piece))
            {
                continue;
            }

            diagonalClean = false;
            break;
        }

        if (diagonalClean && count > 0)
        {
            // Math.Pow(count, count);
            score += count;
        }

        // Now try the other way
        var row = 0;
        var col = 2;
        count = 0;
        diagonalClean = true;

        while (row < Board.Rows && col >= 0)
        {
            var boardPiece = board.GetPieceAtPoint(row, col);

            if (boardPiece == piece)
            {
                count++;
            }

            if (boardPiece == OpponentPieceHelper.GetOpponentPiece(piece))
            {
                diagonalClean = false;
                break;
            }

            row++;
            col--;
        }

        if (count > 0 && diagonalClean)
        {
            score += count;
        }

        return score;
    }

    /// <summary>
    /// Sums up all the possible ways to win for the specified board piece.
    /// </summary>
    /// <param name="board">The board.</param>
    /// <param name="piece">The piece.</param>
    /// <returns>A value that evaluates the piece.</returns>
    private static double EvaluatePiece(Board board, Pieces piece)
    {
        return EvaluateRows(board, piece) + EvaluateColumns(board, piece) + EvaluateDiagonals(board, piece);
    }
}
