using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.BubbleShooter.Demo
{
    public class NextBubbleUI : MonoBehaviour
    {
        [Header("Scene References")]
        [Tooltip("Drag the child object 'Next Bubble' here.")]
        [SerializeField] private Image _nextBubbleImage;
        
        [Tooltip("Drag the 'Flare' object itself here (or leave empty if attached to it).")]
        [SerializeField] private Image _flareImage;

        [Header("Assets")]
        [Tooltip("Drag your 5 Bubble Sprites here (Blue, Red, Green, etc.) in order!")]
        [SerializeField] private List<Sprite> _bubbleSprites;

        // Internal list of colors from the Config
        private List<Color> _gameColors;

        private void Awake()
        {
            // Auto-find the flare image if not assigned
            if (_flareImage == null) _flareImage = GetComponent<Image>();
        }

        public void Initialize(List<Color> colors)
        {
            _gameColors = colors;
        }

        public void UpdatePreview(Color nextColor)
        {
            if (_gameColors == null || _gameColors.Count == 0) return;

            // 1. Find which sprite matches this color
            int index = FindClosestColorIndex(nextColor);

            // 2. Set the Child Sprite (The actual bubble PNG)
            if (index >= 0 && index < _bubbleSprites.Count)
            {
                _nextBubbleImage.sprite = _bubbleSprites[index];
                
                // IMPORTANT: Ensure the bubble itself is WHITE so the PNG art shows correctly
                _nextBubbleImage.color = Color.white; 
            }

            // 3. Set the Parent Flare Color (The Glow)
            if (_flareImage != null)
            {
                _flareImage.color = nextColor;
            }
        }

        private int FindClosestColorIndex(Color targetColor)
        {
            float minDiff = float.MaxValue;
            int bestIndex = 0;

            for (int i = 0; i < _gameColors.Count; i++)
            {
                Color c = _gameColors[i];
                float diff = Mathf.Abs(c.r - targetColor.r) + Mathf.Abs(c.g - targetColor.g) + Mathf.Abs(c.b - targetColor.b);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    bestIndex = i;
                }
            }
            return bestIndex;
        }
    }
}