using System;
using UnityEngine;

namespace DTT.BubbleShooter
{
    /// <summary>
    /// The class holding a color configuration for the bubble.
    /// </summary>
    [Serializable]
    public class ColorConfig
    {
        /// <summary>
        /// Holds a list of colors that are used for bubbles.
        /// </summary> 
        [SerializeField]
        [Tooltip("Color for the bubble.")]
        private Color _bubbleColors;

        /// <summary>
        /// <inheritdoc cref="_bubbleColors"/>
        /// </summary>
        public Color BubbleColors => _bubbleColors;

        /// <summary>
        /// Holds a list of weight.
        /// </summary>
        [SerializeField]
        [Tooltip("Spawning weight of the color.")]
        [Range(0,100)]
        private int _weight;

        /// <summary>
        /// Holds a list of weight.
        /// </summary>
        public int Weight => _weight;
    }
}