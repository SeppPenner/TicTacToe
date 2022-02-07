// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComputerPlayer.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   This class represents a "computer" player.
//   It determines moves using min max decision rules.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe
{
    using System;

    /// <inheritdoc cref="Player"/>
    /// <summary>
    ///     This class represents a "computer" player.
    ///     It determines moves using min max decision rules.
    /// </summary>
    /// <seealso cref="Player"/>
    public class ComputerPlayer : Player
    {
        /// <summary>
        /// The default search depth.
        /// </summary>
        private const int DefaultSearchDepth = 2;

        /// <summary>
        ///  Initializes a new instance of the <see cref="ComputerPlayer"/> class.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        /// <param name="piece">The piece the player is using.</param>
        public ComputerPlayer(string name, Pieces piece) : base(name, piece)
        {
        }

        /// <inheritdoc cref="Player"/>
        /// <summary>
        ///  Start the computer searching for a move
        ///  Clients should listen to the OnPlayerMoved event to be notified
        ///  when the computer has found a move
        /// </summary>
        /// <param name="gameBoard">The board.</param>
        /// <seealso cref="Player"/>
        public override void Move(object gameBoard)
        {
            var b = (Board)gameBoard;

            // To make things interesting we move randomly if the board we
            // are going first (i.e. the board is empty)
            if (b.OpenPositions.Length == 9)
            {
                this.CurrentMove = this.GetRandomMove((Board)gameBoard);
                this.OnPlayerMoved();
                return;
            }

            var root = new MaximumNode(b, null, null)
            {
                MyPiece = this.PlayerPiece,
                Evaluator = new EvaluationFunction()
            };

            root.FindBestMove(DefaultSearchDepth);
            this.CurrentMove = root.BestMove;
            this.OnPlayerMoved();
        }

        /// <summary>
        /// Gets a random move. This can be used to make the game play interesting
        /// particularly in the beginning of the game or if you wish to weaken the computer's
        /// play by adding randomness.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <returns>A new <see cref="TicTacToeMove"/>.</returns>
        private TicTacToeMove GetRandomMove(Board board)
        {
            var openPositions = board.OpenPositions.Length;
            var random = new Random();
            var squareToMoveTo = random.Next(openPositions);
            var move = new TicTacToeMove(squareToMoveTo, this.PlayerPiece);
            return move;
        }
    }
}