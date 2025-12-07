using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter.Demo
{
    public class ColoredBubbleRenderer : IBubbleRenderer
    {
        private readonly List<Sprite> _customSprites;
        private readonly List<Color> _gameColors;

        public ColoredBubbleRenderer(List<Sprite> customSprites, List<Color> gameColors) 
        {
            _customSprites = customSprites;
            _gameColors = gameColors;
        }

        public void Render(Bubble bubble, BubbleController controller)
        {
            ColoredBubble coloredBubble = bubble as ColoredBubble;
    
            if (coloredBubble != null)
            {
                // DEBUG 1: Check if we even have sprites or colors to work with
                if (_customSprites == null || _customSprites.Count == 0)
                {
                    Debug.LogError("CRITICAL ERROR: _customSprites list is EMPTY! Did you forget to assign them in the Inspector?");
                    return;
                }

                if (_gameColors == null || _gameColors.Count == 0)
                {
                    Debug.LogError("CRITICAL ERROR: _gameColors list is EMPTY! The Config is not passing colors to the renderer.");
                    // Fallback to 0 so we see something
                    controller.SpriteRenderer.sprite = _customSprites[0];
                    return;
                }

                // 1. Find the index
                int colorIndex = FindClosestColorIndex(coloredBubble.Color);

                // DEBUG 2: The "Why is it Blue?" Log
                // This will print exactly which color the bubble IS, and which index it chose.
                // If you see "Index: 0" repeatedly, check the "Bubble Color" value.
                // Debug.Log($"Bubble Color: {coloredBubble.Color} | Match Index: {colorIndex} | Total Game Colors: {_gameColors.Count}");

                // 3. Assign the sprite
                if (colorIndex >= 0 && colorIndex < _customSprites.Count)
                {
                    controller.SpriteRenderer.sprite = _customSprites[colorIndex];
                }
                else 
                {
                    Debug.LogWarning($"Index {colorIndex} is out of bounds! (You have {_customSprites.Count} sprites). Defaulting to 0.");
                    controller.SpriteRenderer.sprite = _customSprites[0];
                }

                // 4. Reset tint to white
                controller.SpriteRenderer.color = Color.white;
                controller.Text.text = string.Empty;
            }
        }

        private int FindClosestColorIndex(Color targetColor)
        {
            if (_gameColors == null || _gameColors.Count == 0) return -1;

            int bestIndex = -1;
            float closestDistance = 100f; // Start with a huge distance

            for (int i = 0; i < _gameColors.Count; i++)
            {
                Color candidate = _gameColors[i];
                
                // Calculate difference
                float diff = Mathf.Abs(candidate.r - targetColor.r) +
                             Mathf.Abs(candidate.g - targetColor.g) +
                             Mathf.Abs(candidate.b - targetColor.b);

                // We allow a very large tolerance (1.0f) to catch ANY color that is remotely similar
                if (diff < closestDistance)
                {
                    closestDistance = diff;
                    bestIndex = i;
                }
            }
            
            return bestIndex;
        }
    }
}