// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerMovedHandler.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A delegate for the use with the PlayerMovedHandler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe
{
    /// <summary>
    /// A delegate for the use with the PlayerMovedHandler.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The event args.</param>
    public delegate void PlayerMovedHandler(object sender, PlayerMovedArgs args);
}