// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Player.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   This class abstracts the idea of a player and includes some common functionality.
//   It includes an event for clients to be notified when a move is made.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe;

/// <summary>
///     This class abstracts the idea of a player and includes some common functionality.
///     It includes an event for clients to be notified when a move is made.
/// </summary>
public abstract class Player
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Player"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="piece">The piece.</param>
    protected Player(string name, Pieces piece)
    {
        this.Name = name;
        this.PlayerPiece = piece;
    }

    /// <summary>
    /// An event to listen for a move made by a player.
    /// </summary>
    public event PlayerMovedHandler? PlayerMoved;

    /// <summary>
    /// Gets or sets the current move.
    /// </summary>
    public TicTacToeMove? CurrentMove { get; protected set; }

    /// <summary>
    ///     Gets the player's piece.
    /// </summary>
    public Pieces PlayerPiece { get; }

    /// <summary>
    ///     Gets the player's name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Make a move. Waits for the player to double click a square
    ///     and then triggers the PlayerMoved event.
    /// </summary>
    /// <param name="gameBoard">The board.</param>
    public abstract void Move(object gameBoard);

    /// <summary>
    ///     This is invoked by subclasses to indicate that the player decided on a move.
    /// </summary>
    protected void OnPlayerMoved()
    {
        if (this.CurrentMove is null)
        {
            return;
        }

        this.PlayerMoved?.Invoke(this, new PlayerMovedArgs(this.CurrentMove));
    }
}
