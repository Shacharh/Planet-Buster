using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.BubbleShooter.Demo
{
    public class TurretShooter : MonoBehaviour
    {
        [Header("Object references")]
        [SerializeField] private BubbleShooterManager _manager;
        [SerializeField] private DemoGridRenderer _renderer;
        [SerializeField] private SpriteRenderer _turretBarrelColorSprite;
        [SerializeField] private SpriteRenderer _turretBarrelOpeningSprite;

        // ---------------------------------------------------------
        // [CHANGED] Now referencing the Tracer script directly
        // ---------------------------------------------------------
        [SerializeField] 
        private TrajectoryTracer _trajectoryTracer; 

        [SerializeField] private Text _turretText;
        [SerializeField] private InputManager _inputManager;

        [Header("Bubble spawning settings")]
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private BubbleController _controllerTemplate;
        
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private float animationAmplitude = 1f; 

        public Transform _canonTransform;

        private void Awake()
        {
            _manager.Initialized += () => _manager.Turret.Reloaded += HandleReload;
            _manager.Started += () => _manager.Turret.Shot += HandleShot;
            _canonTransform = transform.GetChild(0);
        }

        private void OnEnable() => _inputManager.ControllerInitialized += InitializeInput;
        private void OnDisable() => _inputManager.ControllerInitialized -= InitializeInput;
        private void InitializeInput() => _inputManager.Controller.Perform += _ => _manager.ShootTurret(_spawnPoint.up);
        
        private bool _isAnimated = false;

        private void HandleReload(Bubble bubble)
        {
            Bubble currentBubble = _manager.Turret.Bubble;
            Color bubbleColor = Color.white;

            if (currentBubble is ColoredBubble coloredBubble)
                bubbleColor = coloredBubble.Color;

            _turretBarrelColorSprite.color = bubbleColor;
            _turretBarrelOpeningSprite.color = bubbleColor;

            // ---------------------------------------------------------
            // [CHANGED] Call the new method on the Tracer
            // ---------------------------------------------------------
            if (_trajectoryTracer != null)
            {
                _trajectoryTracer.SetTraceColor(bubbleColor);
            }

            string turretText = string.Empty;
            if (currentBubble is NumberedBubble numberedBubble)
                turretText = numberedBubble.Number.ToString();

            _turretText.text = turretText;
        }

        private void HandleShot(Bubble bubble, Vector2 direction)
        {
            animationDuration = 0.06f;
            StartCoroutine(ShootAnimation());
            
            BubbleController currentController = Instantiate(_controllerTemplate, _spawnPoint.position, Quaternion.identity);
            currentController.Initialize(_manager, _renderer, bubble);
            _renderer.Render(bubble, currentController, new Vector3(0,0,0));
            currentController.Movement.Initialize(direction);

            _turretBarrelColorSprite.color = Color.white;
            _turretBarrelOpeningSprite.color = Color.white;

            // ---------------------------------------------------------
            // [CHANGED] Reset Tracer to white
            // ---------------------------------------------------------
            if (_trajectoryTracer != null)
            {
                _trajectoryTracer.SetTraceColor(Color.white);
            }

            _turretText.text = string.Empty;
        }
        
        IEnumerator ShootAnimation()
        {
            if (_isAnimated)
                yield break;
            _isAnimated = true;
            float animationTime = animationDuration;
            while (true)
            {
                animationTime -= Time.deltaTime;
                if (animationTime <= 0)
                {
                    _canonTransform.localScale += new Vector3(0.001f*animationAmplitude, 0.002f* animationAmplitude, 0);
                    if (_canonTransform.localScale.x >= 1)
                    {
                        _isAnimated = false;
                        _canonTransform.localScale = new Vector3(1, 1, 1);
                        break;
                    }
                }
                else
                { 
                    _canonTransform.localScale -= new Vector3(0.001f*animationAmplitude, 0.002f*animationAmplitude, 0);
                }

                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}