// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TicTacToeMove.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   Represents a Tic Tac Toe move.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Represents a Tic Tac Toe move.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
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