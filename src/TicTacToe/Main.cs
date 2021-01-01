// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Main.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   This class is responsible for 'driving' the game.
//   It handles drawing the game board, handling board click events,
//   and polling the players for their moves.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    using Languages.Implementation;
    using Languages.Interfaces;

    /// <summary>
    ///     This class is responsible for 'driving' the game.
    ///     It handles drawing the game board, handling board click events,
    ///     and polling the players for their moves.
    /// </summary>
    public partial class TicTacToeForm : Form
    {
        /// <summary>
        /// The language manager.
        /// </summary>
        private readonly ILanguageManager languageManager = new LanguageManager();

        /// <summary>
        /// The main thread.
        /// </summary>
        private Thread mainThread;

        /// <summary>
        /// The player thread.
        /// </summary>
        private Thread playerThread;

        /// <summary>
        /// The game.
        /// </summary>
        private TicTacToeGame game;

        /// <summary>
        /// The last move.
        /// </summary>
        private TicTacToeMove lastMove;

        /// <summary>
        /// The players.
        /// </summary>
        private List<Player> players;

        /// <summary>
        /// The language.
        /// </summary>
        private ILanguage language;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicTacToeForm"/> class.
        /// </summary>
        public TicTacToeForm()
        {
            this.InitializeComponent();
            this.LoadTitleAndDescription();
            this.ticTacToePanel.Paint += this.TicTacToePanelPaint;
            this.FormClosed += this.MainFormClosed;
            this.InitializeLanguageManager();
            this.LoadLanguagesToCombo();
        }

        /// <summary>
        ///     Used to notify a client that user has double clicked on a game square.
        /// </summary>
        public event SquareDoubleClickHandler SquareDoubleClicked;

        /// <summary>
        ///  Add the player as a participant to the game.
        /// </summary>
        /// <param name="player">The player to add.</param>
        public void AddPlayer(Player player)
        {
            // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
            if (this.players == null)
            {
                this.players = new List<Player>();
            }

            if (this.players.Count > 2)
            {
                throw new Exception(this.language.GetWord("MustHaveOnly2Players"));
            }

            if (this.players.Count == 1)
            {
                if (this.players[0].PlayerPiece == player.PlayerPiece)
                {
                    throw new Exception(this.language.GetWord("MustHaveDifferentBoardPieces"));
                }
            }

            this.players.Add(player);
        }

        /// <summary>
        /// Loads the title and description.
        /// </summary>
        private void LoadTitleAndDescription()
        {
            this.Text = Application.ProductName + @" " + Application.ProductVersion;
        }

        /// <summary>
        /// Handles the main form closed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void MainFormClosed(object sender, FormClosedEventArgs e)
        {
            // Clean up the threads
            this.mainThread = null;
            this.playerThread = null;
            Environment.Exit(0);
        }

        /// <summary>
        /// Handles the form load event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void TicTacToeFormLoad(object sender, EventArgs e)
        {
            this.LaunchGame();
        }

        /// <summary>
        ///  Gets the game started. Players must have already been added.
        /// </summary>
        private void LaunchGame()
        {
            if (this.players.Count != 2)
            {
                throw new Exception(this.language.GetWord("MustHaveExactly2Players"));
            }

            this.game = new TicTacToeGame(this.players[0].PlayerPiece, this.players[1].PlayerPiece, this.language);
            this.Show();
            this.ticTacToePanel.Invalidate();
            this.ticTacToePanel.MouseDoubleClick += this.TicTacToePanelMouseDoubleClick;

            this.mainThread = new Thread(this.ProcessPlayerMoves);
            this.mainThread.Start();
        }

        /// <summary>
        /// Gets a move for a player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>A <see cref="TicTacToeMove"/>.</returns>
        private TicTacToeMove GetMoveForPlayer(Player player)
        {
            this.lastMove = null;

            this.playerThread = new Thread(player.Move);
            this.playerThread.Start(this.game.GameBoard);

            // Register a listener
            player.PlayerMoved += this.PlayerMoved;

            // LastMove is assigned in the player PlayerMoved handler
            while (this.lastMove == null)
            {
            }

            // If we get here the player moved
            // Un-register the listener
            player.PlayerMoved -= this.PlayerMoved;

            // Kill the thread
            this.playerThread = null;
            return player.CurrentMove;
        }

        /// <summary>
        /// Applies the player's move to the game board.
        /// </summary>
        private void ProcessPlayerMoves()
        {
            while (!this.game.IsGameOver())
            {
                foreach (var iteratorPlayer in this.players)
                {
                    var player = iteratorPlayer;
                    var playerMove = this.GetMoveForPlayer(player);
                    this.game.MakeMove(new TicTacToeMove(playerMove.Position, player.PlayerPiece));

                    // Update the graphics
                    this.ticTacToePanel.Invalidate();

                    if (!this.IsGameOver())
                    {
                        continue;
                    }

                    this.ShowEndOfGameMessage(iteratorPlayer);
                    this.FinishGame();

                    return;
                }
            }
        }

        /// <summary>
        /// Checks whether the game is over or not.
        /// </summary>
        /// <returns><c>true</c> if the game is over, <c>false</c> else.</returns>
        private bool IsGameOver()
        {
            return this.game.GameBoard.HasWinner() || this.game.GameBoard.IsDraw();
        }

        /// <summary>
        /// Shows the end of the game message.
        /// </summary>
        /// <param name="lastPlayerToAct">The last player to act.</param>
        private void ShowEndOfGameMessage(Player lastPlayerToAct)
        {
            var message = this.language.GetWord("GameOverMessage");

            if (this.game.GameBoard.HasWinner())
            {
                message += lastPlayerToAct.Name + this.language.GetWord("PlayerXWins");
            }
            else
            {
                message += this.language.GetWord("Draw");
            }

            MessageBox.Show(message);
        }

        /// <summary>
        /// Finishes the game.
        /// </summary>
        private void FinishGame()
        {
            // Kill the main game driver thread
            this.mainThread = null;

            // Now un-register the mouse click listener
            this.ticTacToePanel.MouseDoubleClick -= this.TicTacToePanelMouseDoubleClick;
        }

        /// <summary>
        /// Handles the player moved event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void PlayerMoved(object sender, PlayerMovedArgs e)
        {
            this.lastMove = e.Move;
        }

        /// <summary>
        /// Handles the new game tool strip item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void NewGameToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.mainThread = null;
            this.LaunchGame();
        }

        /// <summary>
        /// Handles the panel double click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void TicTacToePanelMouseDoubleClick(object sender, MouseEventArgs e)
        {
            var position = this.TranslateMouseClickToPosition(e.X, e.Y);

            // We will only process the event and notify any clients of the SquareDoubleClicked
            // event if the the square is empty
            if (!this.game.GameBoard.IsValidSquare(position))
            {
                return;
            }

            // Determine the position of th square clicked on the panel and then invoke the event
            var t = new TicTacToeBoardClickedEventArgs(position);
            this.SquareDoubleClicked?.Invoke(this, t);
        }

        /// <summary>
        /// Determines the board location given a mouse click on the game board.
        /// </summary>
        /// <param name="x">The X value.</param>
        /// <param name="y">The Y value.</param>
        /// <returns>The position on the board as <see cref="int"/>.</returns>
        private int TranslateMouseClickToPosition(int x, int y)
        {
            var pixelsPerColumn = this.ticTacToePanel.Width / this.game.Columns;
            var pixelsPerRow = this.ticTacToePanel.Height / this.game.Rows;
            var row = y / pixelsPerRow;
            var col = x / pixelsPerColumn;
            var position = (row * this.game.Columns) + col;
            return position;
        }

        /// <summary>
        /// Redraws the grid and pieces on the board.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void TicTacToePanelPaint(object sender, PaintEventArgs e)
        {
            this.DrawGrid(e.Graphics);
            this.DrawPieces(e.Graphics);
        }

        /// <summary>
        /// Draws each piece on the board onto the panel.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        private void DrawPieces(Graphics graphics)
        {
            for (var i = 0; i < this.game.Rows * this.game.Columns; i++)
            {
                var piece = this.game.GameBoard.GetPieceAtPosition(i);

                if (piece == Pieces.X || piece == Pieces.O)
                {
                    this.DrawPiece(piece, graphics, i);
                }
            }
        }

        /// <summary>
        /// Draws the grid.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        private void DrawGrid(Graphics graphics)
        {
            // Equally space the grid rows
            // ReSharper disable PossibleLossOfFraction
            float pixelsPerRow = this.ticTacToePanel.Width / this.game.Rows;
            float pixelsPerColumn = this.ticTacToePanel.Height / this.game.Columns;

            // Now draw the grid lines
            var p = new Pen(Brushes.Black) { Width = 6 };

            for (var i = 0; i < this.game.Rows - 1; i++)
            {
                graphics.DrawLine(p, pixelsPerColumn * (i + 1), 0, pixelsPerColumn * (i + 1), this.ticTacToePanel.Height);
            }

            // Now draw the horizontal lines
            for (var j = 0; j < this.game.Columns - 1; j++)
            {
                graphics.DrawLine(p, 0, pixelsPerRow * (j + 1), this.ticTacToePanel.Width, pixelsPerRow * (j + 1));
            }
        }

        /// <summary>
        ///     This method returns a PointF struct representing the coordinates
        ///     of where a piece should be drawn on the board given the position (0-8) on the board.
        ///     The coordinates are essentially the center of a position on the board with offsets that allow the
        ///     piece to be drawn in a position so that it is centered.
        /// </summary>
        /// <param name="position">The position on the board for which we want the drawing coordinates.</param>
        /// <returns>A <see cref="PointF"/> representing the x and y coordinates on the panel the piece should be drawn.</returns>
        private PointF GetPieceDrawingCoordsFromPosition(int position)
        {
            // xOffset and yOffset are used for small corrections in placing the X and O's on the screen in order to get them centered
            var row = position / this.game.Columns;
            var column = position % this.game.Columns;
            var pixelsPerRow = this.ticTacToePanel.Height / this.game.Rows;
            var pixelsPerColumn = this.ticTacToePanel.Width / this.game.Columns;
            float midPixelsPerRow = pixelsPerRow / 3;
            float midPixelsPerColumn = pixelsPerColumn / 3;
            var xCoordinate = (pixelsPerColumn * column) + midPixelsPerColumn + -5;
            var yCoordinate = (pixelsPerRow * row) + midPixelsPerRow + -5;
            return new PointF(xCoordinate, yCoordinate);
        }

        /// <summary>
        ///     Draws the specified piece on the board in the designated position.
        ///     Position 0 is the upper left square on the board and position 8 is the lower right
        ///     corner square on the board.
        /// </summary>
        /// <param name="piece">The piece we wish to draw.</param>
        /// <param name="graphics">The graphics object we are drawing on.</param>
        /// <param name="position">The position on the board to draw at.</param>
        private void DrawPiece(Pieces piece, Graphics graphics, int position)
        {
            var point = this.GetPieceDrawingCoordsFromPosition(position);
            var f = new Font("Arial", 40);

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (piece)
            {
                case Pieces.X:
                    graphics.DrawString("X", f, Brushes.Blue, point);
                    break;
                case Pieces.O:
                    graphics.DrawString("O", f, Brushes.Red, point);
                    break;
            }
        }

        /// <summary>
        /// Handles the form closing event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void TicTacToeFormFormClosing(object sender, FormClosingEventArgs e)
        {
            // Clean up the threads
            this.mainThread = null;
            this.playerThread = null;
        }

        /// <summary>
        /// Handles the combo box selected index changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void ComboBoxLanguageSelectedIndexChanged(object sender, EventArgs e)
        {
            this.languageManager.SetCurrentLanguageFromName(this.comboBoxLanguage.SelectedItem.ToString());
        }

        /// <summary>
        /// Initializes the language manager.
        /// </summary>
        private void InitializeLanguageManager()
        {
            this.languageManager.SetCurrentLanguage("de-DE");
            this.languageManager.OnLanguageChanged += this.OnLanguageChanged;
        }

        /// <summary>
        /// Loads the languages to the combo box.
        /// </summary>
        private void LoadLanguagesToCombo()
        {
            foreach (var lang in this.languageManager.GetLanguages())
            {
                this.comboBoxLanguage.Items.Add(lang.Name);
            }

            this.comboBoxLanguage.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the language changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            this.language = this.languageManager.GetCurrentLanguage();
            this.labelLanguage.Text = this.language.GetWord("SelectLanguage");
            this.gameToolStripMenuItem.Text = this.language.GetWord("Game");
            this.newGameToolStripMenuItem.Text = this.language.GetWord("NewGame");
        }
    }
}