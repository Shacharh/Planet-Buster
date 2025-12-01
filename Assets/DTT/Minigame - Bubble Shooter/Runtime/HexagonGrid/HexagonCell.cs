using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.BubbleShooter
{
    /// <summary>
    /// This class represents a cell in a grid holding a specific type of node.
    /// </summary>
    public class HexagonCell
    {
        /// <summary>
        /// The Context property is the <see cref="HexagonGrid"/> context of this cell.
        /// </summary>
        public HexagonGrid Context { get; private set; }

        /// <summary>
        /// The Position property defines where the cell is placed in the grid.
        /// </summary>
        public Vector2Int Position { get; private set; }

        /// <summary>
        /// The _node field is the node that is held by the cell.
        /// </summary>
        private Bubble _node;

        /// <summary>
        /// The Node property is the node that is held by the cell.
        /// </summary>
        public Bubble Node
		{
            get => _node;
            set
			{
                if (_node != null)
                    _node.ToPop -= PopSelf;

                _node = value;

                if (_node != null)
                    _node.ToPop += PopSelf;
			}
		}

        /// <summary>
        /// Constructs a new cell with a position and optionally a node.
        /// </summary>
        /// <param name="context">The <see cref="HexagonGrid"/> instance the cell lives in as context.</param>
        /// <param name="position">The position the cell is located at in the grid.</param>
        /// <param name="initialNode">The (optional) node held by the cell.</param>
        public HexagonCell(HexagonGrid context, Vector2Int position, Bubble initialNode = null)
        {
            Context = context;
            Position = position;
            Node = initialNode;
        }

        /// <summary>
        /// The PopSelf method pops the bubble from the grid.
        /// </summary>
        private void PopSelf() => Context.Pop(Position);
    }
}