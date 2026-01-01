using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DTT.BubbleShooter.Demo.Utility;
using UnityEngine;

namespace DTT.BubbleShooter.Demo
{
    public class TrajectoryTracer : MonoBehaviour
    {
        [SerializeField] private int _maximumRecursions = 10;
        [SerializeField] private float _maximumSegmentLength = 20f;
        [SerializeField] private LineRenderer _lineRendererTemplate;

        // Holds the active line segments
        private LineRenderer[] _lineRenderers;
        private List<Vector3[]> _lineSegmentPoints;
        private const float _DIRECTION_HIT_MARGIN = 0.01f;
        private int bubbleHits = 0;

        private void Awake()
        {
            _lineSegmentPoints = new List<Vector3[]>();

            _lineRenderers = new LineRenderer[_maximumRecursions];
            for (int i = 0; i < _lineRenderers.Length; i++)
            {
                LineRenderer renderer = _lineRenderers[i] = Instantiate(_lineRendererTemplate, transform);
                renderer.enabled = false;
            }
        }

        private void Update()
        {
            _lineSegmentPoints.Clear();
            bubbleHits = 0;
            InvokeReflection(transform.position, transform.up);

            for (int i = 0; i < _lineSegmentPoints.Count; i++)
            {
                Vector3[] linePoints = _lineSegmentPoints[i];
                LineRenderer renderer = _lineRenderers[i];
                
                // Ensure the renderer is enabled
                renderer.enabled = true;
                renderer.positionCount = linePoints.Length;
                
                for (int positionIndex = 0; positionIndex < linePoints.Length; positionIndex++)
                    renderer.SetPosition(positionIndex, linePoints[positionIndex]);
            }

            // Disable unused segments
            for (int i = _lineSegmentPoints.Count; i < _lineRenderers.Length; i++)
                _lineRenderers[i].enabled = false;
        }

        // ---------------------------------------------------------
        // [ADDED] Public method to change color of ALL segments
        // ---------------------------------------------------------
        public void SetTraceColor(Color color)
        {
            if (_lineRenderers == null) return;

            // Create a gradient to ensure the color overrides everything
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );

            foreach (var renderer in _lineRenderers)
            {
                if (renderer != null)
                {
                    renderer.colorGradient = gradient;
                    renderer.startColor = color;
                    renderer.endColor = color;
                }
            }
        }

        private void InvokeReflection(Vector3 origin, Vector3 direction)
        {
            IEnumerable<RaycastHit2D> raycastHits = Physics2D.RaycastAll(origin, direction, _maximumSegmentLength)
                .Where(PredicateHits());

            if (!raycastHits.Any())
            {
                _lineSegmentPoints.Add(
                    new Vector3[] { origin, origin + (direction.normalized * _maximumSegmentLength) });
                return;
            }

            RaycastHit2D raycastHit = raycastHits.MinBy(hit => Vector2.Distance(origin, hit.point));
            Vector2 reflectedDirection = Vector2.Reflect(direction, raycastHit.normal);
            Vector3 marginedHitPosition = raycastHit.point + reflectedDirection.normalized * _DIRECTION_HIT_MARGIN;
    
            _lineSegmentPoints.Add(new Vector3[] { origin, marginedHitPosition });

            bool hitBubble = raycastHit.collider.GetComponent<BubbleController>() != null;
            bool isReflectingTowardsPlayer = Vector2.Dot(reflectedDirection.normalized, Vector2.down) > 0f;

            if (_lineSegmentPoints.Count < _maximumRecursions && !hitBubble && !isReflectingTowardsPlayer)
            {
                InvokeReflection(marginedHitPosition, reflectedDirection);
            }
        }

        private Func<RaycastHit2D, bool> PredicateHits()
        {
            return hit =>
            {
                if (hit.collider != null && hit.collider.GetComponent<BubbleController>() == null)
                    return true;
                else if (hit.collider.GetComponent<BubbleController>() && hit.collider.GetComponent<SpriteRenderer>().sprite != null)
                {
                    bubbleHits++;
                    return bubbleHits >= 1;
                }
                return false;
            };
        }
    }
}