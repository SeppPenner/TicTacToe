using System;

namespace TicTacToe
{
    public class TicTacToeBoardClickedEventArgs : EventArgs
    {
        public TicTacToeBoardClickedEventArgs(int position)
        {
            BoardPosition = position;
        }

        public int BoardPosition { get; }
    }
}