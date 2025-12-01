using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.BubbleShooter
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public class NumberedBubblePool : BubblePool
	{
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="grid"><inheritdoc/></param>
		/// <param name="threshold"><inheritdoc/></param>
		public NumberedBubblePool(HexagonGrid grid, float threshold) : base(grid, threshold) { }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        protected override void OnRecompute()
        {
			IEnumerable<BubbleWrapper> bubbles = p_Grid.Cells
				.Cast<HexagonCell>()
				.Where(cell => cell.Node != null)
				.Select(cell => new BubbleWrapper(cell.Node, cell.Position));

			List<IEnumerable<BubbleWrapper>> groups = new List<IEnumerable<BubbleWrapper>>();

			foreach (BubbleWrapper bubble in bubbles)
			{
				if (bubble.visited)
					continue;

				Color color = ((NumberedBubble)bubble.Bubble).Color;

				List<Bubble> bubbleGroup = new List<Bubble>();
				bubbleGroup.Add(bubble.Bubble);

				IEnumerable<Bubble> adjacentBubbleGroup = p_Grid.GetAdjacentCellsRecursively(
					bubble.Position,
					cell => bubble.Bubble.IsMatch(cell.Node))
					.Select(cell => cell.Node);
				bubbleGroup.AddRange(adjacentBubbleGroup);

				IEnumerable<BubbleWrapper> wrapperGroup = bubbles.Where(wrapper => bubbleGroup.Contains(wrapper.Bubble));

				foreach (BubbleWrapper wrapper in wrapperGroup)
					wrapper.visited = true;

				groups.Add(wrapperGroup);
			}

			foreach (IEnumerable<BubbleWrapper> group in groups)
			{
				Bubble[] bubbleGroup = group.Select(wrapper => wrapper.Bubble).ToArray();
				float chance = 100f / p_Grid.Height * group.Max(wrapper => wrapper.Position.y);
				p_sets.Add(new NumberedBubblePoolSet(bubbleGroup, chance));
			}
		}
    }
}
