using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// Manage the score in the UI.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        /// <summary>
        /// Text object to display the score
        /// </summary>
        [SerializeField]
        [Tooltip("Text component that holds the score.")]
        private Text _text;

        /// <summary>
        /// BubbleShooterManager of the game.
        /// </summary>
        [SerializeField] 
        [Tooltip("Manager of the game.")]
        private BubbleShooterManager _manager;

        /// <summary>
        /// Score in the bottom pop up prefab.
        /// </summary>
        [SerializeField] 
        [Tooltip("Score popup prefab.")]
        private ScorePopUpScript _scorePopUpBottomPrefab;
        
        /// <summary>
        /// Score on the bubble pop up prefab.
        /// </summary>
        [SerializeField] 
        [Tooltip("Score popup prefab.")]
        private ScorePopUpScript _scorePopUpOnBubblePrefab;

        /// <summary>
        /// Popup holder.
        /// </summary>
        [SerializeField] 
        [Tooltip("Popup holder.")]
        private Transform _popUpHolder;

        /// <summary>
        /// Is score popUp showing on the bubble or in the bottom.
        /// </summary>
        [SerializeField] 
        [Tooltip("Score popup on bottom")]
        private bool _scoreOnBottom;

        /// <summary>
        /// Grid renderer of that game.
        /// </summary>
        [SerializeField]
        [Tooltip("Grid renderer of the game.")]
        private DemoGridRenderer _gridRenderer;

        [SerializeField] 
        [Tooltip("Life time.")]
        private float _lifeTime = 0.4f;

        [SerializeField] 
        [Tooltip("Fading speed.")]
        private float _fadingSpeed = 3f;
        
        [SerializeField]
        [Tooltip(" Moving speed")]
        private float _movingSpeed = 100f;

        /// <summary>
        /// Current score of the game.
        /// </summary>
        private int _score = 0;
        
        /// <summary>
        /// Is the text being updated.
        /// </summary>
        private bool _updateText = false;
        
        /// <summary>
        /// UI canvas.
        /// </summary>
        private Canvas _canvas;

        /// <summary>
        /// Initialize the canvas and text value.
        /// </summary>
        private void Awake()
        {
            _manager.Started += Initialize;
            _text.text = _score.ToString();
            _canvas = GetComponentInParent<Canvas>();
        }
        
        /// <summary>
        /// Initialize the text and score value.
        /// </summary>
        private void Initialize()
        {
            _manager.Grid.Attached += UpdateText;
            _score = 0;
            _text.text = _score.ToString();
        }
        
        /// <summary>
        /// Update the score and make popUp for it.
        /// </summary>
        /// <param name="bubble"> The bubble that is being attached.</param>
        /// <param name="vector2Int"> The position of the bubble being attached.</param>
        /// <param name="isPop"> Is some bubble popping.</param>
        /// <param name="toPop"> List of bubble to pop.</param>
        private void UpdateText(Bubble bubble, Vector2Int vector2Int, bool isPop, List<HexagonCell> toPop)
        {
            int bonus = (int) (toPop.Count / 2) - 1;
            int point = toPop.Count * (10 + bonus * 5);
            if (isPop && _scoreOnBottom)
            {
                _score += point;
                ScorePopUpScript scorePopUpScript = Instantiate(_scorePopUpBottomPrefab, _popUpHolder);
                scorePopUpScript.Setup(point, _fadingSpeed, _lifeTime,_movingSpeed);
                _updateText = true;
            }
            else if (isPop)
            {
                _score += point;
                _updateText = true;

                foreach (HexagonCell cell in toPop)
                {
                    Camera camera = Camera.main;
                    BubbleController controller = _gridRenderer.Controllers[cell.Position.x, cell.Position.y];
                    Vector2 viewportPosition = camera.WorldToViewportPoint(controller.transform.position);

                    Rect pixelRect = _canvas.pixelRect;
                    Vector2 proportionalPosition = new Vector2(viewportPosition.x * pixelRect.width, viewportPosition.y * pixelRect.height);

                    Vector3 position = new Vector3(proportionalPosition.x, proportionalPosition.y, 0);
                    ScorePopUpScript scorePopUp = Instantiate(_scorePopUpOnBubblePrefab, position, Quaternion.identity, _popUpHolder);
                    scorePopUp.Setup(10 + bonus * 5, _fadingSpeed, _lifeTime, _movingSpeed);
                }
            }
        }
        
        /// <summary>
        /// Increment the score.
        /// </summary>
        private void Update()
        {
            if (_updateText)
            {
                int score = int.Parse(_text.text);
                score++;
                if (score > _score)
                {
                    _updateText = false;
                    return;
                }

                _text.text = score.ToString();
            }
        }
    }
}
