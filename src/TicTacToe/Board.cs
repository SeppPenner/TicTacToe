// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Board.cs" company="H�mmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   This class represents a Tic Tac Toe board.
//   It is Cloneable so that we can copy board configurations when searching for a move.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe;

/// <inheritdoc cref="ICloneable"/>
/// <summary>
///     This class represents a Tic Tac Toe board.
///     It is Cloneable so that we can copy board configurations when searching for a move.
/// </summary>
/// <seealso cref="ICloneable"/>
public class Board : ICloneable
{
    /// <summary>
    /// The number of columns.
    /// </summary>
    private const int NumberOfColumns = 3;

    /// <summary>
    /// The number of rows.
    /// </summary>
    private const int NumberOfRows = 3;

    /// <summary>
    /// The winning length.
    /// </summary>
    private const int WinningLength = 3;

    /// <summary>
    /// The board.
    /// </summary>
    private readonly int[,] board;

    /// <summary>
    /// The language.
    /// </summary>
    private readonly ILanguage language;

    /// <inheritdoc cref="ICloneable"/>
    /// <summary>
    ///     Initializes a new instance of the <see cref="Board"/> class.
    /// </summary>
    /// <param name="languageParam">The language.</param>
    /// <seealso cref="ICloneable"/>
    public Board(ILanguage languageParam)
    {
        this.language = languageParam;
        this.board = new int[NumberOfRows, NumberOfColumns];
    }

    /// <inheritdoc cref="ICloneable"/>
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
    /// <param name="languageParam">The language to be used</param>
    /// <seealso cref="ICloneable"/>
    private Board(int[,] gameState, ILanguage languageParam) : this(languageParam)
    {
        this.language = languageParam;

        for (var i = 0; i <= gameState.GetUpperBound(0); i++)
        {
            for (var j = 0; j <= gameState.GetUpperBound(1); j++)
            {
                this.board[i, j] = gameState[i, j];
            }
        }
    }

    /// <summary>
    ///     Gets the number of rows in the game board.
    /// </summary>
    public static int Rows => NumberOfRows;

    /// <summary>
    ///     Gets the number of columns in the game board.
    /// </summary>
    public static int Columns => NumberOfColumns;

    /// <summary>
    ///     Gets the winner's piece (an 'X' or an 'O').
    /// </summary>
    public Pieces WinningPiece { get; private set; }

    /// <summary>
    /// Gets the open positions.
    /// </summary>
    public int[] OpenPositions
    {
        get
        {
            var positions = new List<int>();

            for (var i = 0; i < this.board.Length; i++)
            {
                if (!this.IsOccupied(i))
                {
                    positions.Add(i);
                }
            }

            return positions.ToArray();
        }
    }

    /// <inheritdoc cref="ICloneable"/>
    /// <summary>
    ///     Clones the object.
    /// </summary>
    /// <seealso cref="ICloneable"/>
    public object Clone()
    {
        var b = new Board(this.board, this.language);
        return b;
    }

    /// <summary>
    ///     Checks whether the position is on the board and currently not occupied by
    ///     an 'X' or an 'O'. Position 0 is the upper-left most square and increases row-by-row
    ///     so that first square in the second row is position 3 and position and position 8
    ///     is the 3rd square in the 3rd row.
    ///     0 1 2
    ///     3 4 5
    ///     6 7 8
    /// </summary>
    /// <param name="position">The position to test.</param>
    /// <returns>Returns <c>true</c> if the position is on the board and currently not occupied by an 'X' or an 'O', <c>false</c> else.</returns>
    public bool IsValidSquare(int position)
    {
        var p = GetPoint(position);
        return p.X >= 0 && p.X < NumberOfRows && p.Y >= 0 && p.Y < NumberOfColumns && this.IsPositionOpen(p.X, p.Y);
    }

    /// <summary>
    /// Undoes a move.
    /// </summary>
    /// <param name="move">The move.</param>
    public void UndoMove(TicTacToeMove move)
    {
        if (!this.IsOnBoard(move.Position))
        {
            throw new InvalidMoveException(this.language.GetWord("CantUndoAMoveOnAnInvalidSquare") ?? string.Empty);
        }

        // Just reset the position
        var p = GetPoint(move.Position);
        this.board[p.X, p.Y] = 0;
    }

    /// <summary>
    ///     Make a move on the board.
    /// </summary>
    /// <param name="position">The board position to take.</param>
    /// <param name="piece">The piece.</param>
    public void MakeMove(int position, Pieces piece)
    {
        if (!this.IsValidSquare(position))
        {
            throw new InvalidMoveException();
        }

        var pieceNumber = GetPieceNumber(piece);
        var point = GetPoint(position);
        this.board[point.X, point.Y] = pieceNumber;
    }

    /// <summary>
    ///     Returns the piece at the corresponding row and column on the board.
    /// </summary>
    /// <param name="row">The row on the board (0-2).</param>
    /// <param name="column">The column (0-2).</param>
    /// <returns>The <see cref="Pieces"/>.</returns>
    public Pieces GetPieceAtPoint(int row, int column)
    {
        return this.GetPieceAtPosition(GetPositionFromPoint(new Point(row, column)));
    }

    /// <summary>
    ///     Returns the piece at the given board position.
    /// </summary>
    /// <param name="position">
    ///     A number representing the board position.
    ///     0 1 2
    ///     3
    ///     4 5
    ///     6 7 9
    /// </param>
    /// <returns>The <see cref="Pieces"/>.</returns>
    public Pieces GetPieceAtPosition(int position)
    {
        if (!this.IsOccupied(position))
        {
            return Pieces.Empty;
        }

        return this.GetBoardPiece(position) == 1 ? Pieces.X : Pieces.O;
    }

    /// <summary>
    ///     Checks the board to see if there is a winner
    /// </summary>
    /// <returns>True if there is a winner or false otherwise</returns>
    public bool HasWinner()
    {
        for (var i = 0; i < this.board.Length; i++)
        {
            if (!this.IsWinnerAt(i))
            {
                continue;
            }

            this.SetWinnerAtPosition(i);
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Check whether there is a winner or there is a draw or not.
    /// </summary>
    /// <returns>A value indicating whether there is a winner or there is a draw or not.</returns>
    public bool IsGameOver()
    {
        return this.HaveWinner() || this.IsDraw();
    }

    /// <summary>
    ///     Checks the board configuration to see whether there is currently a draw or not.
    ///     A draw occurs when all the positions are full and there isn't a winner.
    /// </summary>
    /// <returns>A value indicating whether there is currently a draw or not.</returns>
    public bool IsDraw()
    {
        if (this.HasWinner())
        {
            return false;
        }

        for (var i = 0; i < this.board.Length; i++)
        {
            if (!this.IsOccupied(i))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Gets the internal representation of the piece.
    /// </summary>
    /// <param name="piece">The piece.</param>
    /// <returns>The number of the <see cref="Pieces"/>.</returns>
    private static int GetPieceNumber(Pieces piece)
    {
        return piece == Pieces.O ? 2 : 1;
    }

    /// <summary>
    /// Gets the position number given the row and column, X is the row, y the column.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>The position number as <see cref="int"/>.</returns>
    private static int GetPositionFromPoint(Point point)
    {
        return (point.X * Columns) + point.Y;
    }

    /// <summary>
    /// Maps a board position number to a point containing 
    /// the row in the x value and the column in the y value.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>The <see cref="Point"/>.</returns>
    private static Point GetPoint(int position)
    {
        var p = new Point
        {
            X = position / NumberOfColumns,
            Y = position % NumberOfRows
        };

        return p;
    }

    /// <summary>
    /// Checks whether the row and column are on the board or not, x is the row and y the column.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>a value indicating whether the row and column are on the board or not.</returns>
    private static bool IsPointInBounds(Point point)
    {
        return point.X >= 0 && point.X < Rows && point.Y >= 0 && point.Y < Columns;
    }

    /// <summary>
    /// Sets the winner at the position.
    /// </summary>
    /// <param name="position">The position.</param>
    private void SetWinnerAtPosition(int position)
    {
        // Get the piece at i
        this.WinningPiece = this.GetPieceAtPosition(position);
    }

    /// <summary>
    /// Gets the board piece.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>The board piece as <see cref="int"/>.</returns>
    private int GetBoardPiece(int position)
    {
        var p = GetPoint(position);
        return this.board[p.X, p.Y];
    }

    /// <summary>
    /// Checks whether the position is available.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column.</param>
    /// <returns>A value indicating whether the position is available or not.</returns>
    private bool IsPositionOpen(int row, int column)
    {
        return this.board[row, column] == 0;
    }

    /// <summary>
    /// Checks whether the position is occupied.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>A value indicating whether the position is occupied or not.</returns>
    private bool IsOccupied(int position)
    {
        var p = GetPoint(position);
        return this.IsOccupied(p.X, p.Y);
    }

    /// <summary>
    /// Checks whether the position is occupied.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column.</param>
    /// <returns>A value indicating whether the position is occupied or not.</returns>
    private bool IsOccupied(int row, int column)
    {
        return !this.IsPositionOpen(row, column);
    }

    /// <summary>
    /// Checks whether the winner is at the position or not.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>A value indicating whether the winner is at the position or not.</returns>
    private bool IsWinnerAt(int position)
    {
        // Check each position for winner to the right
        return this.IsWinnerToTheRight(position) || this.IsWinnerFromTopToBottom(position)
                                                 || this.IsWinnerDiagonallyToRightUp(position)
                                                 || this.IsWinnerDiagonallyToRightDown(position);
    }

    /// <summary>
    /// Checks whether the winner is in the diagonal starting from the bottom-left corner of the board to the upper-right corner or not.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>A value indicating whether the winner is in the diagonal starting from the bottom-left corner of the board to the upper-right corner or not.</returns>
    private bool IsWinnerDiagonallyToRightUp(int position)
    {
        if (!this.IsOccupied(position))
        {
            return false;
        }

        var piece = this.GetPieceAtPosition(position);
        var last = GetPoint(position);

        for (var i = 1; i < WinningLength; i++)
        {
            last.X -= 1;
            last.Y += 1;

            if (!IsPointInBounds(last))
            {
                return false;
            }

            if (piece != this.GetPieceAtPosition(GetPositionFromPoint(last)))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     Checks whether there is a winner or not.
    /// </summary>
    /// <returns>A value indicating whether there is a winner or not.</returns>
    private bool HaveWinner()
    {
        return this.HasWinner();
    }

    /// <summary>
    /// Checks whether there is a winner diagonally from the specified position to the right or not.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>A value indicating whether there is a winner diagonally from the specified position to the right or not.</returns>
    private bool IsWinnerDiagonallyToRightDown(int position)
    {
        GetPoint(position);

        if (!this.IsOccupied(position))
        {
            return false;
        }

        var piece = this.GetPieceAtPosition(position);

        // Keep moving diagonally until we reach the winningLength
        // or we don't see the same piece
        var last = GetPoint(position);

        for (var i = 1; i < WinningLength; i++)
        {
            last.X += 1;
            last.Y += 1;

            if (!IsPointInBounds(last))
            {
                return false;
            }

            if (piece != this.GetPieceAtPosition(GetPositionFromPoint(last)))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks whether there is a winner from top to bottom starting at the specified position or not.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>A value indicating whether there is a winner from top to bottom starting at the specified position or not.</returns>
    private bool IsWinnerFromTopToBottom(int position)
    {
        var point = GetPoint(position);

        // Check if we even have a square here
        if (!this.IsOccupied(position))
        {
            return false;
        }

        // Do we have the room to go down from here?
        if (point.X + WinningLength - 1 >= NumberOfRows)
        {
            return false;
        }

        var piece = this.GetPieceAtPosition(position);

        // If we get here then we know we at least have
        // the potential for a winner from top to bottom
        for (var i = 1; i < WinningLength; i++)
        {
            if (piece != this.GetPieceAtPosition(position + (3 * i)))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks whether there is a winner from the specified position to the right or not.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>A value indicating whether there is a winner from the specified position to the right or not.</returns>
    private bool IsWinnerToTheRight(int position)
    {
        var p = GetPoint(position);

        // Check if we even the square is occupied
        // if it's not then we don't have a winner 
        // starting here
        if (!this.IsOccupied(position))
        {
            return false;
        }

        // check if we have room to the right?
        if (p.Y + WinningLength - 1 >= NumberOfColumns)
        {
            return false;
        }

        var piece = this.GetPieceAtPosition(position);

        for (var i = 1; i < WinningLength; i++)
        {
            if (this.GetPieceAtPosition(position + i) != piece)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks whether the position is on the board.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>>Returns <c>true</c> if the position is on the board, <c>false</c> else.</returns>
    private bool IsOnBoard(int position)
    {
        return this.IsOccupied(position) || this.IsValidSquare(position);
    }
}
