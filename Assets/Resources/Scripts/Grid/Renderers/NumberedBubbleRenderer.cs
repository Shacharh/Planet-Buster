using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter.Demo
{
    public class NumberedBubbleRenderer : IBubbleRenderer
    {
        private readonly List<Sprite> _customSprites;
        private readonly List<Color> _gameColors;

        public NumberedBubbleRenderer(List<Sprite> customSprites, List<Color> gameColors)
        {
            _customSprites = customSprites;
            _gameColors = gameColors;
        }

        public void Render(Bubble bubble, BubbleController controller)
        {
            NumberedBubble numberedBubble = bubble as NumberedBubble;

            if (numberedBubble != null)
            {
                // CHANGE: Use the same helper logic here
                int colorIndex = FindClosestColorIndex(numberedBubble.Color);

                if (colorIndex != -1 && colorIndex < _customSprites.Count)
                {
                    controller.SpriteRenderer.sprite = _customSprites[colorIndex];
                }
                else if (_customSprites.Count > 0)
                {
                    controller.SpriteRenderer.sprite = _customSprites[0];
                }

                controller.SpriteRenderer.color = Color.white;
                controller.Text.text = numberedBubble.Number.ToString();
            }
        }

        private int FindClosestColorIndex(Color targetColor)
        {
            int bestIndex = -1;
            float minDifference = 0.05f; 

            for (int i = 0; i < _gameColors.Count; i++)
            {
                Color candidate = _gameColors[i];
                float diff = Mathf.Abs(candidate.r - targetColor.r) +
                             Mathf.Abs(candidate.g - targetColor.g) +
                             Mathf.Abs(candidate.b - targetColor.b);

                if (diff < minDifference)
                {
                    minDifference = diff;
                    bestIndex = i;
                }
            }
            return bestIndex;
        }
    }
}