using UnityEngine;
using UnityEngine.UI; // Uses Legacy Text (Safe!)

namespace DTT.BubbleShooter.Demo
{
    public class ScoreUI : MonoBehaviour
    {
        [Tooltip("Drag your BubbleShooterManager here")]
        [SerializeField] private BubbleShooterManager _manager;

        [Tooltip("Drag your UI Text object here")]
        [SerializeField] private Text _scoreText; 

        // Tracks the number currently shown on screen (float allows for smooth math)
        private float _displayedScore = 0;

        private void Update()
        {
            // Safety check
            if (_manager == null || _scoreText == null) return;

            // 1. Get the "Real" score (which updates instantly in the background)
            int realScore = _manager.Score;

            // 2. Animate: Move the displayed score towards the real score
            if (_displayedScore < realScore)
            {
                // LOGIC: Calculate how far away we are
                float difference = realScore - _displayedScore;

                // LOGIC: The bigger the difference, the faster we count.
                // If we are 1000 points away, we count fast. If 5 points away, we count slow (1 by 1).
                // "Time.deltaTime" makes it run smoothly on all computers.
                float speed = Mathf.Max(20f, difference * 5f); 

                _displayedScore = Mathf.MoveTowards(_displayedScore, realScore, speed * Time.deltaTime);
            }
            // (Optional: If score resets to 0, snap instantly)
            else if (_displayedScore > realScore)
            {
                _displayedScore = realScore;
            }

            // 3. Update the text (Remove decimals using FloorToInt)
            _scoreText.text = "Score: " + Mathf.FloorToInt(_displayedScore).ToString();
        }
    }
}