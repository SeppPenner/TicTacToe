using System;

namespace TicTacToe
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception representing an invalid move
    /// </summary>
    public class InvalidMoveException : Exception
    {
        public InvalidMoveException()
        {
        }

        public InvalidMoveException(string msg)
            : base(msg)
        {
        }
    }
}