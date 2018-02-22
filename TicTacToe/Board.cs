using System;
using System.Collections.Generic;
using System.Drawing;
using Languages.Interfaces;

namespace TicTacToe
{
    /// <inheritdoc />
    /// <summary>
    ///     This class represents a tic-tac-toe board
    ///     It is Cloneable so that we can copy board configurations when searching for a move
    /// </summary>
    public class Board : ICloneable
    {
        private const int ConstColumns = 3;
        private const int ConstRows = 3;
        private const int WinningLength = 3;
        private readonly int[,] _board; // a two-dimensional array representing the game board
        private static ILanguage _lang;

        /// <inheritdoc />
        /// <summary>
        ///     Constructs a new board from a previous game state.
        ///     The game state conventions are as follows:
        ///     the first index indicates the board row, the second index represents
        ///     the column.  For example, gameState[1,2] represents the 2nd row and third column
        ///     of the board.
        ///     A value of 0 indicates an open square on the board
        ///     A value of 1 indicates an 'X' on the board
        ///     A value of 2 indicates an 'O' on the board
        /// </summary>
        /// <param name="gameState">a two-dimensional array representing the game state</param>
        /// <param name="language">The language to be used</param>
        private Board(int[,] gameState, ILanguage language) : this(language)
        {
            _lang = language;
            for (var i = 0; i <= gameState.GetUpperBound(0); i++)
            for (var j = 0; j <= gameState.GetUpperBound(1); j++)
                _board[i, j] = gameState[i, j];
        }

        /// <summary>
        ///     Constructs an empty board
        /// </summary>
        public Board(ILanguage language)
        {
            _lang = language;
            _board = new int[ConstRows, ConstColumns];
        }

        /// <summary>
        ///     Returns the winner's piece (an 'X' or an 'O')
        /// </summary>
        public Pieces WinningPiece { get; private set; }

        /// <summary>
        ///     Returns the number of rows in the game board
        /// </summary>
        public static int Rows => ConstRows;

        /// <summary>
        ///     Returns the number of columns in the game board
        /// </summary>
        public static int Columns => ConstColumns;

        public int[] OpenPositions
        {
            get
            {
                var positions = new List<int>();
                for (var i = 0; i < _board.Length; i++)
                    if (!IsOccupied(i))
                        positions.Add(i);

                return positions.ToArray();
            }
        }

        public object Clone()
        {
            var b = new Board(_board, _lang);
            return b;
        }

        /// <summary>
        ///     Returns true if the position is on the board and currently not occupied by
        ///     an 'X' or an 'O'.  Position 0 is the upper-left most square and increases row-by-row
        ///     so that first square in the second row is position 3 and position and position 8
        ///     is the 3rd square in the 3rd row
        ///     0 1 2
        ///     3 4 5
        ///     6 7 8
        /// </summary>
        /// <param name="position">The position to test</param>
        /// <returns>Returns true if the position is on the board and currently not occupied by an 'X' or an 'O'.</returns>
        public bool IsValidSquare(int position)
        {
            var p = GetPoint(position);
            return p.X >= 0 && p.X < ConstRows && p.Y >= 0 && p.Y < ConstColumns && IsPositionOpen(p.X, p.Y);
        }

        private bool IsOnBoard(int position)
        {
            return IsOccupied(position) || IsValidSquare(position);
        }

        public void UndoMove(TicTacToeMove m)
        {
            if (!IsOnBoard(m.Position))
                throw new InvalidMoveException(_lang.GetWord("CantUndoAMoveOnAnInvalidSquare"));
            // just reset the position
            var p = GetPoint(m.Position);
            _board[p.X, p.Y] = 0;
        }

        /// <summary>
        ///     Make a move on the board
        /// </summary>
        /// <param name="position">the board position to take</param>
        /// <param name="piece"></param>
        public void MakeMove(int position, Pieces piece)
        {
            if (!IsValidSquare(position))
                throw new InvalidMoveException();
            var pieceNumber = GetPieceNumber(piece);
            var point = GetPoint(position);
            _board[point.X, point.Y] = pieceNumber;
        }

        /// <summary>
        ///     Retries the piece at the corresponding row and column on the board
        /// </summary>
        /// <param name="row">The row on the board (0-2)</param>
        /// <param name="column">The column (0-2)</param>
        /// <returns></returns>
        public Pieces GetPieceAtPoint(int row, int column)
        {
            return GetPieceAtPosition(GetPositionFromPoint(new Point(row, column)));
        }

        /// <summary>
        ///     Returns the piece at the given board position
        /// </summary>
        /// <param name="position">
        ///     A number representing the board position.
        ///     0 1 2
        ///     3
        ///     4 5
        ///     6 7 9
        /// </param>
        /// <returns>The piece at the position</returns>
        public Pieces GetPieceAtPosition(int position)
        {
            if (!IsOccupied(position))
                return Pieces.Empty;
            return GetBoardPiece(position) == 1 ? Pieces.X : Pieces.O;
        }

        /// <summary>
        ///     Checks the board to see if there is a winner
        /// </summary>
        /// <returns>True if there is a winner or false otherwise</returns>
        public bool HasWinner()
        {
            for (var i = 0; i < _board.Length; i++)
            {
                if (!IsWinnerAt(i)) continue;
                SetWinnerAtPosition(i);
                return true;
            }
            return false;
        }

        // Maps a board position number to a point containing 
        // the row in the x value and the column in the y value
        private Point GetPoint(int position)
        {
            var p = new Point
            {
                X = position / ConstColumns,
                Y = position % ConstRows
            };


            return p;
        }

        // Gets the internal representation of the
        // piece
        private int GetPieceNumber(Pieces p)
        {
            return p == Pieces.O ? 2 : 1;
        }

        // Returns the position number given the row and colum
        // p.X is the row
        // p.Y is the column
        private int GetPositionFromPoint(Point p)
        {
            return p.X * Columns + p.Y;
        }

        private void SetWinnerAtPosition(int position)
        {
            // Get the piece at i
            WinningPiece = GetPieceAtPosition(position);
        }

        private int GetBoardPiece(int position)
        {
            var p = GetPoint(position);

            return _board[p.X, p.Y];
        }

        // Is the position available
        private bool IsPositionOpen(int row, int col)
        {
            return _board[row, col] == 0;
        }

        private bool IsOccupied(int position)
        {
            var p = GetPoint(position);
            return IsOccupied(p.X, p.Y);
        }

        private bool IsOccupied(int row, int col)
        {
            return !IsPositionOpen(row, col);
        }

        // Helper method for checking for a winner
        private bool IsWinnerAt(int position)
        {
            // Check each position for winner to the right
            return IsWinnerToTheRight(position) || IsWinnerFromTopToBottom(position)
                   || IsWinnerDiagonallyToRightUp(position) || IsWinnerDiagonallyToRightDown(position);
        }

        // Checks for a winner in the diagonal starting from 
        // the bottom-left corner of the board to the upper-right corner
        private bool IsWinnerDiagonallyToRightUp(int position)
        {
            if (!IsOccupied(position))
                return false;
            var piece = GetPieceAtPosition(position);
            var last = GetPoint(position);
            for (var i = 1; i < WinningLength; i++)
            {
                last.X -= 1;
                last.Y += 1;
                if (!IsPointInBounds(last))
                    return false;
                if (piece != GetPieceAtPosition(GetPositionFromPoint(last)))
                    return false;
            }
            return true;
        }

        /// <summary>
        ///     Returns true if there is a winner or there is a draw
        /// </summary>
        /// <returns>Returns true if there is a winner or there is a draw</returns>
        public bool IsGameOver()
        {
            return HaveWinner() || IsDraw();
        }

        /// <summary>
        ///     Returns true if there is a winner
        /// </summary>
        /// <returns>Returns true if there is a winner</returns>
        private bool HaveWinner()
        {
            return HasWinner();
        }

        /// <summary>
        ///     Returns the piece representing the opponent
        /// </summary>
        /// <param name="yourPiece">The piece representing the player</param>
        /// <returns>Returns the piece representing the opponent</returns>
        public static Pieces GetOponentPiece(Pieces yourPiece)
        {
            switch (yourPiece)
            {
                case Pieces.X:
                    return Pieces.O;
                case Pieces.O:
                    return Pieces.X;
            }
            throw new Exception(_lang.GetWord("InvalidPiece"));
        }

        /// <summary>
        ///     Checks the board configuration to see if it is currently a draw.
        ///     A draw occurs when all the positions are full and there isn't a winner.
        /// </summary>
        /// <returns>Returns true if there is a draw and false otherwise</returns>
        public bool IsDraw()
        {
            if (HasWinner())
                return false;
            for (var i = 0; i < _board.Length; i++)
                if (!IsOccupied(i))
                    return false;
            return true;
        }

        // Checks to see if the row and column are on the board
        // p.X is the row, p.Y is the column
        private static bool IsPointInBounds(Point p)
        {
            return p.X >= 0 && p.X < Rows && p.Y >= 0 && p.Y < Columns;
        }

        // Checks for a winner diagonally from the specified position
        // to the right
        private bool IsWinnerDiagonallyToRightDown(int position)
        {
            GetPoint(position);
            if (!IsOccupied(position))
                return false;
            var piece = GetPieceAtPosition(position);
            // Keep moving diagonally until we reach the winningLength
            // or we don't see the same piece
            var last = GetPoint(position);
            for (var i = 1; i < WinningLength; i++)
            {
                last.X += 1;
                last.Y += 1;
                if (!IsPointInBounds(last))
                    return false;
                if (piece != GetPieceAtPosition(GetPositionFromPoint(last)))
                    return false;
            }
            return true;
        }

        // Checks for winner from top to bottom starting the the 
        // specified position
        private bool IsWinnerFromTopToBottom(int position)
        {
            var p = GetPoint(position);
            // Check if we even have a square here
            if (!IsOccupied(position))
                return false;
            // Do we have the room to go down from here?
            if (p.X + WinningLength - 1 >= ConstRows)
                return false;
            var piece = GetPieceAtPosition(position);
            // If we get here then we know we at least have
            // the potential for a winner from top to bottom
            for (var i = 1; i < WinningLength; i++)
                if (piece != GetPieceAtPosition(position + 3 * i))
                    return false;
            return true;
        }

        // Checks for a winner from the specified position to the right
        private bool IsWinnerToTheRight(int position)
        {
            var p = GetPoint(position);
            // Check if we even the square is occupied
            // if it's not then we don't have a winner 
            // starting here
            if (!IsOccupied(position))
                return false;
            // check if we have room to the right?
            if (p.Y + WinningLength - 1 >= ConstColumns)
                return false;
            var piece = GetPieceAtPosition(position);
            for (var i = 1; i < WinningLength; i++)
                if (GetPieceAtPosition(position + i) != piece)
                    return false;
            return true;
        }
    }
}