using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter
{
    /// <summary>
    /// A container set holding bubbles with their potential chance to be popped.
    /// </summary>
    public abstract class BubblePoolSet
    {
        /// <summary>
        /// The Bubbles property holds a collection of related <see cref="Bubble"/> instances.
        /// </summary>
        public Bubble[] Bubbles { get; private set; }

        /// <summary>
        /// The ChanceToPop property indicates the chance the set of bubbles can pop in the next shot.
        /// </summary>
        public float ChanceToPop { get; private set; }

        /// <summary>
        /// Constructs a new group of corresponding <see cref="Bubble"/> instances with a given chance
        /// to be popped.
        /// </summary>
        /// <param name="bubbles">A collection of <see cref="Bubble"/> instances forming the set.</param>
        /// <param name="chanceToPop">The chance of this set popping in the next shot.</param>
        public BubblePoolSet(Bubble[] bubbles, float chanceToPop)
        {
            Bubbles = bubbles;
            ChanceToPop = chanceToPop;
        }

        /// <summary>
        /// The GenerateBubble method generates a <see cref="Bubble"/> instance that can be used as ammunition and
        /// correspondents to the set.
        /// </summary>
        /// <returns>A new <see cref="Bubble"/> instance corresponding to this group as ammunition.</returns>
        public abstract Bubble GenerateBubble();
    }
}