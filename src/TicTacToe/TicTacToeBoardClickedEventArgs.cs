// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TicTacToeBoardClickedEventArgs.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The board clicked event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe
{
    using System;

    /// <inheritdoc cref="EventArgs"/>
    /// <summary>
    /// The board clicked event args.
    /// </summary>
    /// <seealso cref="EventArgs"/>
    public class TicTacToeBoardClickedEventArgs : EventArgs
    {
        /// <inheritdoc cref="EventArgs"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="TicTacToeBoardClickedEventArgs"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <seealso cref="EventArgs"/>
        public TicTacToeBoardClickedEventArgs(int position)
        {
            this.BoardPosition = position;
        }

        /// <summary>
        /// Gets the board position.
        /// </summary>
        public int BoardPosition { get; }
    }
}