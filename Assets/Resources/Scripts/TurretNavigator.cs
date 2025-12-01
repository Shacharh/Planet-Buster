using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// This class rotates the turret into the direction of where the cursor is positioned on the screen.
    /// </summary>
    public class TurretNavigator : MonoBehaviour
    {
        /// <summary>
        /// The _camera field is used to determine the world position of the cursor's position on the screen.
        /// </summary>
        [SerializeField]
        private Camera _camera;

        /// <summary>
        /// The _manager field is a reference to the <see cref="BubbleShooterManager"/> instance used to listen
        /// for game flow events.
        /// </summary>
        [SerializeField]
        private BubbleShooterManager _manager;

        /// <summary>
        /// The _inputManager field refers to the manager that handles incoming input from a specific platform.
        /// </summary>
        [SerializeField]
        private InputManager _inputManager;

        /// <summary>
        /// The _canMove field indicates whether the turret can move its aiming direction or not.
        /// </summary>
        private bool _canMove;

        /// <summary>
        /// The Awake method initializes the turret to be able to move.
        /// </summary>
        private void Awake() => _canMove = true;

        /// <summary>
        /// The OnEnable method attaches event listeners to freeze or unfreeze the turret.
        /// </summary>
        private void OnEnable()
        {
            _manager.Paused += FreezeTurret;
            _manager.Finish += _ => FreezeTurret();
            _manager.Started += UnfreezeTurret;
            _manager.Continued += UnfreezeTurret;

            _inputManager.ControllerInitialized += InitializeInput;
        }

        /// <summary>
        /// The OnDisable method detaches event listeners to freeze or unfreeze the turret.
        /// </summary>
        private void OnDisable()
        {
            _manager.Paused -= FreezeTurret;
            _manager.Finish -= _ => FreezeTurret();
            _manager.Started -= UnfreezeTurret;
            _manager.Continued -= UnfreezeTurret;

            _inputManager.ControllerInitialized -= InitializeInput;
        }

        /// <summary>
        /// The InitializeInput initializes input from the <see cref="InputManager"/> for hovering over the screen.
        /// </summary>
        private void InitializeInput() => _inputManager.Controller.Hover += HandleHover;

        /// <summary>
        /// The HandleHover method moves the up direction of the transform of the turret accordingly.
        /// </summary>
        /// <param name="position">The position of the cursor to aim the up direction of the turret towards.</param>
        private void HandleHover(Vector2 position)
        {
            if (!_canMove)
                return;

            Vector3 mouseWorldPosition = _camera.ScreenToWorldPoint(position);
            mouseWorldPosition.z = transform.position.z;
            Vector3 mouseDirection = (mouseWorldPosition - transform.position).normalized;
            mouseDirection.y = Mathf.Clamp(mouseDirection.y, 0.2f, 1f);
            transform.up = mouseDirection;
        }

        /// <summary>
        /// The FreezeTurret method disallows the turret to change its aiming direction.
        /// </summary>
        private void FreezeTurret() => _canMove = false;

        /// <summary>
        /// The UnfreezeTurret method allows the turret to change its aiming direction.
        /// </summary>
        private void UnfreezeTurret() => _canMove = true;
    }
}