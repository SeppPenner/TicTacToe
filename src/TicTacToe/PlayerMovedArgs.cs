// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerMovedArgs.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A class for encapsulating a player moved
//   This is passed along with PlayerMoved events
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe;

/// <inheritdoc cref="EventArgs"/>
/// <summary>
///     A class for encapsulating a player moved
///     This is passed along with PlayerMoved events
/// </summary>
/// <seealso cref="EventArgs"/>
public class PlayerMovedArgs : EventArgs
{
    /// <inheritdoc cref="EventArgs"/>
    /// <summary>
    ///     Initializes a new instance of the <see cref="PlayerMovedArgs"/> class.
    /// </summary>
    /// <param name="m">The move to make.</param>
    /// <seealso cref="EventArgs"/>
    public PlayerMovedArgs(TicTacToeMove m)
    {
        this.Move = m;
    }

    /// <summary>
    /// Gets the move.
    /// </summary>
    public TicTacToeMove Move { get; }
}
