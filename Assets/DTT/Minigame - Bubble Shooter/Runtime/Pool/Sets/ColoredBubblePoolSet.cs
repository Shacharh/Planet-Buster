using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class ColoredBubblePoolSet : BubblePoolSet
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="bubbles"><inheritdoc/></param>
        /// <param name="chanceToPop"><inheritdoc/></param>
        public ColoredBubblePoolSet(Bubble[] bubbles, float chanceToPop) : base(bubbles, chanceToPop) { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public override Bubble GenerateBubble() => Bubbles[0].Clone();
    }
}