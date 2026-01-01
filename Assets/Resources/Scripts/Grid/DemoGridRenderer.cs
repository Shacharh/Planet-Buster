using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// This class handles rendering the grid and bubbles.
    /// </summary>
    public class DemoGridRenderer : MonoBehaviour
    {
        /// <summary>
        /// The _manager field references to the <see cref="BubbleShooterManager"/> instance used to retrieve the grid's
        /// information.
        /// </summary>
        [SerializeField]
        private BubbleShooterManager _manager;

        /// <summary>
        /// The _controllerTemplate field is a prefab reference used when instantiating controllers to the grid.
        /// </summary>
        [SerializeField]
        private BubbleController _controllerTemplate;

        /// <summary>
        /// DRAG YOUR 5 CUSTOM SPRITES HERE IN THE INSPECTOR
        /// Ensure the order matches your Config colors (Element 0 = Blue, Element 1 = Red, etc.)
        /// </summary>
        [SerializeField]
        private List<Sprite> _customBubbleSprites;

        /// <summary>
        /// The _spacing field is the distance in world position bubbles have between one another.
        /// </summary>
        [SerializeField]
        private float _spacing = 0.05f;

        /// <summary>
        /// Fading speed for the bubble when they are pop.
        /// </summary>
        [SerializeField] 
        [Tooltip("fading speed for the bubble")]
        private float _fadingSpeed = 10f;
        
        /// <summary>
        /// Left wall of the game.
        /// </summary>
        [SerializeField]
        [Tooltip("The left wall of the game area.")]
        private GameObject _wallsLeft;

        /// <summary>
        /// Right wall of the game.
        /// </summary>
        [SerializeField]
        [Tooltip("The right wall of the game area.")]
        private GameObject _wallsRight;
        
        /// <summary>
        /// Canvas holding the background component.
        /// </summary>
        [SerializeField]
        [Tooltip("Background and design canvas of the game.")]
        private CanvasSet _canvas;
        
        /// <summary>
        /// The _controllers field is a two-dimensional array containing <see cref="BubbleController"/> instances that
        /// represent the grid.
        /// </summary>
        private BubbleController[,] _controllers;
        
        /// <summary>
        /// The _controllers field is a two-dimensional array containing <see cref="BubbleController"/> instances that
        /// represent the grid.
        /// </summary>
        public BubbleController[,] Controllers => _controllers;

        /// <summary>
        /// The _renderers field is a <see cref="Dictionary{TKey, TValue}"/> that references to a renderer for a
        /// corresponding given type of inherited <see cref="Bubble"/>.
        /// </summary>
        private Dictionary<System.Type, IBubbleRenderer> _renderers;

        /// <summary>
        /// Maximum size of each bubble.
        /// </summary>
        private float? _maximumSize = null;

        [Header("Effects")]
        [SerializeField]
        [Tooltip("Drag your Bubble Pop Prefab here (can be a hierarchy)")]
        private GameObject _popEffectPrefab; // CHANGED from ParticleSystem to GameObject

        /// <summary>
        /// The Awake method initializes event listeners for generating and destroying the grid.
        /// </summary>
        private void Awake()
        {
            // We only subscribe to events here.
            // We DO NOT setup colors here, because the Level hasn't been picked yet!
            _manager.Started += DestroyControllers;
            _manager.Started += Generate;
        }

        /// <summary>
        /// The Render method renders the bubble and its controller by the given bubble type.
        /// </summary>
        /// <param name="bubble">The bubble to render on the given controller.</param>
        /// <param name="controller">The controller to render the given bubble for.</param>
        public void Render(Bubble bubble, BubbleController controller, Vector3 position)
        {
            bubble.InitialPosition = position;
            if(_maximumSize.HasValue)
                controller.transform.localScale = new Vector3(_maximumSize.Value, _maximumSize.Value,0);
            
            // Check to prevent crash if renderer not found
            if (_renderers != null && _renderers.ContainsKey(bubble.GetType()))
            {
                _renderers[bubble.GetType()].Render(bubble, controller);
            }
        }

        /// <summary>
        /// The Attach method replaces an existing <see cref="BubbleController"/> on the given position with another given
        /// <see cref="BubbleController"/> instance.
        /// </summary>
        /// <param name="controller">The <see cref="BubbleController"/> instance to be placed on the given position.</param>
        /// <param name="position">The position on where to place the given <see cref="BubbleController"/> instance.</param>
        public void Attach(BubbleController controller, Vector2Int position)
        {
            BubbleController currentController = _controllers[position.x, position.y];
            Vector3 cellPosition = currentController.transform.position;

            _controllers[position.x, position.y] = controller;
            controller.Position = position;

            controller.transform.SetParent(transform);
            controller.transform.position = cellPosition;

            controller.Bubble.InitialPosition = controller.transform.localPosition;

            if (currentController != null)
                Destroy(currentController.gameObject);
        }

        /// <summary>
        /// The Generate method generates an empty grid.
        /// </summary>
        private void Generate()
        {
            // --- LOGIC MOVED HERE: Setup Renderers with the correct Level Config ---
            _renderers = new Dictionary<System.Type, IBubbleRenderer>();
            List<Color> definedColors = new List<Color>();

            // Get the colors from the Manager (which now has the correct Level Config loaded)
            if (_manager.Config != null && _manager.Config.ColorConfiguration != null)
            {
                 definedColors = _manager.Config.ColorConfiguration.Select(c => c.BubbleColors).ToList();
            }

            // Create the renderers passing the Sprites and the Colors
            _renderers.Add(typeof(ColoredBubble), new ColoredBubbleRenderer(_customBubbleSprites, definedColors));
            _renderers.Add(typeof(NumberedBubble), new NumberedBubbleRenderer(_customBubbleSprites, definedColors));
            // -----------------------------------------------------------------------

            _controllers = new BubbleController[_manager.Grid.Width, _manager.Grid.RealHeight];

            _manager.Grid.Updated += Redraw;

            for (int x = 0; x < _manager.Grid.Width; x++)
            {
                for (int y = 0; y < _manager.Grid.RealHeight; y++)
                {
                    Bubble bubble = _manager.Grid[x, y].Node;

                    BubbleController controller = Instantiate(_controllerTemplate, transform);
                    controller.Initialize(_manager, this, bubble);
                    controller.Position = new Vector2Int(x, y);
                    controller.Movement.enabled = false;

                    _controllers[x, y] = controller;
                }
            }

            float scale = GetWallsPosition(_manager.Grid.Width);
            float? canvas = _canvas.UpdateCanvas(scale);
            if (canvas.HasValue)
            {
                scale = canvas.Value;
                _maximumSize = (scale -_spacing * _manager.Grid.Width) / (_manager.Grid.Width);
            }
            else
            {
                _maximumSize = null;
            }

            _wallsLeft.transform.position = new Vector3(-(scale*1.2f) / 2, 0, 0);
            _wallsRight.transform.position = new Vector3((scale*1.2f) / 2, 0, 0);
            
            _manager.Grid.ForceNotifyUpdate();
        }

        /// <summary>
        /// Start the redraw coroutine.
        /// </summary>
        /// <param name="updatedCells">A collection of <see cref="HexagonCell"/> instances that got updated in the grid.</param>
        /// <param name="mode">The <see cref="HexagonRelativityMode"/> currently used to properly display the grid.</param>
        /// <param name="instant">Whether to instantly update the grid or update each cell one-by-one.</param>
        private void Redraw(IEnumerable<HexagonCell> updatedCells, HexagonRelativityMode mode, bool instant)
        {
            StartCoroutine(RedrawCoroutine(updatedCells, mode, instant));
        }

        private float GetWallsPosition(int width)
        {
            if (_controllers[0,0] == null) return 0f; // Safety check
            float bubbleScale = _controllers[0, 0].SpriteRenderer.bounds.size.x;
            return bubbleScale * (width) + _spacing * (width+2);
        }

        /// <summary>
        /// The InitiateHide method handles the visual death of the bubble.
        /// </summary>
        private async void InitiateHide(BubbleController controller)
        {
            // ---------------------------------------------------------
            // [UPDATED] Use Prefab Colors Only (No Tinting)
            // ---------------------------------------------------------
            if (_popEffectPrefab != null)
            {
                // 1. Position and Instantiate
                Vector3 spawnPosition = controller.transform.position;
                GameObject effectInstance = Instantiate(_popEffectPrefab, spawnPosition, Quaternion.identity);

                // 2. Loop through particles ONLY to fix the Layer/Sorting Order
                // We do NOT touch the color.
                ParticleSystem[] childSystems = effectInstance.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem ps in childSystems)
                {
                    var renderer = ps.GetComponent<ParticleSystemRenderer>();
                    if (renderer != null) 
                    {
                        // Forces the explosion to be drawn on top of the bubbles
                        renderer.sortingOrder = 2000; 
                    }
                }

                // 3. Cleanup
                Destroy(effectInstance, 1.0f);
            }
            // ---------------------------------------------------------

            Vector3 initialScale = controller.transform.localScale;
            float scale = 1f;

            while (scale > 0f)
            {
                scale -= _fadingSpeed * Time.deltaTime;
                controller.transform.localScale = initialScale * scale;
                await Task.Yield();
            }

            controller.transform.localScale = initialScale;
            controller.SpriteRenderer.sprite = null;
            controller.Text.text = string.Empty;
        }

        /// <summary>
        /// The DestroyControllers method destroys all <see cref="BubbleController"/> instances currently present on and
        /// outside of the grid.
        /// </summary>
        private void DestroyControllers()
        {
            if(_controllers != null)
                foreach (BubbleController controller in _controllers)
                    if(controller != null) Destroy(controller.gameObject);

            foreach (BubbleController controller in FindObjectsOfType<BubbleController>())
                Destroy(controller.gameObject);

            _controllers = null;
        }

        /// <summary>
        /// Redraws the updated cell with a small delay between each popped cell.
        /// </summary>
        /// <param name="updatedCells">A collection of <see cref="HexagonCell"/> instances that got updated in the grid.</param>
        /// <param name="mode">The <see cref="HexagonRelativityMode"/> currently used to properly display the grid.</param>
        /// <param name="instant">Whether to instantly update the grid or update each cell one-by-one.</param>
        private IEnumerator RedrawCoroutine(IEnumerable<HexagonCell> updatedCells, HexagonRelativityMode mode, bool instant)
        {
             if (!instant)
             {
                 HexagonCell firstUpdated = updatedCells.First();
                 updatedCells = updatedCells.OrderBy(cell => Mathf.Abs((cell.Position - firstUpdated.Position).magnitude));
             }

             foreach (HexagonCell updatedCell in updatedCells)
             {
                 Vector2Int cellPosition = updatedCell.Position;
                 int x = cellPosition.x;
                 int y = cellPosition.y;

                 if(_controllers != null && x < _controllers.GetLength(0) && y < _controllers.GetLength(1)) {
                 
                 BubbleController controller = _controllers[x, y];
                 
                 // Safety Check
                 if(controller == null) continue;

                 Bubble bubble = updatedCell.Node;
                 Bubble oldBubble = controller.Bubble;
                 controller.Bubble = bubble;

                 Vector3 spriteBoundsSize = new Vector3();
                 spriteBoundsSize = _maximumSize.HasValue ? 
                     new Vector3(_maximumSize.Value,_maximumSize.Value,0f) : controller.InitialSpriteBounds.size;
                 if (_maximumSize.HasValue) 
                     controller.transform.localScale = new Vector3(_maximumSize.Value, _maximumSize.Value,0);
                
                 float xPosition = spriteBoundsSize.x * (x - _manager.Grid.Width / 2f) + spriteBoundsSize.x / 4f + (_spacing * x);
                 float yPosition = spriteBoundsSize.y * (_manager.Grid.RealHeight - y) - 
                                   (_manager.Grid.RealHeight * spriteBoundsSize.y) - (_spacing * y);

                 float quarterSpriteWidth = spriteBoundsSize.x / 4;
                 float quarterSpriteHeight = spriteBoundsSize.y / 4;

                 switch (mode)
                 {
                     case HexagonRelativityMode.ODD_R:
                         if (y % 2 != 0) xPosition += quarterSpriteWidth;
                         else xPosition -= quarterSpriteWidth;
                         break;
                     case HexagonRelativityMode.EVEN_R:
                         if (y % 2 == 0) xPosition += quarterSpriteWidth;
                         else xPosition -= quarterSpriteWidth;
                         break;
                     case HexagonRelativityMode.ODD_Q:
                         if (x % 2 != 0) yPosition -= quarterSpriteHeight;
                         else yPosition += quarterSpriteHeight;
                         break;
                     case HexagonRelativityMode.EVEN_Q:
                         if (x % 2 == 0) yPosition -= quarterSpriteHeight;
                         else yPosition += quarterSpriteHeight;
                         break;
                 }
                 controller.transform.localPosition = new Vector3(xPosition, yPosition, 0);

                 if (bubble != null)
                 {
                     // If first row, set initial position 2 rows up.
                     if(bubble.InitialPosition == new Vector3(0,0,0)) 
                         bubble.InitialPosition = new Vector3(xPosition, yPosition + spriteBoundsSize.y +_spacing, 0);
                     // If new line is added, make bubble move down.
                     if (bubble.InitialPosition != null && instant)
                         controller.MoveAnimation(bubble, new Vector3(xPosition, yPosition, 0));
                     
                     Render(bubble, controller, new Vector3(xPosition, yPosition, 0));
                 }
                 else if (oldBubble != null)
                 {
                     InitiateHide(controller);
                 }
                 else
                 {
                     controller.SpriteRenderer.sprite = null;
                     controller.Text.text = string.Empty;
                 }
                 
                 if (!instant)
                     yield return new WaitForSeconds(0.04f);
                 }
             }
             if(!instant)
                 _manager.Turret.canShoot = true;
        }
    }
}