using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TicTacToe
{
    /// <summary>
    ///     This class represents a Tic-Tac-Toe game board.  It includes logic
    ///     to keep track of player turns and assign board squares to a player
    /// </summary>
    public class TicTacToeGame
    {
        private readonly Stack<TicTacToeMove> _moves;
        private readonly Pieces _player1Piece; // Player 1 uses X and player 2 uses O by default
        private readonly Pieces _player2Piece;

        /// <inheritdoc />
        /// <summary>
        ///     Constructs a new TicTacToeGame using the default board pieces for player one and two
        /// </summary>
        public TicTacToeGame() : this(Pieces.X, Pieces.O)
        {
        }

        /// <summary>
        ///     Constructs a new TicTacToe game using the specified player's pieces.
        /// </summary>
        /// <param name="player1Piece">Player one's piece</param>
        /// <param name="player2Piece">Player two's piece</param>
        public TicTacToeGame(Pieces player1Piece, Pieces player2Piece)
        {
            _player1Piece = player1Piece;
            _player2Piece = player2Piece;
            GameBoard = new Board();
            _moves = new Stack<TicTacToeMove>();
        }

        /// <summary>
        ///     Gets the Board associated with this game
        /// </summary>
        public Board GameBoard { get; }

        /// <summary>
        ///     Gets number of columns on the board
        /// </summary>
        public int Columns => Board.Columns;

        /// <summary>
        ///     Gets the number of rows on the game board
        /// </summary>
        public int Rows => Board.Rows;

        /// <summary>
        ///     Returns the player for whose turn it is
        /// </summary>
        public Players CurrentPlayerTurn { get; private set; } = Players.Player1;


        /// <summary>
        ///     Returns true if the game is over (if there is a winner or there is a draw)
        /// </summary>
        /// <returns>true if the game is over or false otherwise</returns>
        public bool IsGameOver()
        {
            return GameBoard.IsGameOver();
        }

        /// <summary>
        ///     Makes the specified move
        /// </summary>
        /// <param name="m">The TicTacToe move to be made</param>
        public void MakeMove(TicTacToeMove m)
        {
            MakeMove(m, GetPlayerWhoHasPiece(m.Piece));
        }

        /// <summary>
        ///     Makes the move for the specified player
        /// </summary>
        /// <param name="m">The move to make</param>
        /// <param name="p">The player making the move</param>
        private void MakeMove(TicTacToeMove m, Players p)
        {
            if (CurrentPlayerTurn != p)
                throw new InvalidMoveException("You went out of turn!");
            if (!GameBoard.IsValidSquare(m.Position))
                throw new InvalidMoveException("Pick a square on the board!");
            if (!Enum.IsDefined(typeof(Players), p))
                throw new InvalidEnumArgumentException(nameof(p), (int) p, typeof(Players));
            GameBoard.MakeMove(m.Position, m.Piece);
            _moves.Push(m);
            SwapTurns();
        }


        /// <summary>
        ///     This should not be called by clients.  This is only for unit testing
        /// </summary>
        /// <param name="position">The position to take</param>
        /// <param name="p">The player who is taking the piece</param>
        public void TakeSquare(int position, Players p)
        {
            if (CurrentPlayerTurn != p)
                throw new InvalidMoveException("You tried to move out of turn!");
            if (!GameBoard.IsValidSquare(position))
                throw new InvalidMoveException();
            GameBoard.MakeMove(position, GetPlayersPiece(p));
            SwapTurns();
        }

        // Returns the game piece for the specified player
        private Pieces GetPlayersPiece(Players p)
        {
            return p == Players.Player1 ? _player1Piece : _player2Piece;
        }

        // returns the Player who has the specified piece
        private Players GetPlayerWhoHasPiece(Pieces piece)
        {
            return piece == _player1Piece ? Players.Player1 : Players.Player2;
        }

        // Swap whose turn it is.
        // If X just moved we make it O's turn and
        // vice versa
        private void SwapTurns()
        {
            CurrentPlayerTurn = CurrentPlayerTurn == Players.Player1 ? Players.Player2 : Players.Player1;
        }
    }
}