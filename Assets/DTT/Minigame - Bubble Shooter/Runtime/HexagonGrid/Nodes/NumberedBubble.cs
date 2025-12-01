using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.BubbleShooter
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class NumberedBubble : ColoredBubble
    {
        /// <summary>
        /// The _number field is the number the bubble visually holds.
        /// </summary>
        private int _number;

        /// <summary>
        /// The Number property is the number the bubble visually holds.
        /// </summary>
        public int Number => _number;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="color"><inheritdoc/></param>
        /// <param name="number">The number the bubble holds.</param>
        public NumberedBubble(Color color, int number) : base(color)
        {
            _number = number;

            AddMatches<NumberedBubble>(
                bubble => bubble.Color == Color,
                bubbleGroup => bubbleGroup.Sum(bubble => bubble.Number) == _number
                );
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public override Bubble Clone() => new NumberedBubble(Color, _number);
    }
}