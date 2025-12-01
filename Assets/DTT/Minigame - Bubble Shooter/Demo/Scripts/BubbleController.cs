using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// This class is responsible for handling the behaviour of an individual bubble in a scene.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer), typeof(BubbleMovement))]
    public class BubbleController : MonoBehaviour
    {
        /// <summary>
        /// The _text field is a reference to the text element used to display a potential number value.
        /// </summary>
        [SerializeField]
        [Tooltip("Text element to display number in educative mode.")]
        private Text _text;
        
        /// <summary>
        /// The speed of the bubble when creating a new row.
        /// </summary>
        [SerializeField]
        [Tooltip("Bubble speed.")]
        private float _speed = 2f;

        /// <summary>
        /// The Text property is a reference to the text element used to display a potential number value.
        /// </summary>
        public Text Text => _text;

        /// <summary>
        /// The Manager field to retrieve information about a running game.
        /// </summary>
        public BubbleShooterManager Manager { get; private set; }

        /// <summary>
        /// The Renderer field is a reference to the renderer the controller will be attached to.
        /// </summary>
        public DemoGridRenderer Renderer { get; private set; }

        /// <summary>
        /// The bubble field holds the <see cref="Bubble"/> instance this controller holds.
        /// </summary>
        public Bubble Bubble { get; internal set; }

        /// <summary>
        /// The SpriteRenderer property is a reference to the <see cref="UnityEngine.SpriteRenderer"/> which displays the bubble.
        /// </summary>
        public SpriteRenderer SpriteRenderer { get; private set; }

        /// <summary>
        /// The InitialSpriteBounds property is the initial bounds of the sprite of the bubble in world-space.
        /// </summary>
        public Bounds InitialSpriteBounds { get; private set; }

        /// <summary>
        /// The Movement property is a reference to the movement behaviour of this controller.
        /// </summary>
        public BubbleMovement Movement { get; private set; }

        /// <summary>
        /// The Position property indicates the position the controller is on within its grid. This value can be represented as
        /// null if it is not present in a grid.
        /// </summary>
        public Vector2Int? Position { get; set; }
        
        /// <summary>
        /// Is the bubble moving.
        /// </summary>
        private bool _isMoving;
        
        /// <summary>
        /// The destination of the bubble.
        /// </summary>
        private Vector3 _destination;

        /// <summary>
        /// The Initialize method sets necessary values to identify this controller and have it function properly.
        /// </summary>
        /// <param name="manager">The <see cref="BubbleShooterManager"/> instance to get required information from.</param>
        /// <param name="renderer">The <see cref="DemoGridRenderer"/> instance to attach the controller to.</param>
        /// <param name="bubble">The <see cref="Bubble"/> instance this controller holds.</param>
        public void Initialize(BubbleShooterManager manager, DemoGridRenderer renderer, Bubble bubble)
        {
            Manager = manager;
            Renderer = renderer;
            Bubble = bubble;
        }

        /// <summary>
        /// The Awake method initializes components and initial values.
        /// </summary>
        private void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Movement = GetComponent<BubbleMovement>();

            InitialSpriteBounds = SpriteRenderer.bounds;
        }

        /// <summary>
        /// Move a bubble from hist position to a new position.
        /// </summary>
        /// <param name="bubble">The bubble that will be moved.</param>
        /// <param name="position">The future position of the bubble.</param>
        public void MoveAnimation(Bubble bubble, Vector3 position)
        {
            SpriteRenderer.transform.localPosition = bubble.InitialPosition;
            _destination = position;
            StartCoroutine(MovingAnimation());
        }
        
        /// <summary>
        /// Move a bubble to his new location on the grid.
        /// </summary>
        /// <returns>IEnumerator</returns>
        private IEnumerator MovingAnimation()
        {
            Manager.Turret.canShoot = false;
            yield return new WaitForSeconds(0.3f);

            if(_isMoving)
                yield break;

            _isMoving = true;

            while (true)
            {
                Vector3 currentPosition = transform.localPosition;
                float maxDistanceDelta = _speed * Time.deltaTime;
                transform.localPosition = Vector3.MoveTowards(currentPosition, _destination, maxDistanceDelta);
                
                if (transform.localPosition == _destination)
                {
                    if (Bubble == null)
                        break;

                    Bubble.InitialPosition = transform.localPosition;
                    _isMoving = false;
                    break;
                }

                yield return new WaitForSeconds(0.01f);
            }

            Manager.Turret.canShoot = true;
        }
    }
}