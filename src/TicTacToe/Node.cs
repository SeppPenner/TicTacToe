// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Node.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   This class represents a node in the search tree.
//   Each node contains a particular board configuration. Nodes 0 or more children
//   that represent subsequent moves from that node's board.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe;

/// <summary>
///     This class represents a node in the search tree.
///     Each node contains a particular board configuration. Nodes 0 or more children
///     that represent subsequent moves from that node's board.
/// </summary>
public abstract class Node
{
    /// <summary>
    /// The board that this node represents.
    /// </summary>
    protected readonly Board? Board;

    /// <summary>
    /// This node's children.
    /// </summary>
    protected readonly List<Node> Children = new();

    /// <summary>
    /// The move that was made from the parent node that created this node.
    /// </summary>
    private readonly TicTacToeMove? move;

    /// <summary>
    /// The child node that represents the best move  for a node.
    /// </summary>
    private Node? bestMoveNode;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Node"/> class.
    /// </summary>
    /// <param name="board">The board that the Node will use to evaluate itself and generate its children</param>
    /// <param name="parent">The parent of this node</param>
    /// <param name="move">The move from the parent's board that generated this node's board</param>
    protected Node(Board board, Node? parent, TicTacToeMove? move)
    {
        this.Board = board;
        this.move = move;

        if (parent != null)
        {
            this.MyPieceLocal = OpponentPieceHelper.GetOpponentPiece(parent.MyPiece);
        }

        this.Children = new List<Node>();
    }

    /// <summary>
    ///     Gets or sets the game piece that this node has.
    /// </summary>
    public Pieces MyPiece
    {
        private get => this.MyPieceLocal;
        set
        {
            this.MyPieceLocal = value;
            OpponentPieceHelper.GetOpponentPiece(value);
        }
    }

    /// <summary>
    ///     Sets the evaluation function used by this node to calculate
    ///     its value
    /// </summary>
    public EvaluationFunction Evaluator
    {
        set => this.EvaluatorLocal = value;
    }

    /// <summary>
    /// Gets or sets the value of this node either computed by the evaluation function
    /// or the value selected from among child nodes.
    /// </summary>
    public double Value { get; protected set; }

    /// <summary>
    /// Gets the best move from this node's board configuration.
    /// </summary>
    public TicTacToeMove? BestMove => this.bestMoveNode?.move;

    /// <summary>
    /// Gets the piece representing the node's piece.
    /// </summary>
    protected Pieces MyPieceLocal { get; private set; }

    /// <summary>
    /// Gets the evaluation function.
    /// </summary>
    protected EvaluationFunction? EvaluatorLocal { get; private set; }

    /// <summary>
    ///     Finds the best move for the node by doing a pseudo-depth-f
    /// </summary>
    /// <param name="depth">The depth to search</param>
    public void FindBestMove(int depth)
    {
        if (depth <= 0)
        {
            return;
        }

        // Expand this node. Sub classes provide their own implementation of this.
        this.GenerateChildren();

        // Evaluate each child.
        // If there is a winner, there is no need to go further down the tree.
        // Sends the Evaluate() message to each child node, which is implemented
        // by sub classes.
        this.EvaluateChildren();

        // Check for a winner
        var haveWinningChild = this.Children.Exists(c => c.IsGameEndingNode());

        if (haveWinningChild)
        {
            // The best move depends on the subclass
            this.SelectBestMove();
        }
        else
        {
            foreach (var child in this.Children)
            {
                child.FindBestMove(depth - 1);
            }

            this.SelectBestMove();
        }
    }

    /// <summary>
    /// Evaluates the node.
    /// </summary>
    protected abstract void Evaluate();

    /// <summary>
    /// Generates the node's children.
    /// </summary>
    protected abstract void GenerateChildren();

    /// <summary>
    /// Sorts the children nodes.
    /// </summary>
    /// <param name="unsortedChildren">The unsorted children nodes.</param>
    /// <returns>A new <see cref="List{T}"/> of <see cref="Node"/>.</returns>
    protected abstract List<Node> SortChildren(IEnumerable<Node> unsortedChildren);

    /// <summary>
    ///     Selects the best move node for the node.
    /// </summary>
    private void SelectBestMove()
    {
        // If no children there is no best move for the node
        if (this.Children.Count == 0)
        {
            this.bestMoveNode = null;
            return;
        }

        // Sort the children so that the first element contains the 'best' node
        var sortedChildren = this.SortChildren(this.Children);
        this.bestMoveNode = sortedChildren[0];
        this.Value = this.bestMoveNode.Value;
    }

    /// <summary>
    ///     Checks to see if the node is either a winner or a maximum or a minimum.
    /// </summary>
    /// <returns><c>true</c> if either 'X' or 'O'.</returns>
    private bool IsGameEndingNode()
    {
        return Math.Abs(this.Value - double.MaxValue) < 0.00000000000000000000001
               || Math.Abs(this.Value - double.MinValue) < 0.00000000000000000000001;
    }

    /// <summary>
    /// Evaluate the child nodes.
    /// </summary>
    private void EvaluateChildren()
    {
        foreach (var child in this.Children)
        {
            child.Evaluate();
        }
    }
}
