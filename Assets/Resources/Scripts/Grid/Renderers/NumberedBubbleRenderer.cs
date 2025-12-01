using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// This class is responsible for rendering a <see cref="NumberedBubble"/> on a given <see cref="BubbleController"/>.
    /// </summary>
    public class NumberedBubbleRenderer : IBubbleRenderer
    {
        /// <summary>
        /// The _defaultSprite field is the sprite used when showing the <see cref="BubbleController"/>.
        /// </summary>
        private readonly Sprite _defaultSprite;

        /// <summary>
        /// The constructor initializes the <see cref="_defaultSprite"/> field value.
        /// </summary>
        /// <param name="defaultSprite">The default sprite to use when showing the <see cref="BubbleController"/>.</param>
        public NumberedBubbleRenderer(Sprite defaultSprite) => _defaultSprite = defaultSprite;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="bubble"><inheritdoc/></param>
        /// <param name="controller"><inheritdoc/></param>
        public void Render(Bubble bubble, BubbleController controller)
        {
            NumberedBubble numberedBubble = bubble as NumberedBubble;
            controller.SpriteRenderer.sprite = _defaultSprite;
            controller.SpriteRenderer.color = numberedBubble.Color;
            controller.Text.text = numberedBubble.Number.ToString();
        }
    }
}