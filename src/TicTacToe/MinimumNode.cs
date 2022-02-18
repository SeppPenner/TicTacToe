// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MinimumNode.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   This class represents a minimum node in the game tree.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe;

/// <inheritdoc cref="Node"/>
/// <summary>
///     This class represents a minimum node in the game tree.
/// </summary>
/// <seealso cref="Node"/>
public sealed class MinimumNode : Node
{
    /// <inheritdoc cref="Node"/>
    /// <summary>
    ///     Initializes a new instance of the <see cref="MinimumNode"/> class.
    /// </summary>
    /// <param name="board">The board that this node represents.</param>
    /// <param name="parent">This node's parent.</param>
    /// <param name="move">The move that was made from the parent to lead to this node's board.</param>
    /// <seealso cref="Node"/>
    public MinimumNode(Board board, Node parent, TicTacToeMove move) : base(board, parent, move)
    {
    }

    /// <inheritdoc cref="Node"/>
    /// <summary>
    /// Generates the node's children. Minimum nodes have maximum children.
    /// </summary>
    /// <seealso cref="Node"/>
    protected override void GenerateChildren()
    {
        if (this.Board is null)
        {
            throw new ArgumentNullException(nameof(this.Board), "The board wasn't initialized properly.");
        }

        var openPositions = this.Board.OpenPositions;

        foreach (var i in openPositions)
        {
            var b = (Board)this.Board.Clone();
            var m = new TicTacToeMove(i, this.MyPieceLocal);
            b.MakeMove(i, this.MyPieceLocal);
            this.Children.Add(new MaximumNode(b, this, m));
        }
    }

    /// <inheritdoc cref="Node"/>
    /// <summary>
    /// Sorts the children nodes in ascending order.
    /// The first node in the list will be the best node for the minimum node.
    /// </summary>
    /// <param name="unsortedChildren">The unsorted children nodes.</param>
    /// <returns>A new <see cref="List{T}"/> of <see cref="Node"/>.</returns>
    /// <seealso cref="Node"/>
    protected override List<Node> SortChildren(IEnumerable<Node> unsortedChildren)
    {
        var sortedChildren = unsortedChildren.OrderBy(n => n.Value).ToList();
        return sortedChildren;
    }

    /// <inheritdoc cref="Node"/>
    /// <summary>
    /// Evaluates the value of the node using the evaluation function.
    /// </summary>
    /// <seealso cref="Node"/>
    protected override void Evaluate()
    {
        if (this.Board is null)
        {
            throw new ArgumentNullException(nameof(this.Board), "The board wasn't initialized properly.");
        }

        this.Value = this.EvaluatorLocal?.Evaluate(this.Board, OpponentPieceHelper.GetOpponentPiece(this.MyPieceLocal)) ?? -1;
    }
}
