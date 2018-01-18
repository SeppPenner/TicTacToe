namespace TicTacToe
{
    // This delegate is used to respond to moves by a player
    public delegate void PlayerMovedHandler(object sender, PlayerMovedArgs args);

    /// <summary>
    ///     This class abstracts the idea of a player and includes some common functionality.
    ///     It includes an event for clients to be notified when a move is made.
    /// </summary>
    public abstract class Player
    {
        protected TicTacToeMove CurrentMoveLocalVar;

        protected Player(string name, Pieces p)
        {
            Name = name;
            PlayerPiece = p;
        }

        public TicTacToeMove CurrentMove => CurrentMoveLocalVar;

        /// <summary>
        ///     Get or set the player's piece
        /// </summary>
        public Pieces PlayerPiece { get; }

        /// <summary>
        ///     Get or set the player's name
        /// </summary>
        public string Name { get; }

        // Listen for a move made by a player
        public event PlayerMovedHandler PlayerMoved;

        public abstract void Move(object gameBoard);

        /// <summary>
        ///     This is invoked by subclasses to indicate that the player decided on a move
        /// </summary>
        protected void OnPlayerMoved()
        {
            PlayerMoved?.Invoke(this, new PlayerMovedArgs(CurrentMoveLocalVar));
        }
    }
}