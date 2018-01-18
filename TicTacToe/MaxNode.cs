using System.Collections.Generic;
using System.Linq;

namespace TicTacToe
{
    /// <inheritdoc />
    /// <summary>
    ///     This class represents a MAX node in the game tree.
    /// </summary>
    public sealed class MaxNode : Node
    {
        /// <inheritdoc />
        /// <summary>
        ///     Constructs a new max node
        /// </summary>
        /// <param name="b">The Board that this node represents</param>
        /// <param name="parent">The node's parent</param>
        /// <param name="m">The move made to create this node's board</param>
        public MaxNode(Board b, Node parent, TicTacToeMove m)
            : base(b, parent, m)
        {
        }

        // Generate Children.  MAX Nodes have MIN children
        protected override void GenerateChildren()
        {
            // Create child nodes for each of the availble positions 
            var openPositions = Board.OpenPositions;
            foreach (var i in openPositions)
            {
                var b = (Board) Board.Clone();
                var m = new TicTacToeMove(i, MyPieceLocal);

                b.MakeMove(i, MyPieceLocal);
                Children.Add(new MinNode(b, this, m));
            }
        }

        // Evaluates how favorable the board configuration is for this node
        protected override void Evaluate()
        {
            Value = EvaluatorLocal.Evaluate(Board, MyPieceLocal);
        }

        // Returns a list of this nodes children sorted in descending order 
        protected override List<Node> SortChildren(IEnumerable<Node> unsortedChildren)
        {
            var sortedChildren = unsortedChildren.OrderByDescending(n => n.Value).ToList();
            return sortedChildren;
        }
    }
}