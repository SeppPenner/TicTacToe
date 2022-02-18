// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidMoveException.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   An exception representing an invalid move
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe;

/// <inheritdoc cref="Exception"/>
/// <summary>
///     An exception representing an invalid move
/// </summary>
/// <seealso cref="Exception"/>
public class InvalidMoveException : Exception
{
    /// <inheritdoc cref="Exception"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidMoveException"/> class.
    /// </summary>
    /// <seealso cref="Exception"/>
    public InvalidMoveException()
    {
    }

    /// <inheritdoc cref="Exception"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidMoveException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <seealso cref="Exception"/>
    public InvalidMoveException(string message) : base(message)
    {
    }
}
