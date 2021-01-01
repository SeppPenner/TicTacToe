// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TicTacToeGame.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   This class represents a Tic-Tac-Toe game board. It includes logic
//   to keep track of player turns and assign board squares to a player.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using Languages.Interfaces;

    /// <summary>
    ///     This class represents a Tic-Tac-Toe game board. It includes logic
    ///     to keep track of player turns and assign board squares to a player.
    /// </summary>
    public class TicTacToeGame
    {
        /// <summary>
        /// The moves.
        /// </summary>
        private readonly Stack<TicTacToeMove> moves;

        /// <summary>
        /// Player one's piece.
        /// </summary>
        private readonly Pieces player1Piece;
        

        /// <summary>
        /// Player two's piece.
        /// </summary>
        private readonly Pieces player2Piece;

        /// <summary>
        /// The language.
        /// </summary>
        private readonly ILanguage language;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicTacToeGame"/> class.
        /// </summary>
        /// <param name="language">The language.</param>
        // ReSharper disable once UnusedMember.Global
        public TicTacToeGame(ILanguage language)
            : this(Pieces.X, Pieces.O, language)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="TicTacToeGame"/> class.
        /// </summary>
        /// <param name="player1Piece">Player one's piece.</param>
        /// <param name="player2Piece">Player two's piece.</param>
        /// <param name="language">The language.</param>
        public TicTacToeGame(Pieces player1Piece, Pieces player2Piece, ILanguage language)
        {
            this.language = language;
            this.player1Piece = player1Piece;
            this.player2Piece = player2Piece;
            this.GameBoard = new Board(language);
            this.moves = new Stack<TicTacToeMove>();
        }

        /// <summary>
        ///     Gets the board associated with this game.
        /// </summary>
        public Board GameBoard { get; }

        /// <summary>
        ///     Gets the number of columns on the board.
        /// </summary>
        public int Columns => Board.Columns;

        /// <summary>
        ///     Gets the number of rows on the board.
        /// </summary>
        public int Rows => Board.Rows;

        /// <summary>
        ///     Gets the player for whose turn it is.
        /// </summary>
        public Players CurrentPlayerTurn { get; private set; } = Players.Player1;

        /// <summary>
        ///     Gets a value indicating whether the game is over or not.
        /// </summary>
        /// <returns><c>true</c> if the game is over, <c>false</c> else.</returns>
        public bool IsGameOver()
        {
            return this.GameBoard.IsGameOver();
        }

        /// <summary>
        ///     Makes the specified move.
        /// </summary>
        /// <param name="m">The move to be made.</param>
        public void MakeMove(TicTacToeMove m)
        {
            this.MakeMove(m, this.GetPlayerWhoHasPiece(m.Piece));
        }

        /// <summary>
        /// Takes a square. This should not be called by clients. This is only for unit testing.
        /// </summary>
        /// <param name="position">The position to take.</param>
        /// <param name="p">The player who is taking the piece.</param>
        // ReSharper disable once UnusedMember.Global
        public void TakeSquare(int position, Players p)
        {
            if (this.CurrentPlayerTurn != p)
            {
                throw new InvalidMoveException(this.language.GetWord("YouTriedOutOfTurn"));
            }

            if (!this.GameBoard.IsValidSquare(position))
            {
                throw new InvalidMoveException();
            }

            this.GameBoard.MakeMove(position, this.GetPlayersPiece(p));
            this.SwapTurns();
        }

        /// <summary>
        ///     Makes the move for the specified player.
        /// </summary>
        /// <param name="m">The move to make.</param>
        /// <param name="p">The player making the move.</param>
        private void MakeMove(TicTacToeMove m, Players p)
        {
            if (this.CurrentPlayerTurn != p)
            {
                throw new InvalidMoveException(this.language.GetWord("OutOfTurn"));
            }

            if (!this.GameBoard.IsValidSquare(m.Position))
            {
                throw new InvalidMoveException(this.language.GetWord("PickASquare"));
            }

            if (!Enum.IsDefined(typeof(Players), p))
            {
                throw new InvalidEnumArgumentException(nameof(p), (int)p, typeof(Players));
            }

            this.GameBoard.MakeMove(m.Position, m.Piece);
            this.moves.Push(m);
            this.SwapTurns();
        }

        /// <summary>
        /// Gets the piece for the player.
        /// </summary>
        /// <param name="p">The player.</param>
        /// <returns>The <see cref="Pieces"/>.</returns>
        private Pieces GetPlayersPiece(Players p)
        {
            return p == Players.Player1 ? this.player1Piece : this.player2Piece;
        }

        /// <summary>
        /// Gets the player for the piece.
        /// </summary>
        /// <param name="piece">The piece.</param>
        /// <returns>The <see cref="Players"/>.</returns>
        private Players GetPlayerWhoHasPiece(Pieces piece)
        {
            return piece == this.player1Piece ? Players.Player1 : Players.Player2;
        }

        /// <summary>
        /// Swaps who's turn it is.
        /// </summary>
        private void SwapTurns()
        {
            this.CurrentPlayerTurn = this.CurrentPlayerTurn == Players.Player1 ? Players.Player2 : Players.Player1;
        }
    }
}