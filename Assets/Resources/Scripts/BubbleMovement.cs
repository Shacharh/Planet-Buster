using DTT.BubbleShooter.Demo.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// This class handles the behaviour for moving the bubble when shooting it from a turret.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(BubbleController))]
    public class BubbleMovement : MonoBehaviour
    {
        /// <summary>
        /// The _speed field indicates the speed at which the bubble moves through space.
        /// </summary>
        [SerializeField]
        private float _speed = 800f;

        /// <summary>
        /// The _trailRenderer field is a component reference used to render a trail behind the moving bubble.
        /// </summary>
        [SerializeField]
        private TrailRenderer _trailRenderer;

        /// <summary>
        /// The _direction field indicates the continuous direction the bubble will move towards.
        /// </summary>
        private Vector2 _direction;

        /// <summary>
        /// The _controller field is a reference to the controller that controls this movement behaviour.
        /// </summary>
        private BubbleController _controller;

        /// <summary>
        /// The _rigidbody field is a component reference to a <see cref="Rigidbody2D"/> that handles movement through space.
        /// </summary>
        private Rigidbody2D _rigidbody;

        /// <summary>
        /// The _visitedController field is a list that contains <see cref="BubbleController"/> instances this bubble has
        /// seen when travelling through space.
        /// </summary>
        private HashSet<BubbleController> _visitedController;

        /// <summary>
        /// The Awake method initializes related components and sets initial values.
        /// </summary>
        private void Awake()
        {
            _controller = GetComponent<BubbleController>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _visitedController = new HashSet<BubbleController>();
        }

        /// <summary>
        /// The OnEnable method enables emission of the trail renderer.
        /// </summary>
        private void OnEnable() => _trailRenderer.emitting = true;

        /// <summary>
        /// The OnDisable method disabled emission of the trail renderer and stops movement.
        /// </summary>
        private void OnDisable()
        {
            _trailRenderer.emitting = false;
            _rigidbody.linearVelocity = Vector2.zero;
        }

        /// <summary>
        /// The Initialize method sets the initial direction the bubble will move towards.
        /// </summary>
        /// <param name="direction">The direction in which the bubble will move towards.</param>
        public void Initialize(Vector2 direction)
        {
            if(_controller.Bubble is ColoredBubble coloredBubble)
            {
                _trailRenderer.colorGradient = new Gradient()
                {
                    mode = GradientMode.Fixed,
                    colorKeys = new GradientColorKey[] { new GradientColorKey(coloredBubble.Color, 0f) },
                    alphaKeys = _trailRenderer.colorGradient.alphaKeys
                };
            }

            _direction = direction;
            enabled = true;
        }

        /// <summary>
        /// The FixedUpdate method continuously sets the direction the bubble should move towards.
        /// </summary>
        private void FixedUpdate() => _rigidbody.linearVelocity = _direction * _speed * Time.fixedDeltaTime;

        /// <summary>
        /// The Update method continuously checks for any other <see cref="BubbleController"/> instances in front of it
        /// and potentially attaches itself to one and its corresponding grid.
        /// </summary>
        private void Update() {
            Vector2 continuousDirection = _rigidbody.linearVelocity.normalized;
            float continuousDistance = _controller.SpriteRenderer.bounds.extents.magnitude;

            List<RaycastHit2D> continuousHits = new List<RaycastHit2D>();
            foreach (Vector3 rayOffset in new Vector3[] { Vector2.left, Vector2.zero, Vector2.right })
            {
                Vector3 continuousDirectionRight = Vector3.Cross(Vector3.back, continuousDirection);
                Vector3 offsetDirection = continuousDirectionRight * rayOffset.x;
                Vector3 offsetRayPosition = transform.position + (offsetDirection.normalized * _controller.SpriteRenderer.bounds.extents.magnitude / 2);

                continuousHits.AddRange(Physics2D.RaycastAll(offsetRayPosition, continuousDirection, continuousDistance));
                Debug.DrawRay(offsetRayPosition, continuousDirection, Color.red, Time.deltaTime);
            }

            foreach (RaycastHit2D continuousHit in continuousHits)
            {
                if (continuousHit.collider == null || !continuousHit.collider.TryGetComponent(out BubbleController hitController))
                    continue;

                if (!hitController.Position.HasValue)
                    continue;

                if (hitController.Bubble == null)
                {
                    if(hitController.Position.Value.y == 0)
                    {
                        Attach(hitController);
                        return;
                    }

                    if(hitController != _controller)
                        _visitedController.Add(hitController);
                    continue;
                }

                HexagonGrid grid = hitController.Manager.Grid;

                IEnumerable<Vector2Int> hitControllerPositions = grid
                            .GetAdjacentCells(hitController.Position.Value)
                            .Select(cell => cell.Position);

                BubbleController visitingController = _visitedController
                    .Where(controller =>
                        grid.Touch(controller.Position.Value) &&
                        hitControllerPositions.Contains(controller.Position.Value))
                    .MinBy(controller => Vector2.Distance(controller.transform.position, transform.position));

                Attach(visitingController);

                break;
            }
        }

        /// <summary>
        /// The Attach method assigns the bubble to an empty <see cref="BubbleController"/> on the colliding grid.
        /// </summary>
        /// <param name="other">The empty <see cref="BubbleController"/> to replace on the existing grid.</param>
        private void Attach(BubbleController other)
        {
            HexagonGrid grid = other.Manager.Grid;
            other.Renderer.Attach(_controller, other.Position.Value);
            grid.Attach(_controller.Bubble, _controller.Position.Value);

            _visitedController.Clear();
            enabled = false;
        }

        /// <summary>
        /// The OnCollisionEnter2D continuously checks for collisions to bounce the bubble into its reflected direction.
        /// </summary>
        /// <param name="collision">The collided <see cref="Collider2D"/> the bubble collided with.</param>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider == null || collision.collider.GetComponent<BubbleController>() != null)
                return;

            _direction = Vector2.Reflect(_direction, collision.contacts[0].normal);
        }
    }
}