// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpponentPieceHelper.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A class to be used as helper class for the <see cref="Pieces"/>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe;

/// <summary>
/// A class to be used as helper class for the <see cref="Pieces"/>.
/// </summary>
public static class OpponentPieceHelper
{
    /// <summary>
    ///     Gets the piece representing the opponent.
    /// </summary>
    /// <param name="yourPiece">The piece representing the player.</param>
    /// <returns>Returns the <see cref="Pieces"/> representing the opponent.</returns>
    public static Pieces GetOpponentPiece(Pieces yourPiece)
    {
        return yourPiece switch
        {
            Pieces.X => Pieces.O,
            Pieces.O => Pieces.X,
            Pieces.Empty => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };
    }
}
