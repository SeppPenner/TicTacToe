using System;

namespace TicTacToe
{
    /// <inheritdoc />
    /// <summary>
    ///     This class represents a "computer" player.
    ///     It determines moves using minmax decision rules
    /// </summary>
    public class ComputerPlayer : Player
    {
        private const int DefaultSearchDepth = 2;

        /// <summary>
        ///     Constructs a new computer player
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <param name="p">The piece the player is using</param>
        public ComputerPlayer(string name, Pieces p) : base(name, p)
        {
        }

        /// <summary>
        ///     Start the computer searching for a move
        ///     Clients should listen to the OnPlayerMoved event to be notified
        ///     when the computer has found a move
        /// </summary>
        /// <param name="gameBoard">The current game board</param>
        public override void Move(object gameBoard)
        {
            var b = (Board) gameBoard;
            //To make things interesting we move randomly if the board we
            //are going first (i.e. the board is empty)
            if (b.OpenPositions.Length == 9)
            {
                CurrentMoveLocalVar = GetRandomMove((Board) gameBoard);
                OnPlayerMoved();
                return;
            }
            Node root = new MaxNode(b, null, null);
            root.MyPiece = PlayerPiece;
            root.Evaluator = new EvaluationFunction();
            root.FindBestMove(DefaultSearchDepth);
            CurrentMoveLocalVar = root.BestMove;
            OnPlayerMoved();
        }

        // Gets a random move. This can be used to make the game play interesting
        // particularly in the beginning of the game or if you wish to weaken the computer's
        // play by adding randomness.
        private TicTacToeMove GetRandomMove(Board b)
        {
            var openPositions = b.OpenPositions.Length;
            var rGen = new Random();
            var squareToMoveTo = rGen.Next(openPositions);
            var move = new TicTacToeMove(squareToMoveTo, PlayerPiece);
            return move;
        }
    }
}