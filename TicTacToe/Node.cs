using System;
using System.Collections.Generic;

namespace TicTacToe
{
    /// <summary>
    ///     This class represents a Node in the search Tree.
    ///     Each Node contains a particular board configuration. Nodes 0 or more children
    ///     that represent subsequent moves from that Node's board.
    /// </summary>
    public abstract class Node
    {
        // The evaluation function 
        protected static EvaluationFunction EvaluatorLocal;

        // The move that was made from the parent node that created this node
        private readonly TicTacToeMove _move;

        // The board that this node represents
        protected readonly Board Board;

        // This nodes children
        protected readonly List<Node> Children;

        // The child node that represents the best move
        // for a node.
        private Node _bestMoveNode;

        protected Pieces MyPieceLocal; // the piece representing the node's piece
        // The value associated with the node -- this is the result of the
        // evaluatio function for leaf nodes

        /// <summary>
        ///     Constructs a new Node
        /// </summary>
        /// <param name="b">The board that the Node will use to evaluate itself and generate its children</param>
        /// <param name="parent">The parent of this node</param>
        /// <param name="move">The move from the parent's board that generated this node's board</param>
        protected Node(Board b, Node parent, TicTacToeMove move)
        {
            Board = b;
            _move = move;
            if (parent != null)
                MyPieceLocal = Board.GetOponentPiece(parent.MyPiece);
            Children = new List<Node>();
        }

        /// <summary>
        ///     The game piece that this node 'has'
        /// </summary>
        public Pieces MyPiece
        {
            private get => MyPieceLocal;
            set
            {
                MyPieceLocal = value;
                Board.GetOponentPiece(value);
            }
        }

        /// <summary>
        ///     Sets the evaluation function used by this node to calculate
        ///     its value
        /// </summary>
        public EvaluationFunction Evaluator
        {
            set => EvaluatorLocal = value;
        }

        /// <summary>
        ///     The value of this node either computed by the evaluation function
        ///     or the value selected from among child nodes,
        /// </summary>
        public double Value { get; protected set; }

        /// <summary>
        ///     The best move from this node's board configuration
        /// </summary>
        public TicTacToeMove BestMove => _bestMoveNode._move;

        protected abstract void Evaluate();

        /// <summary>
        ///     Selects the best move node for the node
        /// </summary>
        private void SelectBestMove()
        {
            // If no children there is no best move for the node
            if (Children.Count == 0)
            {
                _bestMoveNode = null;
                return;
            }
            // Sort the children so that the first element contains the 'best' node
            var sortedChildren = SortChildren(Children);
            _bestMoveNode = sortedChildren[0];
            Value = _bestMoveNode.Value;
        }

        /// <summary>
        ///     Finds the best move for the node by doing a pseudo-depth-f
        /// </summary>
        /// <param name="depth">The depth to search</param>
        public void FindBestMove(int depth)
        {
            if (depth <= 0) return;
            // Expand this node -- subclasses provide their own implementation of this
            GenerateChildren();
            // Evaluate each child
            // if there is a winner there is no need to go further down
            // the tree
            // Sends the Evaluate() message to each child node, which is implemented
            // by subclasses
            EvaluateChildren();
            // Check for a winner
            var haveWinningChild = Children.Exists(c => c.IsGameEndingNode());
            if (haveWinningChild)
            {
                // The best move depends on the subclass
                SelectBestMove();
            }
            else
            {
                foreach (var child in Children)
                    child.FindBestMove(depth - 1);
                SelectBestMove();
            }
        }

        /// <summary>
        ///     Generate the nodes children
        /// </summary>
        protected abstract void GenerateChildren();

        /// <summary>
        ///     Checks to see if the node is either a winner or MAX or MIN
        /// </summary>
        /// <returns>true if either 'X' or 'O'</returns>
        private bool IsGameEndingNode()
        {
            return Math.Abs(Value - double.MaxValue) < 0.00000000000000000000001 ||
                   Math.Abs(Value - double.MinValue) < 0.00000000000000000000001;
        }

        // Evaluate the child nodes
        private void EvaluateChildren()
        {
            foreach (var child in Children)
                child.Evaluate();
        }

        protected abstract List<Node> SortChildren(IEnumerable<Node> unsortedChildren);
    }
}