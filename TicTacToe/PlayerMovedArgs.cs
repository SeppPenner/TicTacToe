using System;

namespace TicTacToe
{
    /// <inheritdoc />
    /// <summary>
    ///     A class for encapuslating a player moved
    ///     This is passed along with PlayerMoved events
    /// </summary>
    public class PlayerMovedArgs : EventArgs
    {
        /// <inheritdoc />
        /// <summary>
        ///     Constructs a new PlayerMovedArgs object with the specified Move and Player
        /// </summary>
        /// <param name="m">The move to make</param>
        public PlayerMovedArgs(TicTacToeMove m)
        {
            Move = m;
        }

        public TicTacToeMove Move { get; }
    }
}