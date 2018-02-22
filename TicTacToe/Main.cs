using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Languages.Implementation;
using Languages.Interfaces;

namespace TicTacToe
{
    /// <inheritdoc />
    /// <summary>
    ///     This class is responsible for 'driving' the game.
    ///     It handles drawing the game board, handling board click events,
    ///     and polling the players for their moves.
    /// </summary>
    public partial class TicTacToeForm : Form
    {
        private TicTacToeGame _game;
        private TicTacToeMove _lastMove;
        private Thread _mainThread;
        private List<Player> _players;
        private Thread _playerThread;
        private readonly ILanguageManager _lm = new LanguageManager();
        private ILanguage _lang;

        public TicTacToeForm()
        {
            InitializeComponent();
            LoadTitleAndDescription();
            ticTacToePanel.Paint += TicTacToePanel_Paint;
            ticTacToePanel.MouseDoubleClick += TicTacToePanel_MouseDoubleClick;
            FormClosed += Main_FormClosed;
            InitializeLanguageManager();
            LoadLanguagesToCombo();
        }

        private void LoadTitleAndDescription()
        {
            Text = Application.ProductName + @" " + Application.ProductVersion;
        }

        /// <summary>
        ///     Used to notify a client that user has double clicked on a game square
        /// </summary>
        public event SquareDoubleClickHandler SquareDoubleClicked;

        /// <summary>
        ///     Add the player as a participant to the game
        /// </summary>
        /// <param name="p">The player to add</param>
        public void AddPlayer(Player p)
        {
            if (_players == null)
                _players = new List<Player>();
            if (_players.Count > 2)
                throw new Exception(_lang.GetWord("MustHaveOnly2Players"));
            if (_players.Count == 1)
                if (_players[0].PlayerPiece == p.PlayerPiece)
                    throw new Exception(_lang.GetWord("MustHaveDifferentBoardPieces"));
            _players.Add(p);
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Clean up threads
            _mainThread?.Abort();
            _playerThread?.Abort();
        }

        private void TicTacToeForm_Load(object sender, EventArgs e)
        {
            LaunchGame();
        }

        /// <summary>
        ///     Gets the game started.  Players must have already been added.
        /// </summary>
        private void LaunchGame()
        {
            if (_players.Count != 2)
                throw new Exception(_lang.GetWord("MustHave2Players"));
            _game = new TicTacToeGame(_players[0].PlayerPiece, _players[1].PlayerPiece, _lang);
            Show();
            ticTacToePanel.Invalidate();
            _mainThread = new Thread(ProcessPlayerMoves);
            _mainThread.Start();
        }

        private TicTacToeMove GetMoveForPlayer(Player p)
        {
            _lastMove = null;
            _playerThread = new Thread(p.Move);
            _playerThread.Start(_game.GameBoard);
            // Register a listener
            p.PlayerMoved += Player_PlayerMoved;
            // LastMove is assigned in the player_PlayerMoved handler
            while (_lastMove == null)
            {
            }
            // If we get here the player moved
            // Unregister the listenter
            p.PlayerMoved -= Player_PlayerMoved;
            // Kill the thread
            _playerThread.Abort();
            return p.CurrentMove;
        }

        // Gets each players move applies it to the game board
        private void ProcessPlayerMoves()
        {
            while (!_game.IsGameOver())
                foreach (var iteratorPlayer in _players)
                {
                    var p = iteratorPlayer;
                    var playerMove = GetMoveForPlayer(p);
                    _game.MakeMove(new TicTacToeMove(playerMove.Position, p.PlayerPiece));
                    // Update the graphics
                    ticTacToePanel.Invalidate();
                    if (!IsGameOver()) continue;
                    ShowEndOfGameMessage(iteratorPlayer);
                    FinishGame();
                }
        }

        private bool IsGameOver()
        {
            return _game.GameBoard.HasWinner() || _game.GameBoard.IsDraw();
        }

        private void ShowEndOfGameMessage(Player lastPlayerToAct)
        {
            var msg = _lang.GetWord("GameOverMessage");
            if (_game.GameBoard.HasWinner())
                msg += lastPlayerToAct.Name + _lang.GetWord("PlayerXWins");
            else
                msg += _lang.GetWord("Draw");
            MessageBox.Show(msg);
        }

        private void FinishGame()
        {
            // Kill the main game driver thread
            _mainThread.Abort();
            // Now unregister the mouseclick listener
            ticTacToePanel.MouseDoubleClick -= TicTacToePanel_MouseDoubleClick;
        }

        private void Player_PlayerMoved(object sender, PlayerMovedArgs args)
        {
            _lastMove = args.Move;
        }

        private void NewGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _mainThread?.Abort();
            LaunchGame();
        }

        private void TicTacToePanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var position = TranslateMouseClickToPosition(e.X, e.Y);
            // We will only process the event and notify any clients of the SquareDoubleClicked
            // event if the the square is empty
            if (!_game.GameBoard.IsValidSquare(position)) return;
            // Determine the position of th square clicked on the panel and then inoke the event
            var t = new TicTacToeBoardClickedEventArgs(position);
            SquareDoubleClicked?.Invoke(this, t);
        }

        // Determines the board location given a mouse click on the 
        // game board
        private int TranslateMouseClickToPosition(int x, int y)
        {
            var pixelsPerColumn = ticTacToePanel.Width / _game.Columns;
            var pixelsPerRow = ticTacToePanel.Height / _game.Rows;
            var row = y / pixelsPerRow;
            var col = x / pixelsPerColumn;
            var position = row * _game.Columns + col;
            return position;
        }

        // Redraw the grid and pieces on the board
        private void TicTacToePanel_Paint(object sender, PaintEventArgs e)
        {
            DrawGrid(e.Graphics);
            DrawPieces(e.Graphics);
        }

        // Draws each piece on the board onto the panel
        private void DrawPieces(Graphics g)
        {
            for (var i = 0; i < _game.Rows * _game.Columns; i++)
            {
                var piece = _game.GameBoard.GetPieceAtPosition(i);
                if (piece == Pieces.X || piece == Pieces.O)
                    DrawPiece(piece, g, i);
            }
        }

        // Used as a flag to indicate that a p
        // Thread used to request a player to move
        // runs in a seperate thread so that it does not block main thread
        // and effect UI
        private void DrawGrid(Graphics g)
        {
            // Equally space the grid rows
            // ReSharper disable PossibleLossOfFraction
            float pixelsPerRow = ticTacToePanel.Width / _game.Rows;
            float pixelsPerColumn = ticTacToePanel.Height / _game.Columns;
            // Now draw the grid lines
            var p = new Pen(Brushes.Black) {Width = 6};
            for (var i = 0; i < _game.Rows - 1; i++)
                g.DrawLine(p, pixelsPerColumn * (i + 1), 0, pixelsPerColumn * (i + 1), ticTacToePanel.Height);
            // Now draw the horizontal lines
            for (var j = 0; j < _game.Columns - 1; j++)
                g.DrawLine(p, 0, pixelsPerRow * (j + 1), ticTacToePanel.Width, pixelsPerRow * (j + 1));
        }

        /// <summary>
        ///     This method returns a PointF struct representing the cooridinates
        ///     of where a piece should be drawn on the board given the position (0-8) on the board.
        ///     The coords are essentially the center of a position on the board with offsets that allow the
        ///     piece to be drawn in a position so that it is centered
        /// </summary>
        /// <param name="position">The position on the board for which we want the drawing coords</param>
        /// <returns>a PointF representing the x and y coords on the panel the piece should be drawn</returns>
        private PointF GetPieceDrawingCoordsFromPosition(int position)
        {
            // xOffset and yOffset are used for small corrections
            // in placing the X and O's on the screen
            // in order to get them centered
            var row = position / _game.Columns;
            var col = position % _game.Columns;
            var pixelsPerRow = ticTacToePanel.Height / _game.Rows;
            var pixelsPerColumn = ticTacToePanel.Width / _game.Columns;
            float midPixelsPerRow = pixelsPerRow / 3;
            float midPixelsPerColumn = pixelsPerColumn / 3;
            var xCoord = pixelsPerColumn * col + midPixelsPerColumn + -5;
            var yCoord = pixelsPerRow * row + midPixelsPerRow + -5;
            return new PointF(xCoord, yCoord);
        }

        /// <summary>
        ///     Draws the specified piece on the board in the designated position.
        ///     Position 0 is the upper left square on the board and position 8 is the lower right
        ///     corner square on the board
        /// </summary>
        /// <param name="p">The piece we wish to draw</param>
        /// <param name="g">The graphics object we are drawing on</param>
        /// <param name="position">The position on the board to draw at</param>
        private void DrawPiece(Pieces p, Graphics g, int position)
        {
            var point = GetPieceDrawingCoordsFromPosition(position);
            var f = new Font("Arial", 40);
            switch (p)
            {
                case Pieces.X:
                    g.DrawString("X", f, Brushes.Blue, point);
                    break;
                case Pieces.O:
                    g.DrawString("O", f, Brushes.Red, point);
                    break;
            }
        }

        private void TicTacToeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _mainThread?.Abort();
            _playerThread?.Abort();
        }

        private void ComboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            _lm.SetCurrentLanguageFromName(comboBoxLanguage.SelectedItem.ToString());
        }

        private void InitializeLanguageManager()
        {
            _lm.SetCurrentLanguage("de-DE");
            _lm.OnLanguageChanged += OnLanguageChanged;
        }

        private void LoadLanguagesToCombo()
        {
            foreach (var lang in _lm.GetLanguages())
                comboBoxLanguage.Items.Add(lang.Name);
            comboBoxLanguage.SelectedIndex = 0;
        }

        private void OnLanguageChanged(object sender, EventArgs eventArgs)
        {
            _lang = _lm.GetCurrentLanguage();
            labelLanguage.Text = _lang.GetWord("SelectLanguage");
            gameToolStripMenuItem.Text = _lang.GetWord("GameItem");
            newGameToolStripMenuItem.Text = _lang.GetWord("NewGame");
        }
    }
}