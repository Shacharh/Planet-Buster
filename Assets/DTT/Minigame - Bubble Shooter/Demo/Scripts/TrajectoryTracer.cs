using DTT.BubbleShooter.Demo.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// This class handles drawing a recursive trajectory to indicate what the turret is aiming it.
    /// </summary>
    public class TrajectoryTracer : MonoBehaviour
    {
        /// <summary>
        /// The _maximumRecursions is the amount of times the line trajectory can recursively perform a reflected raycast.
        /// </summary>
        [SerializeField]
        private int _maximumRecursions = 10;

        /// <summary>
        /// The _maximumSegmentLength field is the length in world-space the raycast can cast for.
        /// </summary>
        [SerializeField]
        private float _maximumSegmentLength = 20f;

        /// <summary>
        /// The _lineRendererTemplate is a reference to a prefab instantiated and used when drawing a trajectory line.
        /// </summary>
        [SerializeField]
        private LineRenderer _lineRendererTemplate;

        /// <summary>
        /// The _lineRenderers field holds <see cref="LineRenderer"/> instances used to reference each trajectory segment.
        /// </summary>
        private LineRenderer[] _lineRenderers;

        /// <summary>
        /// The _lineSegmentPoints holds the points in world-space used for each trajectory segment.
        /// </summary>
        private List<Vector3[]> _lineSegmentPoints;

        /// <summary>
        /// The _DIRECTION_HIT_MARGIN field is a constant value that marges outwards of a hit collision point to not collide
        /// with itself again on the next reflected raycast.
        /// </summary>
        private const float _DIRECTION_HIT_MARGIN = 0.01f;

        /// <summary>
        /// The Awake method instantiates line renderer components for each segment of the complete trajectory.
        /// </summary>
        private void Awake()
        {
            _lineSegmentPoints = new List<Vector3[]>();

            _lineRenderers = new LineRenderer[_maximumRecursions];
            for(int i = 0; i < _lineRenderers.Length; i++)
            {
                LineRenderer renderer = _lineRenderers[i] = Instantiate(_lineRendererTemplate, transform);
                renderer.enabled = false;
            }
        }

        /// <summary>
        /// The Update method recursively performs raycasts to draw points of a complete trajectory.
        /// </summary>
        private void Update()
        {
            _lineSegmentPoints.Clear();
            InvokeReflection(transform.position, transform.up);

            for (int i = 0; i < _lineSegmentPoints.Count; i++)
            {
                Vector3[] linePoints = _lineSegmentPoints[i];

                LineRenderer renderer = _lineRenderers[i];
                renderer.enabled = true;

                renderer.positionCount = linePoints.Length;
                for (int positionIndex = 0; positionIndex < linePoints.Length; positionIndex++)
                    renderer.SetPosition(positionIndex, linePoints[positionIndex]);
            }

            for (int i = _lineSegmentPoints.Count; i < _lineRenderers.Length; i++)
                _lineRenderers[i].enabled = false;
        }

        /// <summary>
        /// The InvokeReflection method performs a single raycast and calls itself recursively from a given point into a
        /// given direction.
        /// </summary>
        /// <param name="origin">The point in world-space from which a raycast should be performed.</param>
        /// <param name="direction">The direction in which the raycast will be performed. The given length is unaffected.</param>
        private void InvokeReflection(Vector3 origin, Vector3 direction)
        {
            IEnumerable<RaycastHit2D> raycastHits = Physics2D.RaycastAll(origin, direction, _maximumSegmentLength)
                .Where(hit =>
                hit.collider != null &&
                hit.collider.GetComponent<BubbleController>() == null);

            if (!raycastHits.Any())
            {
                _lineSegmentPoints.Add(new Vector3[] { origin, origin + (direction.normalized * _maximumSegmentLength) });
                return;
            }

            RaycastHit2D raycastHit = raycastHits.MinBy(hit => Vector2.Distance(origin, hit.point));
            Vector2 reflectedDirection = Vector2.Reflect(direction, raycastHit.normal);
            Vector3 marginedHitPosition = raycastHit.point + reflectedDirection.normalized * _DIRECTION_HIT_MARGIN;
            _lineSegmentPoints.Add(new Vector3[] { origin, marginedHitPosition });

            if(_lineSegmentPoints.Count < _maximumRecursions)
                InvokeReflection(marginedHitPosition, reflectedDirection);
        }
    }
}