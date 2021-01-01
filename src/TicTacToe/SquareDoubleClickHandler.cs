// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SquareDoubleClickHandler.cs" company="H�mmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A delegate for the use with the SquareDoubleClickHandler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe
{
    /// <summary>
    /// A delegate for the use with the SquareDoubleClickHandler.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    public delegate void SquareDoubleClickHandler(object sender, TicTacToeBoardClickedEventArgs e);
}