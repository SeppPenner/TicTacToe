using System.Collections.Generic;
using System.Linq;

namespace TicTacToe
{
    /// <inheritdoc />
    /// <summary>
    ///     This class represents a MIN node in the game tree
    /// </summary>
    public sealed class MinNode : Node
    {
        /// <inheritdoc />
        /// <summary>
        ///     Constructs a MIN node
        /// </summary>
        /// <param name="b">The board that this node represents</param>
        /// <param name="parent">This node's parent</param>
        /// <param name="m">The move that was made from the parent to lead to this node's board</param>
        public MinNode(Board b, Node parent, TicTacToeMove m)
            : base(b, parent, m)
        {
        }

        // Generates the node's children.  MIN nodes have MAX children
        protected override void GenerateChildren()
        {
            var openPositions = Board.OpenPositions;
            foreach (var i in openPositions)
            {
                var b = (Board) Board.Clone();
                var m = new TicTacToeMove(i, MyPieceLocal);
                b.MakeMove(i, MyPieceLocal);
                Children.Add(new MaxNode(b, this, m));
            }
        }

        // Returns a list of the child nodes in ascending order
        // the first node in the list will be the best node for the min node
        protected override List<Node> SortChildren(IEnumerable<Node> unsortedChildren)
        {
            var sortedChildren = unsortedChildren.OrderBy(n => n.Value).ToList();
            return sortedChildren;
        }

        /// <summary>
        ///     Evalutes the value of the node using the evaluation function
        /// </summary>
        protected override void Evaluate()
        {
            Value = EvaluatorLocal.Evaluate(Board, Board.GetOponentPiece(MyPieceLocal));
        }
    }
}