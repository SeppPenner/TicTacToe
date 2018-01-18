namespace TicTacToe
{
    /// <inheritdoc />
    /// <summary>
    ///     This class represents a Human Player
    /// </summary>
    public class HumanPlayer : Player
    {
        private readonly TicTacToeForm _ticTacToeForm;
        private bool _alreadyMoved;

        public HumanPlayer(string name, Pieces p, TicTacToeForm tttf)
            : base(name, p)
        {
            _ticTacToeForm = tttf;
        }

        /// <summary>
        ///     Make a move. Waits for the player to double click a square
        ///     and then triggers the PlayerMoved Event
        /// </summary>
        /// <param name="gameBoard"></param>
        public override void Move(object gameBoard)
        {
            // Start listening to clicks
            _ticTacToeForm.SquareDoubleClicked += SquareDoubleClicked;
            // Now wait until the user clicks
            while (!_alreadyMoved)
            {
            }
            // Reset the flag
            _alreadyMoved = false;
            // Raise the PlayerMovedEvent
            OnPlayerMoved();
        }

        // When a user double clicks a square on the TicTacToeForm this method receives the 
        // event message the current move is set and the alreadyMoved flag is set to true so that the 
        // which breaks the while loop in the Move method
        private void SquareDoubleClicked(object sender, TicTacToeBoardClickedEventArgs args)
        {
            // Unregister the double clicked event
            _ticTacToeForm.SquareDoubleClicked -= SquareDoubleClicked;
            CurrentMoveLocalVar = new TicTacToeMove(args.BoardPosition, PlayerPiece);
            _alreadyMoved = true;
        }
    }
}