using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.BubbleShooter
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class NumberedBubblePoolSet : BubblePoolSet
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="bubbles"><inheritdoc/></param>
        /// <param name="chanceToPop"><inheritdoc/></param>
        public NumberedBubblePoolSet(Bubble[] bubbles, float chanceToPop) : base(bubbles, chanceToPop) { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public override Bubble GenerateBubble()
        {
            Color bubbleColor = ((NumberedBubble) Bubbles[0]).Color;
            int bubbleNumber = Bubbles.Sum(bubble => ((NumberedBubble)bubble).Number);
            return new NumberedBubble(bubbleColor, bubbleNumber);
        }
    }
}