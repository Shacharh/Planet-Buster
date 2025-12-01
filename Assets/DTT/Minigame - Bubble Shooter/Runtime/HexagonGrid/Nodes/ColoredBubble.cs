using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.BubbleShooter
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public class ColoredBubble : Bubble
	{
		/// <summary>
		/// The _color field is the color the bubble visually holds.
		/// </summary>
		private Color _color;

		/// <summary>
		/// The Color property is the color the bubble visually holds.
		/// </summary>
		public Color Color => _color;

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <param name="color">The initial color the bubble holds.</param>
		public ColoredBubble(Color color)
		{
			_color = color;

			AddMatches<ColoredBubble>(
				bubble => bubble.Color == _color,
				bubbleGroup => bubbleGroup.All(bubble => bubble.Color == _color)
				);
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		/// <returns><inheritdoc/></returns>
		public override Bubble Clone() => new ColoredBubble(_color);
    }
}