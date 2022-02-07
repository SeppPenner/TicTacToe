// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TicTacToeMove.cs" company="H�mmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   Represents a Tic Tac Toe move.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe
{
    /// <summary>
    ///     Represents a Tic Tac Toe move.
    /// </summary>
    public class TicTacToeMove
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TicTacToeMove"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="piece">The piece.</param>
        public TicTacToeMove(int position, Pieces piece)
        {
            this.Position = position;
            this.Piece = piece;
        }

        /// <summary>
        ///     Gets the position on the board.
        /// </summary>
        public int Position { get; }

        /// <summary>
        ///     Gets the piece making this move.
        /// </summary>
        public Pieces Piece { get; }
    }
}