using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter
{
    /// <summary>
    /// This wrapper class is used when computing groups of <see cref="BubbleShooter.Bubble"/> instances.
    /// </summary>
    public class BubbleWrapper
    {
        /// <summary>
        /// The Bubble property is used to identify a <see cref="BubbleShooter.Bubble"/> instance in the wrapper.
        /// </summary>
        public Bubble Bubble { get; private set; }

        /// <summary>
        /// The Position property indicates the position of the cell in the grid.
        /// </summary>
        public Vector2Int Position { get; private set; }

        /// <summary>
        /// The visited field indicates whether the bubble has been visited when computing for groups.
        /// </summary>
        public bool visited;

        /// <summary>
        /// Constructs a new wrapper for a given <see cref="BubbleShooter.Bubble"/> instance.
        /// </summary>
        /// <param name="bubble">The <see cref="BubbleShooter.Bubble"/> instance for the wrapper.</param>
        /// <param name="position">The zero-based position of the cell in the grid.</param>
        public BubbleWrapper(Bubble bubble, Vector2Int position)
        {
            Bubble = bubble;
            Position = position;
        }
    }
}