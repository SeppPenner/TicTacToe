// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HumanPlayer.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   This class represents a human player.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe;

/// <inheritdoc cref="Player"/>
/// <summary>
///     This class represents a human player.
/// </summary>
/// <seealso cref="Player"/>
public class HumanPlayer : Player
{
    /// <summary>
    /// The Tic Tac Toe form.
    /// </summary>
    private readonly TicTacToeForm ticTacToeForm;

    /// <summary>
    /// A value indicating whether the player has already moved or not.
    /// </summary>
    private bool alreadyMoved;

    /// <summary>
    /// Initializes a new instance of the <see cref="HumanPlayer"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="piece">The piece.</param>
    /// <param name="form">The form.</param>
    public HumanPlayer(string name, Pieces piece, TicTacToeForm form) : base(name, piece)
    {
        this.ticTacToeForm = form;
    }

    /// <inheritdoc cref="Player"/>
    /// <summary>
    ///     Make a move. Waits for the player to double click a square
    ///     and then triggers the PlayerMoved event.
    /// </summary>
    /// <param name="gameBoard">The board.</param>
    /// <seealso cref="Player"/>
    public override void Move(object gameBoard)
    {
        // Start listening to clicks
        this.ticTacToeForm.SquareDoubleClicked += this.SquareDoubleClicked;

        // Now wait until the user clicks
        while (!this.alreadyMoved)
        {
        }

        // Reset the flag
        this.alreadyMoved = false;

        // Raise the PlayerMovedEvent
        this.OnPlayerMoved();
    }

    /// <summary>
    /// Handles the square double click event. When a user double clicks a square on the TicTacToeForm,
    /// this method receives the event message the current move is set and the alreadyMoved flag is set
    /// to true so that the which breaks the while loop in the move method.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void SquareDoubleClicked(object sender, TicTacToeBoardClickedEventArgs e)
    {
        // Un-register the double clicked event
        this.ticTacToeForm.SquareDoubleClicked -= this.SquareDoubleClicked;
        this.CurrentMove = new TicTacToeMove(e.BoardPosition, this.PlayerPiece);
        this.alreadyMoved = true;
    }
}
