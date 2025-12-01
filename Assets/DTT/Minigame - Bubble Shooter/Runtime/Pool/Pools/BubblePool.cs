using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.BubbleShooter
{
    /// <summary>
    /// This class handles finding groups of corresponding bubbles in the field and calculates
    /// a chance to pop and generating new ammunition to be picked from.
    /// </summary>
    public abstract class BubblePool
    {
        /// <summary>
        /// The Grid property holds a reference to the grid to find groups of bubbles from.
        /// </summary>
        protected HexagonGrid p_Grid { get; private set; }

        /// <summary>
        /// The _sets field is a list of sets found within the grid.
        /// </summary>
        protected List<BubblePoolSet> p_sets;

        /// <summary>
        /// The Threshold property indicates the threshold of percentage to pop a <see cref="BubblePoolSet"/> should
        /// have to be picked when picking a bubble from the pool.
        /// </summary>
        protected float p_Threshold { get; private set; }

        /// <summary>
        /// Constructs a new pool and generates groups of bubbles into <see cref="BubblePoolSet"/> instances.
        /// </summary>
        /// <param name="grid">A <see cref="HexagonGrid"/> instance that holds cells to compute.</param>
        /// <param name="threshold">The minimum threshold a <see cref="BubblePoolSet"/> has to have to be included when picking.</param>
        public BubblePool(HexagonGrid grid, float threshold)
        {
            p_sets = new List<BubblePoolSet>();
            p_Grid = grid;
            p_Threshold = threshold;
        }

        /// <summary>
        /// The Recompute method clears the current pool and repopulates the pool with new entries of <see cref="BubblePoolSet"/>
        /// instances.
        /// </summary>
        public void Recompute()
        {
            p_sets.Clear();
            OnRecompute();
        }

        /// <summary>
        /// The Recompute method repopulates the pool with new entries of <see cref="BubblePoolSet"/> instances.
        /// </summary>
        protected abstract void OnRecompute();

        /// <summary>
        /// The PickBubble method picks a generated bubble from a <see cref="BubblePoolSet"/> as long as it reaches the given threshold.
        /// <para>
        /// If no sets can be found that reach the threshold, a random set will be picked.
        /// </para>
        /// </summary>
        /// <returns>A <see cref="Bubble"/> instance generated to be used as ammunition.</returns>
        public Bubble PickBubble()
        {
            if (p_sets.Count == 0)
            {
                Debug.LogWarning("A bubble instance was attempted to be picked from the pool, but the pool is empty. If this is expected behaviour, ignore this warning.");
                return null;
            }

            IEnumerable<BubblePoolSet> validSets = p_sets.Where(set => set.ChanceToPop >= p_Threshold);
            BubblePoolSet randomSet = validSets.ElementAtOrDefault(Random.Range(0, validSets.Count()));

            if (randomSet == null)
                return p_sets[Random.Range(0, p_sets.Count)].GenerateBubble();

            return randomSet.GenerateBubble();
        }
    }
}