// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaximumNode.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   This class represents a maximum node in the game tree.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TicTacToe
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     This class represents a maximum node in the game tree.
    /// </summary>
    public sealed class MaximumNode : Node
    {
        /// <inheritdoc cref="Node"/>
        /// <summary>
        ///     Initializes a new instance of the <see cref="MaximumNode"/> class.
        /// </summary>
        /// <param name="board">The board that this node represents.</param>
        /// <param name="parent">This node's parent.</param>
        /// <param name="move">The move that was made from the parent to lead to this node's board.</param>
        /// <seealso cref="Node"/>
        public MaximumNode(Board board, Node parent, TicTacToeMove move) : base(board, parent, move)
        {
        }

        /// <inheritdoc cref="Node"/>
        /// <summary>
        /// Generates the node's children. Maximum nodes have minimum children.
        /// </summary>
        /// <seealso cref="Node"/>
        protected override void GenerateChildren()
        {
            // Create child nodes for each of the available positions 
            var openPositions = this.Board.OpenPositions;

            foreach (var i in openPositions)
            {
                var b = (Board)this.Board.Clone();
                var m = new TicTacToeMove(i, this.MyPieceLocal);
                b.MakeMove(i, this.MyPieceLocal);
                this.Children.Add(new MinimumNode(b, this, m));
            }
        }

        /// <inheritdoc cref="Node"/>
        /// <summary>
        /// Sorts the children nodes in descending order.
        /// The first node in the list will be the best node for the maximum node.
        /// </summary>
        /// <param name="unsortedChildren">The unsorted children nodes.</param>
        /// <returns>A new <see cref="List{T}"/> of <see cref="Node"/>.</returns>
        /// <seealso cref="Node"/>
        protected override List<Node> SortChildren(IEnumerable<Node> unsortedChildren)
        {
            var sortedChildren = unsortedChildren.OrderByDescending(n => n.Value).ToList();
            return sortedChildren;
        }

        /// <inheritdoc cref="Node"/>
        /// <summary>
        /// Evaluates the value of the node using the evaluation function.
        /// </summary>
        /// <seealso cref="Node"/>
        protected override void Evaluate()
        {
            this.Value = this.EvaluatorLocal?.Evaluate(this.Board, this.MyPieceLocal) ?? -1;
        }
    }
}