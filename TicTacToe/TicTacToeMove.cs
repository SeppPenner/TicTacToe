namespace TicTacToe
{
    /// <summary>
    ///     Represents a TicTacToe move
    /// </summary>
    public class TicTacToeMove
    {
        /// <summary>
        ///     Constructs a TicTacToeMove
        /// </summary>
        /// <param name="position">The position to move to</param>
        /// <param name="piece">The piece that is moving</param>
        public TicTacToeMove(int position, Pieces piece)
        {
            Position = position;
            Piece = piece;
        }

        /// <summary>
        ///     Gets or sets the position on the board
        /// </summary>
        public int Position { get; }

        /// <summary>
        ///     Gets or sets the piece making this move
        /// </summary>
        public Pieces Piece { get; }
    }
}